# Logbert v2.0 Release Notes

**Release Date:** December 2025
**Target Framework:** .NET 10.0
**UI Framework:** Avalonia 11.3.8

---

## Overview

Logbert v2.0 is a major release that brings cross-platform support through a complete migration from Windows Forms to Avalonia UI. This release maintains full feature parity with v1.x while adding new capabilities and improving the overall user experience.

---

## What's New

### Cross-Platform Support

Logbert now runs natively on:
- **Windows** (x64, ARM64)
- **macOS** (Intel x64, Apple Silicon ARM64)
- **Linux** (x64, ARM64)

All platforms share the same codebase and feature set, with platform-specific receivers appropriately handled.

### Modern UI Framework

- Migrated from Windows Forms to **Avalonia UI 11.3.8**
- Fluent Design theme with Light/Dark mode support
- System theme following (auto Light/Dark)
- Improved high-DPI support
- Better font rendering

### New Export Functionality

- Export log messages to **CSV** or **Plain Text** format
- Export scope: All messages or filtered messages only
- Multiple encoding options (UTF-8, UTF-16, ASCII, Latin-1)
- Progress indicator with cancellation support
- Keyboard shortcut: `Ctrl+E`

### Enhanced Error Handling

- New centralized **NotificationService** for consistent error dialogs
- User-friendly validation messages in receiver configuration
- Expandable error details for debugging
- Confirmation dialogs for destructive actions

### Settings Persistence

- JSON-based settings storage (cross-platform)
- Window position and size remembered
- Panel layout preservation
- Column width persistence
- Recent files (MRU) tracking

### Recent Files Menu

- Quick access to last 9 opened log files
- Automatic cleanup of non-existent files
- Integrated with File menu

---

## All 16 Receiver Types

### File-Based Receivers (6)
| Receiver | Description |
|----------|-------------|
| Log4Net File | XML files with FileSystemWatcher |
| Log4Net Directory | Multiple files as one stream |
| NLog File | XML files |
| NLog Directory | Multiple files monitoring |
| Syslog File | RFC 3164 format |
| Custom File | Configurable columnizer/regex |

### Network Receivers (7)
| Receiver | Description |
|----------|-------------|
| Log4Net UDP | UDP network logging |
| NLog TCP | TCP network logging |
| NLog UDP | UDP network logging |
| Syslog UDP | Standard syslog over UDP |
| Custom TCP | TCP with custom parsing |
| Custom UDP | UDP with custom parsing |
| Custom HTTP | HTTP POST endpoint |

### System Receivers (2)
| Receiver | Platform | Description |
|----------|----------|-------------|
| Windows Event Log | Windows | System event logs |
| Windows Debug Output | Windows | OutputDebugString capture |

### Custom Receiver (1)
| Receiver | Description |
|----------|-------------|
| Custom Directory | Regex-based with Columnizer support |

---

## Core Features

### Log Viewing
- DataGrid with virtualization for large files
- Color-coded log levels
- Column sorting and resizing
- Zoom in/out (Ctrl++/Ctrl+-)

### Search & Navigation
- Full-text search with regex support
- Case-sensitive and whole-word options
- Search history
- Find next/previous (F3/Shift+F3)

### Filtering
- Log level toggle buttons (Trace/Debug/Info/Warning/Error/Fatal)
- Logger tree with hierarchical filtering
- Lua scripting for advanced filtering

### Bookmarks
- Mark important log messages
- Quick navigation via bookmarks panel
- Export bookmarks with logs

### Statistics
- Total message count
- Time range analysis
- Log level distribution with percentages
- Messages per second rate

### Lua Scripting
- MoonSharp Lua engine
- Filter and transform log messages
- Script editor with syntax highlighting
- Real-time script execution

### Color Map
- Visual overview of log file
- Color-coded by severity
- Click to navigate

---

## Breaking Changes

### Platform Requirements
- **Minimum .NET version:** .NET 10.0 (was .NET Framework 4.8)
- **Windows Event Log receiver:** Windows only
- **Windows Debug Output receiver:** Windows only

### Settings Migration
- Settings now stored in JSON format
- Previous v1.x settings are not automatically migrated
- Settings location:
  - Windows: `%AppData%\Logbert\settings.json`
  - macOS/Linux: `~/.config/Logbert/settings.json`

### Removed Dependencies
- Windows Forms
- GDI+ graphics
- System.Drawing (Windows-specific)

---

## Installation

### Prerequisites
- .NET 10.0 Runtime or SDK

### Windows
```powershell
# Download and extract ZIP
# Or install via winget (coming soon)
```

### macOS
```bash
# Download .dmg and drag to Applications
# Or extract .app bundle from ZIP
```

### Linux
```bash
# AppImage (portable)
chmod +x Logbert-2.0.0-x64.AppImage
./Logbert-2.0.0-x64.AppImage

# Or install .deb package (Debian/Ubuntu)
sudo dpkg -i logbert_2.0.0_amd64.deb
```

---

## Build from Source

```bash
# Clone repository
git clone https://github.com/couchcoding/logbert.git
cd logbert

# Build
dotnet build src/Logbert/Logbert.csproj

# Run
dotnet run --project src/Logbert/Logbert.csproj

# Publish (self-contained)
dotnet publish -c Release -r win-x64 --self-contained
dotnet publish -c Release -r osx-arm64 --self-contained
dotnet publish -c Release -r linux-x64 --self-contained
```

---

## Known Issues

1. **Code signing not yet configured** - macOS Gatekeeper may require manual approval
2. **First launch may be slow** - JIT compilation on first run
3. **High memory usage with 1M+ messages** - Consider filtering to reduce memory

---

## Reporting Issues

Please report bugs and feature requests on GitHub:
- Issues: https://github.com/couchcoding/logbert/issues
- Discussions: https://github.com/couchcoding/logbert/discussions

---

## Credits

### Core Team
- Couchcoding (Original author and maintainer)

### Technologies
- [Avalonia UI](https://avaloniaui.net/) - Cross-platform UI framework
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) - MVVM pattern
- [AvaloniaEdit](https://github.com/AvaloniaUI/AvaloniaEdit) - Text editor control
- [MoonSharp](https://www.moonsharp.org/) - Lua scripting engine

### Contributors
Thank you to all contributors who helped with testing, documentation, and code improvements.

---

## Upgrade Path

### From v1.x (Windows Forms)
1. Install .NET 10.0 Runtime
2. Download Logbert v2.0
3. Re-configure receivers (settings are not migrated)
4. Verify log format compatibility

### From v2.0 Beta
1. Settings are compatible
2. Simply replace executable

---

## Future Roadmap

- [ ] Installer packages (MSI, PKG)
- [ ] Code signing for all platforms
- [ ] Performance optimizations for 10M+ messages
- [ ] Plugin architecture for custom receivers
- [ ] Cloud log source integration (AWS CloudWatch, Azure Monitor)
- [ ] Log correlation and tracing

---

## License

Logbert is licensed under the MIT License. See [LICENSE](../LICENSE) for details.
