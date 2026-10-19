$ErrorActionPreference = 'Stop'
$log = "$env:USERPROFILE\AppData\LocalLow\TeamSoda\Duckov\Player.log"
$lines = Get-Content $log -Encoding UTF8

function Show-Range($from, $to, $title) {
    Write-Output ""
    Write-Output "===== $title  ($from..$to) ====="
    for ($i = $from - 1; $i -lt [Math]::Min($to, $lines.Count); $i++) {
        Write-Output ("{0}: {1}" -f ($i + 1), $lines[$i])
    }
}

Show-Range 11140 11221 "死亡前后"
Show-Range 8560 8600 "第一次死亡 NRE 栈"
Show-Range 10840 10880 "后续死亡 NRE 栈"
