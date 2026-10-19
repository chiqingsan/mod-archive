$ErrorActionPreference = 'Stop'
$ManagedDir = 'd:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data\Managed'
$global:EPO_Seen = New-Object 'System.Collections.Generic.HashSet[string]'
$global:EPO_ManagedDir = $ManagedDir

$handler = [System.ResolveEventHandler] {
    param($s, $e)
    $simple = [System.Reflection.AssemblyName]::new($e.Name).Name
    if ($global:EPO_Seen.Add($simple)) {
        $p = [System.IO.Path]::Combine($global:EPO_ManagedDir, $simple + '.dll')
        if ([System.IO.File]::Exists($p)) { return [System.Reflection.Assembly]::LoadFrom($p) }
    }
    return $null
}
[System.AppDomain]::CurrentDomain.add_AssemblyResolve($handler)

$epo = [System.Reflection.Assembly]::LoadFrom([System.IO.Path]::Combine($ManagedDir, 'EPO.dll'))

function Dump-Type([string]$name) {
    $t = $epo.GetType($name, $false)
    if ($null -eq $t) { Write-Output "!! 未找到 $name"; return }
    Write-Output "===== $name ====="
    Write-Output ("  kind      : " + $(if ($t.IsEnum) {'enum'} elseif ($t.IsValueType) {'struct'} elseif ($t.IsSubclassOf([System.Delegate])) {'delegate'} else {'class'}) + "  base=" + $t.BaseType)
    if ($t.IsEnum) {
        Write-Output ("  values    : " + ([System.Enum]::GetNames($t) -join ', '))
        return
    }
    foreach ($c in $t.GetConstructors()) {
        $ps = @($c.GetParameters() | ForEach-Object { $_.ParameterType.Name + ' ' + $_.Name })
        Write-Output ("  ctor(" + ($ps -join ', ') + ")")
    }
    foreach ($p in $t.GetProperties([System.Reflection.BindingFlags]'Public,Instance,Static,DeclaredOnly')) {
        Write-Output ("  prop  " + $p.PropertyType.Name + ' ' + $p.Name + '  {' + $(if ($p.CanRead) {'get;'} else {''}) + $(if ($p.CanWrite) {'set;'} else {''}) + '}')
    }
    foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]'Public,Instance,Static,DeclaredOnly') | Where-Object { -not $_.IsSpecialName }) {
        $ps = @($m.GetParameters() | ForEach-Object { $_.ParameterType.Name + ' ' + $_.Name })
        Write-Output ("  meth  " + $m.ReturnType.Name + ' ' + $m.Name + '(' + ($ps -join ', ') + ')')
    }
    if ($t.IsSubclassOf([System.Delegate])) {
        $iv = $t.GetMethod('Invoke')
        $ps = @($iv.GetParameters() | ForEach-Object { $_.ParameterType.Name + ' ' + $_.Name })
        Write-Output ("  >>> DELEGATE Invoke: " + $iv.ReturnType.Name + ' (' + ($ps -join ', ') + ')')
    }
    foreach ($f in $t.GetFields([System.Reflection.BindingFlags]'Public,Instance,Static,DeclaredOnly')) {
        Write-Output ("  field " + $f.FieldType.Name + ' ' + $f.Name)
    }
    Write-Output ""
}

Dump-Type 'EPOOutline.OutlineTarget'
Dump-Type 'EPOOutline.OutlineTargetProvider'
Dump-Type 'EPOOutline.RendererFilteringUtility'
Dump-Type 'EPOOutline.Outlinable'
Dump-Type 'EPOOutline.OutlineEffect'
