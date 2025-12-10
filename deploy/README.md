# Logbert Deployment Scripts

This directory contains scripts for building and packaging Logbert for distribution across multiple platforms.

## Prerequisites

- .NET 10.0 SDK or later
- Platform-specific tools (see below)

## Directory Structure

```
deploy/
├── scripts/           # Cross-platform build scripts
│   ├── publish-all.sh     # Bash script for all platforms
│   └── publish-all.ps1    # PowerShell script for all platforms
├── windows/           # Windows-specific packaging
│   └── package-windows.ps1
├── linux/             # Linux-specific packaging
│   ├── package-linux.sh
│   └── AppImageBuilder.yml
└── macos/             # macOS-specific packaging
    └── package-macos.sh
```

## Quick Start

### Build for All Platforms

**On Linux/macOS:**
```bash
cd deploy/scripts
./publish-all.sh
```

**On Windows (PowerShell):**
```powershell
cd deploy\scripts
.\publish-all.ps1
```

### Build for Specific Platforms

```bash
# Windows only
./publish-all.sh -w

# Linux only
./publish-all.sh -l

# macOS only
./publish-all.sh -m

# Combine flags
./publish-all.sh -w -l
```

## Platform-Specific Packaging

After running the publish script, use the platform-specific packaging scripts:

### Windows

Creates ZIP archives for distribution:

```powershell
cd deploy\windows
.\package-windows.ps1 --version 2.0.0
```

**Output:**
- `packages/Logbert-2.0.0-win-x64.zip`
- `packages/Logbert-2.0.0-win-arm64.zip`

### Linux

Creates tar.gz archives and .deb packages:

```bash
cd deploy/linux
./package-linux.sh --version 2.0.0
```

**Output:**
- `packages/Logbert-2.0.0-linux-x64.tar.gz`
- `packages/Logbert-2.0.0-linux-arm64.tar.gz`
- `packages/Logbert-2.0.0-linux-x64.deb` (if dpkg-deb available)
- `packages/Logbert-2.0.0-linux-arm64.deb` (if dpkg-deb available)

**For AppImage (requires appimage-builder):**
```bash
cd deploy/linux
appimage-builder --recipe AppImageBuilder.yml
```

### macOS

Creates .app bundle and .dmg installer:

```bash
cd deploy/macos
./package-macos.sh --version 2.0.0
```

**Options:**
- `--app` - Create .app bundle only
- `--dmg` - Create .dmg installer only
- `--sign IDENTITY` - Code sign with specified identity

**Output:**
- `packages/Logbert.app`
- `packages/Logbert-2.0.0-macos-arm64.dmg` (on macOS)
- `packages/Logbert-2.0.0-macos-arm64.tar.gz` (fallback on non-macOS)

## Supported Architectures

| Platform | Architectures |
|----------|---------------|
| Windows  | x64, ARM64    |
| Linux    | x64, ARM64    |
| macOS    | ARM64 (Apple Silicon) |

## Build Output

All publish output goes to the `publish/` directory in the repository root:

```
publish/
├── win-x64/
├── win-arm64/
├── linux-x64/
├── linux-arm64/
└── osx-arm64/
```

Packaged distributions go to the `packages/` directory:

```
packages/
├── Logbert-2.0.0-win-x64.zip
├── Logbert-2.0.0-win-arm64.zip
├── Logbert-2.0.0-linux-x64.tar.gz
├── Logbert-2.0.0-linux-arm64.tar.gz
├── Logbert-2.0.0-linux-x64.deb
├── Logbert-2.0.0-linux-arm64.deb
├── Logbert.app/
└── Logbert-2.0.0-macos-arm64.dmg
```

## Version Numbers

Set the version using the `--version` flag or `VERSION` environment variable:

```bash
VERSION=2.1.0 ./publish-all.sh
# or
./publish-all.sh --version 2.1.0
```

## Troubleshooting

### Linux .deb Creation Fails
Install `dpkg-deb`:
```bash
sudo apt-get install dpkg
```

### macOS .dmg Creation Fails
DMG creation requires `hdiutil`, which is only available on macOS. On other platforms, a tar.gz archive is created instead.

### AppImage Creation Fails
Install appimage-builder:
```bash
pip3 install appimage-builder
```

## Notes

- All builds are self-contained (no .NET runtime required on target machine)
- Single-file deployment is enabled for cleaner distribution
- macOS builds target Apple Silicon (ARM64) only
- For Intel Mac support, modify the scripts to include `osx-x64` RID
