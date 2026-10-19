# Dump ItemLevelAndSearchSoundMod.dll: types, methods, fields, and every IL ldc.i4 constant.
# Purpose: recover the value -> rarity thresholds that mod uses, so LootRarityMarker can match
# the rarity the player actually sees in the loot UI.
$ErrorActionPreference = 'Stop'

$managed = 'd:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data\Managed'
$modDir  = 'D:\app\steam\steamapps\workshop\content\3167020\3588386576'
$dll     = Join-Path $modDir 'ItemLevelAndSearchSoundMod.dll'

# NOTE: inside this handler use ONLY .NET statics -- PowerShell cmdlets re-enter the resolver.
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
        $p2 = [System.IO.Path]::Combine($managed, $simple + '.dll')
        if ([System.IO.File]::Exists($p2)) { return [System.Reflection.Assembly]::LoadFrom($p2) }
    } catch { }
    return $null
}
[System.AppDomain]::CurrentDomain.add_AssemblyResolve($handler)

$asm = [System.Reflection.Assembly]::LoadFrom($dll)
Write-Output ("ASSEMBLY: " + $asm.FullName)
Write-Output ""

# ---- IL constant extractor (ldc.i4 family) ----
function Get-IlConstants([System.Reflection.MethodBase] $m) {
    $out = New-Object System.Collections.Generic.List[string]
    try { $body = $m.GetMethodBody() } catch { return $out }
    if ($null -eq $body) { return $out }
    $il = $body.GetILAsByteArray()
    if ($null -eq $il) { return $out }
    $i = 0
    while ($i -lt $il.Length) {
        $op = $il[$i]
        switch ($op) {
            0x1F { if ($i + 4 -lt $il.Length) { $out.Add([string][sbyte]$il[$i + 1]) }; $i += 5; continue }
            0x20 { if ($i + 4 -lt $il.Length) { $out.Add([string][System.BitConverter]::ToInt32($il, $i + 1)) }; $i += 5; continue }
            0x21 { if ($i + 8 -lt $il.Length) { $out.Add([string][System.BitConverter]::ToInt64($il, $i + 1)) }; $i += 9; continue }
            0x22 { if ($i + 4 -lt $il.Length) { $v = [System.BitConverter]::ToSingle($il, $i + 1); if ($v -gt 1 -and $v -lt 100000000) { $out.Add("f:" + $v) } }; $i += 5; continue }
            0x23 { if ($i + 8 -lt $il.Length) { $v = [System.BitConverter]::ToDouble($il, $i + 1); if ($v -gt 1 -and $v -lt 100000000) { $out.Add("d:" + $v) } }; $i += 9; continue }
        }
        if ($op -ge 0x08 -and $op -le 0x0E) { $out.Add([string]($op - 6)) }
        elseif ($op -eq 0x15) { $out.Add("-1") }
        elseif ($op -ge 0x16 -and $op -le 0x1E) { $out.Add([string]($op - 0x16)) }
        $i += 1
    }
    return $out
}

foreach ($t in $asm.GetTypes()) {
    Write-Output ("=" * 70)
    Write-Output ("TYPE " + $t.FullName + "  (base " + $t.BaseType + ")")

    foreach ($f in $t.GetFields([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Static -bor [System.Reflection.BindingFlags]::Instance)) {
        $v = '<unread>'
        try {
            if ($f.IsLiteral) { $v = [string]$f.GetRawConstantValue() }
            elseif ($f.IsStatic) { $v = [string]$f.GetValue($null) }
        } catch { $v = '<err>' }
        Write-Output ("   FIELD " + $f.FieldType.Name + " " + $f.Name + " = " + $v)
    }

    foreach ($m in $t.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Static -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::DeclaredOnly)) {
        $consts = Get-IlConstants $m
        $sig = ($m.GetParameters() | ForEach-Object { $_.ParameterType.Name }) -join ', '
        Write-Output ("   METHOD " + $m.ReturnType.Name + " " + $m.Name + "(" + $sig + ")")
        if ($consts.Count -gt 0) {
            $uniq = $consts | Select-Object -Unique
            Write-Output ("        IL 常量: " + ($uniq -join ' '))
        }
    }
    Write-Output ""
}
