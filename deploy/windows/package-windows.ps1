# Logbert Windows Packaging Script
# Creates ZIP packages for Windows x64 and ARM64

param(
    [string]$Version = "2.0.0",
    [string]$PublishDir = "",
    [switch]$Help
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = (Get-Item "$ScriptDir/../..").FullName

if ([string]::IsNullOrEmpty($PublishDir)) {
    $PublishDir = Join-Path $RootDir "publish"
}

$PackageDir = Join-Path $RootDir "packages"

function Show-Help {
    Write-Host @"
Logbert Windows Packaging Script

Usage: .\package-windows.ps1 [options]

Options:
  -Version      Version number for the package (default: 2.0.0)
  -PublishDir   Path to the publish directory (default: ../../publish)
  -Help         Show this help message

Prerequisites:
  Run publish-all.ps1 first to build the executables.
"@
}

function Create-ZipPackage {
    param(
        [string]$RID,
        [string]$Architecture
    )

    $SourceDir = Join-Path $PublishDir $RID

    if (-not (Test-Path $SourceDir)) {
        Write-Host "Source directory not found: $SourceDir" -ForegroundColor Yellow
        Write-Host "Run publish-all.ps1 -Windows first" -ForegroundColor Yellow
        return
    }

    $ZipName = "Logbert-$Version-windows-$Architecture.zip"
    $ZipPath = Join-Path $PackageDir $ZipName

    Write-Host "Creating $ZipName..." -ForegroundColor Cyan

    # Create a temporary directory with proper structure
    $TempDir = Join-Path $env:TEMP "logbert-package-$RID"
    if (Test-Path $TempDir) {
        Remove-Item -Recurse -Force $TempDir
    }

    $AppDir = Join-Path $TempDir "Logbert"
    New-Item -ItemType Directory -Path $AppDir -Force | Out-Null

    # Copy files
    Copy-Item -Path "$SourceDir\*" -Destination $AppDir -Recurse

    # Add README
    $ReadmeContent = @"
Logbert $Version for Windows $Architecture

INSTALLATION
============
1. Extract this ZIP to a folder of your choice
2. Run Logbert.exe

REQUIREMENTS
============
No additional requirements - this is a self-contained application.

DOCUMENTATION
=============
https://github.com/logbert/logbert/wiki

LICENSE
=======
MIT License - See LICENSE.txt for details
"@
    $ReadmeContent | Out-File -FilePath (Join-Path $AppDir "README.txt") -Encoding UTF8

    # Create ZIP
    if (Test-Path $ZipPath) {
        Remove-Item $ZipPath
    }

    Compress-Archive -Path "$TempDir\*" -DestinationPath $ZipPath -CompressionLevel Optimal

    # Cleanup
    Remove-Item -Recurse -Force $TempDir

    Write-Host "Created: $ZipPath" -ForegroundColor Green

    # Show file size
    $FileSize = (Get-Item $ZipPath).Length / 1MB
    Write-Host "Size: $([math]::Round($FileSize, 2)) MB" -ForegroundColor Gray
}

# Main script
if ($Help) {
    Show-Help
    exit 0
}

Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "Logbert Windows Packaging Script" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host ""

# Create packages directory
if (-not (Test-Path $PackageDir)) {
    New-Item -ItemType Directory -Path $PackageDir -Force | Out-Null
}

# Create packages for each architecture
Create-ZipPackage -RID "win-x64" -Architecture "x64"
Create-ZipPackage -RID "win-arm64" -Architecture "arm64"

Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host "Packaging Complete!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Packages created in: $PackageDir"
Get-ChildItem $PackageDir -Filter "*.zip" | Where-Object { $_.Name -like "*windows*" } | Format-Table Name, Length
