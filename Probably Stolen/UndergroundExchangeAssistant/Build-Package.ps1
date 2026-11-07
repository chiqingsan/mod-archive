$ErrorActionPreference = 'Stop'
$projectDir = $PSScriptRoot
dotnet build (Join-Path $projectDir 'UndergroundExchangeAssistant.csproj') -c Release
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
$packageDir = Join-Path $projectDir 'package'
New-Item -ItemType Directory -Force -Path $packageDir | Out-Null
Copy-Item -LiteralPath (Join-Path $projectDir 'bin\Release\netstandard2.1\UndergroundExchangeAssistant.dll') -Destination $packageDir
Copy-Item -LiteralPath (Join-Path $projectDir 'manifest.xml'),(Join-Path $projectDir 'preview.png') -Destination $packageDir
Get-FileHash -Algorithm SHA256 -LiteralPath (Join-Path $packageDir 'UndergroundExchangeAssistant.dll'),(Join-Path $packageDir 'preview.png')
