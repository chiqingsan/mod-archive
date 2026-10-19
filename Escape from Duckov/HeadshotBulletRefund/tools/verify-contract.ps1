param(
    [Parameter(Mandatory = $true)][string]$Dll,
    [Parameter(Mandatory = $true)][string]$ManagedDir,
    [string]$ModName = 'HeadshotBulletRefund'
)

# 用反射复现游戏 ModManager.ActivateMod 的加载契约：
#   Assembly.LoadFrom(dll).GetType("<name>.ModBehaviour") 非空且继承 Duckov.Modding.ModBehaviour
#
# 重要：AssemblyResolve 处理器里绝不要调用 PowerShell cmdlet（Test-Path/Join-Path）。
# cmdlet 查找会触发资源程序集解析 -> 再次进入处理器 -> 递归 -> 栈溢出（0xC00000FD）。
# 处理器内只用 .NET 静态方法，并用 seen 集合兜底防重入。
$ErrorActionPreference = 'Stop'

$global:HRFM_Seen = [System.Collections.Generic.HashSet[string]]::new()
$global:HRFM_ManagedDir = $ManagedDir

$handler = [System.ResolveEventHandler] {
    param($sender, $eventArgs)

    $resolved = $null
    try {
        $simpleName = ([System.Reflection.AssemblyName]::new($eventArgs.Name)).Name
        if (-not $global:HRFM_Seen.Contains($simpleName)) {
            [void]$global:HRFM_Seen.Add($simpleName)
            $candidate = [System.IO.Path]::Combine($global:HRFM_ManagedDir, $simpleName + '.dll')
            if ([System.IO.File]::Exists($candidate)) {
                $resolved = [System.Reflection.Assembly]::LoadFrom($candidate)
            }
        }
    }
    catch {
        $resolved = $null
    }
    return $resolved
}

[System.AppDomain]::CurrentDomain.add_AssemblyResolve($handler)
try {
    $asm = [System.Reflection.Assembly]::LoadFrom($Dll)
    Write-Output ("assembly  : {0}" -f $asm.GetName().Name)
    Write-Output ("imageVer  : {0}" -f $asm.ImageRuntimeVersion)

    $typeName = "$ModName.ModBehaviour"
    $type = $asm.GetType($typeName, $true)
    $baseType = $type.BaseType
    Write-Output ("type      : {0}" -f $type.FullName)
    Write-Output ("baseType  : {0}" -f $baseType.FullName)
    Write-Output ("isAbstract: {0}" -f $type.IsAbstract)
    Write-Output ("baseOfBase: {0}" -f $baseType.BaseType.FullName)

    $ok = ($type.FullName -eq $typeName) -and
          ($baseType.FullName -eq 'Duckov.Modding.ModBehaviour') -and
          (-not $type.IsAbstract)

    $flags = [System.Reflection.BindingFlags]::Instance -bor
             [System.Reflection.BindingFlags]::NonPublic -bor
             [System.Reflection.BindingFlags]::Public -bor
             [System.Reflection.BindingFlags]::DeclaredOnly
    $names = @()
    foreach ($m in $type.GetMethods($flags)) { $names += $m.Name }
    Write-Output ("methods   : {0}" -f (($names | Sort-Object -Unique) -join ', '))

    $sflags = [System.Reflection.BindingFlags]::Static -bor
              [System.Reflection.BindingFlags]::NonPublic -bor
              [System.Reflection.BindingFlags]::Public -bor
              [System.Reflection.BindingFlags]::DeclaredOnly
    $fields = @()
    foreach ($f in $type.GetFields($sflags)) { $fields += ("{0} {1}" -f $f.FieldType.Name, $f.Name) }
    Write-Output ("statics   : {0}" -f ($fields -join ', '))

    $missing = @()
    foreach ($r in $asm.GetReferencedAssemblies()) {
        $p = [System.IO.Path]::Combine($ManagedDir, $r.Name + '.dll')
        if ([System.IO.File]::Exists($p)) { continue }
        $missing += $r.Name
    }
    Write-Output ("refs      : {0} referenced, missing in Managed: [{1}]" -f $asm.GetReferencedAssemblies().Count, ($missing -join ', '))
    if ($missing.Count -gt 0) { $ok = $false }

    Write-Output ("CONTRACT  : {0}" -f $(if ($ok) { 'PASS' } else { 'FAIL' }))
    if (-not $ok) { exit 1 }
}
catch {
    Write-Output ("ERROR     : " + $_.Exception.ToString())
    exit 2
}
finally {
    [System.AppDomain]::CurrentDomain.remove_AssemblyResolve($handler)
}
