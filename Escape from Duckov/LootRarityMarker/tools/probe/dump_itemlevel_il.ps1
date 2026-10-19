# Dump raw IL bytes of the value->rarity mapping methods, so the thresholds can be read directly.
$ErrorActionPreference = 'Stop'
$managed = 'd:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data\Managed'
$modDir  = 'D:\app\steam\steamapps\workshop\content\3167020\3588386576'
$dll     = Join-Path $modDir 'ItemLevelAndSearchSoundMod.dll'

$global:LRM_Seen = [System.Collections.Generic.HashSet[string]]::new()
$global:LRM_Dirs = @($managed, $modDir)
$handler = [System.ResolveEventHandler] {
    param($s, $e)
    try {
        $simple = [System.Reflection.AssemblyName]::new($e.Name).Name
        if ([string]::IsNullOrEmpty($simple)) { return $null }
        if (-not $global:LRM_Seen.Add($simple)) { return $null }
        foreach ($d in $global:LRM_Dirs) {
            $p = [System.IO.Path]::Combine($d, $simple + '.dll')
            if ([System.IO.File]::Exists($p)) { return [System.Reflection.Assembly]::LoadFrom($p) }
        }
    } catch { }
    return $null
}
[System.AppDomain]::CurrentDomain.add_AssemblyResolve($handler)
$asm = [System.Reflection.Assembly]::LoadFrom($dll)

function Dump-Method([string] $typeName, [string] $methodName) {
    $t = $asm.GetType($typeName)
    if ($null -eq $t) { Write-Output "!! type not found: $typeName"; return }
    foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Static -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::DeclaredOnly)) {
        if ($m.Name -ne $methodName) { continue }
        Write-Output ("### " + $t.FullName + "." + $m.Name)
        try { $body = $m.GetMethodBody() } catch { $body = $null }
        if ($null -eq $body) { Write-Output "   (no body)"; continue }
        $il = $body.GetILAsByteArray()
        Write-Output ("   IL 长度 = " + $il.Length)
        Write-Output ("   " + (($il | ForEach-Object { $_.ToString('x2') }) -join ' '))
        Write-Output ""
    }
}

Dump-Method 'ItemLevelAndSearchSoundMod.Util' 'CalculateItemValueLevel'
Dump-Method 'ItemLevelAndSearchSoundMod.Util' 'GetItemValueLevel'
Dump-Method 'ItemLevelAndSearchSoundMod.Util' 'ParseQuality'
Dump-Method 'ItemLevelAndSearchSoundMod.Util' 'ParseDisplayQuality'
Dump-Method 'ItemLevelAndSearchSoundMod.Util' 'GetItemValueLevelColor'
Dump-Method 'ItemLevelAndSearchSoundMod.Util' 'GetInspectingTime'

Write-Output "=== 静态构造 / 初始化里的数字 ==="
foreach ($t in $asm.GetTypes()) {
    if ($t.Name -eq 'Util' -or $t.Name -eq 'ModBehaviour') {
        foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]::Static -bor [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::DeclaredOnly)) {
            if ($m.Name -notlike '*cctor*' -and $m.Name -ne '.cctor') { continue }
            try { $body = $m.GetMethodBody() } catch { continue }
            if ($null -eq $body) { continue }
            $il = $body.GetILAsByteArray()
            Write-Output ("### " + $t.Name + "..cctor  IL=" + $il.Length)
            Write-Output ("   " + (($il | ForEach-Object { $_.ToString('x2') }) -join ' '))
        }
    }
}
