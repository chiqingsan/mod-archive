$ErrorActionPreference = 'Stop'
$d = "$env:USERPROFILE\AppData\LocalLow\TeamSoda\Duckov"

foreach ($name in @('Player-prev.log')) {
    $p = Join-Path $d $name
    if (-not (Test-Path $p)) { Write-Output ("缺文件: " + $p); continue }
    $lines = Get-Content $p -Encoding UTF8
    Write-Output ("===== " + $name + "  总行数 " + $lines.Count + " =====")
    Write-Output "--- 末尾 20 行 ---"
    $start = [Math]::Max(0, $lines.Count - 20)
    for ($i = $start; $i -lt $lines.Count; $i++) { Write-Output ("{0}: {1}" -f ($i + 1), $lines[$i]) }

    Write-Output "--- 该会话里 LootRarityMarker 启动行 ---"
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match 'LootRarityMarker.*(开始工作|不可用|已接上)') { Write-Output ("{0}: {1}" -f ($i + 1), $lines[$i]) }
    }
}
