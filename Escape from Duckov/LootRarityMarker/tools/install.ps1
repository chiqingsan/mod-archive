param(
    [string]$Dist = (Join-Path $PSScriptRoot '..\dist'),
    [string]$GameRoot = 'd:\app\steam\steamapps\common\Escape from Duckov',
    [string]$BuildDll = (Join-Path $PSScriptRoot '..\src\bin\Release\LootRarityMarker.dll'),
    [switch]$SkipVerify
)

# 把 dist/ 安装到游戏的 Duckov_Data\Mods\<name>\，并复现游戏 ModManager 的校验规则。
$ErrorActionPreference = 'Stop'

$dist = (Resolve-Path $Dist).Path
$modName = 'LootRarityMarker'
$modDllName = $modName + '.dll'

# ---- 先把刚编译出来的 dll 同步进 dist/，避免装到旧版本 ----
if ($BuildDll -and (Test-Path $BuildDll)) {
    $distDll = Join-Path $dist $modDllName
    $buildTime = (Get-Item $BuildDll).LastWriteTime
    $distTime = if (Test-Path $distDll) { (Get-Item $distDll).LastWriteTime } else { [datetime]::MinValue }
    if ($buildTime -ne $distTime) {
        Copy-Item $BuildDll $distDll -Force
        Write-Output "已同步新编译的 dll 到 dist/ ($($buildTime.ToString('yyyy-MM-dd HH:mm:ss')))"
    }
}

$modsRoot = Join-Path $GameRoot 'Duckov_Data\Mods'
$managedDir = Join-Path $GameRoot 'Duckov_Data\Managed'
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
if ($name -ne $modName) { throw "info.ini 的 name ($name) 与预期 ($modName) 不一致" }

$expectDll = Join-Path $dist $modDllName
if (-not (Test-Path $expectDll)) { throw "DLL 缺失，游戏会期望: $expectDll" }

# ---- 安装前先验加载契约 ----
if (-not $SkipVerify) {
    Write-Output '--- verify-contract ---'
    & (Join-Path $PSScriptRoot 'verify-contract.ps1') -Dll $expectDll -ManagedDir $managedDir -ModName $modName
    Write-Output '-----------------------'
}

# ---- 备份目录放在 Mods 之外 ----
# 为什么不能留在 Mods 里：ModManager.Rescan() 是 Directory.GetDirectories(Mods)，
# 而备份目录里带着一份 name 完全相同的 info.ini。ScanAndActivateMods() 里是
# `if (!activeMods.ContainsKey(modInfo.name))` —— 同名只激活第一个，谁的目录先被枚举到谁生效。
# 也就是说，留在 Mods 里的备份有可能反过来把旧版本顶上去。所以备份一律移到 Mods 的兄弟目录。
$backupRoot = Join-Path (Split-Path $modsRoot -Parent) 'ModBackups'

# 顺手把以前留在 Mods 里的同名备份搬走（老版本 install.ps1 的产物）
$stale = Get-ChildItem $modsRoot -Directory -Filter "$name.bak-*" -ErrorAction SilentlyContinue
foreach ($s in $stale) {
    if ($s.Parent.FullName -ne (Resolve-Path $modsRoot).Path) { continue }
    if (-not $s.Name.StartsWith("$name.bak-")) { continue }
    New-Item -ItemType Directory -Force -Path $backupRoot | Out-Null
    Write-Output "把遗留在 Mods 里的备份移出扫描范围: $($s.Name) -> $backupRoot"
    Move-Item $s.FullName (Join-Path $backupRoot $s.Name) -Force
}

# ---- 复制 ----
$target = Join-Path $modsRoot $name
if (Test-Path $target) {
    $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    New-Item -ItemType Directory -Force -Path $backupRoot | Out-Null
    $backup = Join-Path $backupRoot ("$name.bak-$stamp")
    Write-Output "已存在同名目录，先备份到: $backup"
    Move-Item $target $backup
}
New-Item -ItemType Directory -Force -Path $target | Out-Null
Copy-Item (Join-Path $dist '*') $target -Recurse -Force

# ---- 安装后校验 ----
$ok = $true
$dll = Join-Path $target $modDllName
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
