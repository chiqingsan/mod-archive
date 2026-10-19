$ErrorActionPreference = 'Stop'
$p = "$env:USERPROFILE\AppData\LocalLow\TeamSoda\Duckov\Player-prev.log"
$lines = Get-Content $p -Encoding UTF8
$start = [Math]::Max(0, $lines.Count - 120)
Write-Output "--- Player-prev.log 末尾 120 行 ---"
for ($i = $start; $i -lt $lines.Count; $i++) { Write-Output ("{0}: {1}" -f ($i + 1), $lines[$i]) }
