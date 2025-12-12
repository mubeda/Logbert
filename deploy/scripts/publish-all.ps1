# Logbert Cross-Platform Build Script (PowerShell)
# Builds self-contained executables for all supported platforms

param(
    [switch]$Windows,
    [switch]$Linux,
    [switch]$MacOS,
    [switch]$All,
    [string]$Version = "2.0.0",
    [switch]$Help
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = (Get-Item "$ScriptDir/../..").FullName
$ProjectPath = Join-Path $RootDir "src/Logbert/Logbert.csproj"
$OutputDir = Join-Path $RootDir "publish"

function Show-Help {
    Write-Host @"
Logbert Cross-Platform Build Script

Usage: .\publish-all.ps1 [options]

Options:
  -Windows    Build for Windows (x64 and ARM64)
  -Linux      Build for Linux (x64 and ARM64)
  -MacOS      Build for macOS (ARM64)
  -All        Build for all platforms (default if no platform specified)
  -Version    Set the version number (default: 2.0.0)
  -Help       Show this help message

Examples:
  .\publish-all.ps1 -All
  .\publish-all.ps1 -Windows -Linux
  .\publish-all.ps1 -Windows -Version "2.1.0"
"@
}

function Publish-Runtime {
    param(
        [string]$RID,
        [string]$FriendlyName
    )

    Write-Host ""
    Write-Host "Building for $FriendlyName ($RID)..." -ForegroundColor Cyan
    Write-Host "----------------------------------------"

    $TargetDir = Join-Path $OutputDir $RID

    & dotnet publish $ProjectPath `
        -c Release `
        -r $RID `
        --self-contained true `
        -p:PublishSingleFile=true `
        -p:PublishTrimmed=false `
        -p:Version=$Version `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -o $TargetDir

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed for $RID" -ForegroundColor Red
        exit 1
    }

    Write-Host "Built: $TargetDir" -ForegroundColor Green
}

# Main script
if ($Help) {
    Show-Help
    exit 0
}

Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "Logbert Build Script v$Version" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host ""

# If no platform specified, build all
if (-not ($Windows -or $Linux -or $MacOS -or $All)) {
    $All = $true
}

if ($All) {
    $Windows = $true
    $Linux = $true
    $MacOS = $true
}

# Clean previous builds
Write-Host "Cleaning previous builds..."
if (Test-Path $OutputDir) {
    Remove-Item -Recurse -Force $OutputDir
}
New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

# Build for selected platforms
if ($Windows) {
    Publish-Runtime -RID "win-x64" -FriendlyName "Windows x64"
    Publish-Runtime -RID "win-arm64" -FriendlyName "Windows ARM64"
}

if ($Linux) {
    Publish-Runtime -RID "linux-x64" -FriendlyName "Linux x64"
    Publish-Runtime -RID "linux-arm64" -FriendlyName "Linux ARM64"
}

if ($MacOS) {
    Publish-Runtime -RID "osx-arm64" -FriendlyName "macOS ARM64 (Apple Silicon)"
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host "Build Complete!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Output directory: $OutputDir"
Write-Host ""
Get-ChildItem $OutputDir | Format-Table Name, LastWriteTime
