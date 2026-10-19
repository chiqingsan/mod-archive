$ErrorActionPreference = 'Stop'
$log = "$env:USERPROFILE\AppData\LocalLow\TeamSoda\Duckov\Player.log"
$lines = Get-Content $log -Encoding UTF8

Write-Output ("总行数: " + $lines.Count)

$diag = 0
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -like '*LootRarityMarker][诊断]*') { $diag++ }
}
Write-Output ("LootRarityMarker 诊断行数: " + $diag)

Write-Output ""
Write-Output "==== 所有异常块（异常行 + 下一行栈顶）===="
$seen = @{}
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match '^(NullReferenceException|MissingReferenceException|ArgumentException|InvalidOperationException|KeyNotFoundException|IndexOutOfRangeException|Unhandled Exception|StackOverflowException|.*Exception:)') {
        $key = $lines[$i]
        if ($i + 1 -lt $lines.Count) { $key = $key + ' || ' + $lines[$i + 1].Trim() }
        if ($seen.ContainsKey($key)) { $seen[$key] = $seen[$key] + 1 } else { $seen[$key] = 1 }
    }
}
foreach ($k in $seen.Keys) { Write-Output ("[{0}x] {1}" -f $seen[$k], $k) }

Write-Output ""
Write-Output "==== 含 LootRarityMarker 的栈行 ===="
$m = 0
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match 'LootRarityMarker' -and $lines[$i] -notmatch '诊断' -and $lines[$i] -notmatch '参数 - key' -and $lines[$i] -notmatch '字段设置完成' -and $lines[$i] -notmatch '已成功添加下拉') {
        Write-Output ("{0}: {1}" -f ($i + 1), $lines[$i]); $m++
    }
}
Write-Output ("合计 " + $m + " 行")
