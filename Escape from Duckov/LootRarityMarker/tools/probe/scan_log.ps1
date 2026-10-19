$ErrorActionPreference = 'Stop'
$log = "$env:USERPROFILE\AppData\LocalLow\TeamSoda\Duckov\Player.log"
$lines = Get-Content $log -Encoding UTF8
Write-Output ("总行数: " + $lines.Count)

Write-Output "==== 关键行号 ===="
$hits = @()
for ($i = 0; $i -lt $lines.Count; $i++) {
    $l = $lines[$i]
    if ($l -match '死亡|躯壳|Respawn|Exception|卸载|Unloading|LevelBegin|LevelInitialized|MainCharacterDead|OnEvacuat') {
        $hits += ("{0}: {1}" -f ($i + 1), $l)
    }
}
$hits | Select-Object -Last 50 | ForEach-Object { Write-Output $_ }

Write-Output ""
Write-Output "==== 末尾 25 行（带行号）===="
$start = [Math]::Max(0, $lines.Count - 25)
for ($i = $start; $i -lt $lines.Count; $i++) {
    Write-Output ("{0}: {1}" -f ($i + 1), $lines[$i])
}
