$ErrorActionPreference = 'Stop'
$log = "$env:USERPROFILE\AppData\LocalLow\TeamSoda\Duckov\Player.log"
$lines = Get-Content $log -Encoding UTF8

$nums = New-Object System.Collections.Generic.List[int]
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i].Contains('StaminaHUD.Update_Patch1')) { $nums.Add($i + 1) }
}
Write-Output ("StaminaHUD NRE 次数: " + $nums.Count)
if ($nums.Count -gt 0) {
    Write-Output ("首次出现行: " + $nums[0] + "  末次出现行: " + $nums[$nums.Count - 1])
    Write-Output "分布（每 1000 行一个桶）:"
    $bucket = @{}
    foreach ($n in $nums) { $b = [Math]::Floor($n / 1000); if ($bucket.ContainsKey($b)) { $bucket[$b]++ } else { $bucket[$b] = 1 } }
    $keys = $bucket.Keys | Sort-Object
    foreach ($k in $keys) { Write-Output ("  行 {0}000-{1}999 : {2} 次" -f $k, $k, $bucket[$k]) }
}

Write-Output ""
Write-Output "==== 谁 patch 了 StaminaHUD ===="
$mods = "d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data\Mods"
$hits = Select-String -Path (Join-Path $mods '*\*.dll') -Pattern 'StaminaHUD' -Encoding Default -List -ErrorAction SilentlyContinue
if ($hits) { $hits | ForEach-Object { Write-Output $_.Path } } else { Write-Output '（Mods 下没有 DLL 直接出现 StaminaHUD 字样）' }

Write-Output ""
Write-Output "==== TppMod / KillFeed 来源 ===="
Select-String -Path (Join-Path $mods '*\*.dll') -Pattern 'TppMod' -Encoding Default -List -ErrorAction SilentlyContinue | ForEach-Object { Write-Output $_.Path }
Select-String -Path (Join-Path $mods '*\*.dll') -Pattern 'KillFeed' -Encoding Default -List -ErrorAction SilentlyContinue | ForEach-Object { Write-Output $_.Path }

Write-Output ""
Write-Output "==== 已安装 mod 列表 ===="
Get-ChildItem $mods -Directory | ForEach-Object { Write-Output $_.Name }
