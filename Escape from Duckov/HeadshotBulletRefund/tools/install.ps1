param(
    [string]$Dist = (Join-Path $PSScriptRoot '..\dist'),
    [string]$GameRoot = 'd:\app\steam\steamapps\common\Escape from Duckov',
    [string]$BuildDll = (Join-Path $PSScriptRoot '..\src\bin\Release\HeadshotBulletRefund.dll')
)

# 把 dist/ 安装到游戏的 Duckov_Data\Mods\<name>\，并复现游戏 ModManager 的校验规则。
$ErrorActionPreference = 'Stop'

$dist = (Resolve-Path $Dist).Path

# ---- 先把刚编译出来的 dll 同步进 dist/，避免装到旧版本 ----
if ($BuildDll -and (Test-Path $BuildDll)) {
    $buildTime = (Get-Item $BuildDll).LastWriteTime
    $distDll = Join-Path $dist 'HeadshotBulletRefund.dll'
    $distTime = if (Test-Path $distDll) { (Get-Item $distDll).LastWriteTime } else { [datetime]::MinValue }
    if ($buildTime -ne $distTime) {
        Copy-Item $BuildDll $distDll -Force
        Write-Output "已同步新编译的 dll 到 dist/ ($($buildTime.ToString('yyyy-MM-dd HH:mm:ss')))"
    }
}

$modsRoot = Join-Path $GameRoot 'Duckov_Data\Mods'
$infoFile = Join-Path $dist 'info.ini'
if (-not (Test-Path $infoFile)) { throw "缺少 info.ini: $infoFile" }
if (-not (Test-Path $modsRoot)) { throw "找不到 Mods 目录: $modsRoot" }

# ---- 复现 ModManager.TryProcessModFolder 的 info.ini 解析 ----
$dict = [ordered]@{}
foreach ($raw in [System.IO.File]::ReadAllLines($infoFile, [System.Text.Encoding]::UTF8)) {
    $line = $raw.Trim()
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    if ($line.StartsWith('[')) { continue }
    $i = $line.IndexOf('=')
    if ($i -ge 1 -and ($i + 1) -lt $line.Length) {
        $dict[$line.Substring(0, $i).Trim()] = $line.Substring($i + 1).Trim()
    }
}
if (-not $dict.Contains('name')) { throw "info.ini 缺少 name 键" }
$name = $dict['name']

$expectDll = Join-Path $dist ($name + '.dll')
if (-not (Test-Path $expectDll)) { throw "DLL 缺失，游戏会期望: $expectDll" }

# ---- 复制 ----
$target = Join-Path $modsRoot $name
if (Test-Path $target) {
    $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $backup = "$target.bak-$stamp"
    Write-Output "已存在同名目录，先备份到: $backup"
    Move-Item $target $backup
}
New-Item -ItemType Directory -Force -Path $target | Out-Null
Copy-Item (Join-Path $dist '*') $target -Recurse -Force

# ---- 安装后校验 ----
$ok = $true
$dll = Join-Path $target ($name + '.dll')
$ini = Join-Path $target 'info.ini'
$png = Join-Path $target 'preview.png'
$cfg = Join-Path $target 'config.json'
foreach ($f in @($dll, $ini, $png)) {
    $exists = Test-Path $f
    if (-not $exists) { $ok = $false }
    Write-Output ("  {0,-22} {1}" -f (Split-Path $f -Leaf), $(if ($exists) { 'OK' } else { 'MISSING' }))
}
# config.json 不是游戏要求的文件，缺了也能跑（用内置默认值）
Write-Output ("  {0,-22} {1}" -f 'config.json', $(if (Test-Path $cfg) { 'OK (可选)' } else { '缺失 (可选)' }))

Write-Output "安装目录 : $target"
Write-Output ("name     : {0}" -f $name)
Write-Output ("display  : {0}" -f $dict['displayName'])
Write-Output ("version  : {0}" -f $dict['version'])
Write-Output ("desc len : {0} chars" -f $dict['description'].Length)
Write-Output ("dll bytes: {0}" -f (Get-Item $dll).Length)
Write-Output ("INSTALL  : {0}" -f $(if ($ok) { 'PASS' } else { 'FAIL' }))
if (-not $ok) { exit 1 }
