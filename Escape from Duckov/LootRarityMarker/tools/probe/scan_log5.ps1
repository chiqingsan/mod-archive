$ErrorActionPreference = 'Stop'
$log = "$env:USERPROFILE\AppData\LocalLow\TeamSoda\Duckov\Player.log"
$lines = Get-Content $log -Encoding UTF8

Write-Output "==== KillFeed.OnDead NRE 上下文 ===="
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i].Contains('KillFeed.ModBehaviour.OnDead')) {
        $from = [Math]::Max(0, $i - 25)
        $to = [Math]::Min($lines.Count - 1, $i + 15)
        for ($j = $from; $j -le $to; $j++) { Write-Output ("{0}: {1}" -f ($j + 1), $lines[$j]) }
        break
    }
}

Write-Output ""
Write-Output "==== 崩溃相关 ===="
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match 'Crash|crash!!|StackOverflow|OutOfMemory|Fatal|abort') {
        Write-Output ("{0}: {1}" -f ($i + 1), $lines[$i])
    }
}

Write-Output ""
Write-Output "==== 目录里有没有崩溃转储 ===="
$d = "$env:USERPROFILE\AppData\LocalLow\TeamSoda\Duckov"
Get-ChildItem $d -Recurse -File | Where-Object { $_.Name -match 'crash|dump|error' } | Select-Object FullName, Length | ForEach-Object { Write-Output ($_.FullName + '  ' + $_.Length) }
