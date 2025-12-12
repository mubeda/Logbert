# Logbert

**A modern, cross-platform log file viewer built with .NET 10 and Avalonia UI**

Logbert is an advanced log message viewer supporting multiple logging frameworks including Log4Net, NLog, and Syslog. Originally a Windows-only WinForms application, Logbert has been completely rewritten for .NET 10 with Avalonia UI to provide a native experience on Windows, macOS, and Linux.

![Logbert Screenshot](doc/screenshot.png "Logbert Screenshot")

## Features

### Log Receivers (16 Types - All Implemented)

**File-based receivers:**
- Log4Net XML files with FileSystemWatcher monitoring
- Log4Net directory monitoring (multiple files as one stream)
- NLog XML files
- NLog directory monitoring
- Syslog files (RFC 3164 format)
- Custom format files with configurable columnizer

**Network receivers:**
- Log4Net UDP receiver
- NLog TCP receiver
- NLog UDP receiver
- Syslog UDP receiver
- Custom TCP/UDP/HTTP receivers

**System receivers (Windows only):**
- Windows Event Log
- Windows Debug Output

### User Interface
- **Modern Avalonia UI** - Native look and feel on all platforms
- **Custom docking layout** - Filter panel, logger tree, bookmarks panel
- **Enhanced log details** - Rich formatting with sections for timestamp, logger, message, and exceptions
- **Color map** - Visual log level distribution bar
- **Fluent theme** - Dark/Light modes following system preferences

### Filtering & Search
- **Logger tree** - Hierarchical view of log sources
- **Level filtering** - Show/hide Trace, Debug, Info, Warning, Error, Fatal
- **Advanced search** - Case-sensitive, whole word, and regular expression support
- **Search history** - Quick access to recent searches
- **Match counter** - Shows current match position (e.g., "Match 3 of 15")
- **Bookmarks** - Create unlimited bookmarks for important messages

### Advanced Features
- **Lua scripting** - MoonSharp embedded script engine for custom filtering and processing
- **Statistics dialog** - Visual charts showing log level distribution and metrics
- **Time shift** - Synchronize logs from different time zones
- **Export** - Save logs as CSV or original format

### Cross-Platform Support
- **Windows** - Windows 10/11 (x64, ARM64)
- **macOS** - macOS 12+ (Intel x64, Apple Silicon ARM64)
- **Linux** - GTK-based distributions (x64, ARM64)

## Getting Started

### Prerequisites
- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Windows 10+**, **macOS 12+**, or **Linux** with GTK 3.22+

### Building from Source

```bash
# Clone the repository
git clone https://github.com/couchcoding/Logbert.git
cd Logbert

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project src/Logbert/Logbert.csproj
```

### Build Status
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Publishing for Distribution

```bash
# Windows (x64)
dotnet publish -c Release -r win-x64 --self-contained

# Windows (ARM64)
dotnet publish -c Release -r win-arm64 --self-contained

# macOS (Intel)
dotnet publish -c Release -r osx-x64 --self-contained

# macOS (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained

# Linux (x64)
dotnet publish -c Release -r linux-x64 --self-contained

# Linux (ARM64)
dotnet publish -c Release -r linux-arm64 --self-contained
```

Published applications will be in `src/Logbert/bin/Release/net10.0/{runtime}/publish/`

## Architecture

Logbert uses a modern MVVM architecture:

| Component | Technology | Version |
|-----------|------------|---------|
| Runtime | .NET | 10.0 |
| UI Framework | Avalonia | 11.3.8 |
| MVVM | CommunityToolkit.Mvvm | 8.4.0 |
| Docking | Custom Grid-based layout | - |
| Code Editor | AvaloniaEdit | 11.3.0 |
| Scripting | MoonSharp (Lua) | - |

### Key Components

```
MainWindow
  +-- Filter Panel (Log level filtering)
  +-- Log Viewer (DataGrid with virtualization)
  +-- Logger Tree (Hierarchical namespace view)
  +-- Bookmarks Panel
  +-- Details Panel (Rich log message view)
  +-- Color Map (Visual level distribution)

Receivers (16 types)
  +-- File-based (6): Log4Net, NLog, Syslog, Custom
  +-- Network (7): UDP, TCP, HTTP protocols
  +-- System (2): Windows Event Log, Debug Output
  +-- Custom (1): Configurable regex-based parser
```

## Documentation

Detailed documentation is available in the [docs/](docs/) directory:

- [Migration Status](docs/AVALONIA_MIGRATION_STATUS.md) - Project status, migration progress, and Phase 6 roadmap
- [Testing Checklist](docs/TESTING_CHECKLIST.md) - Comprehensive test procedures
- [User Guide](docs/USER_GUIDE.md) - How to use Logbert
- [Developer Guide](docs/DEVELOPER_GUIDE.md) - Contributing and development
- [Receivers](docs/RECEIVERS.md) - Receiver configuration details
- [Scripting](docs/SCRIPTING.md) - Lua API reference

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Make your changes following the existing code style
4. Submit a pull request

### Development Setup

1. Install .NET 10 SDK
2. Clone the repository
3. Open `src/Logbert.sln` in your IDE
4. Build and run

## License

Copyright (c) 2015-2025 Couchcoding

Released under the [MIT License](LICENSE).

## Acknowledgments

- Original Logbert by Couchcoding
- [Avalonia UI](https://avaloniaui.net/) - Cross-platform XAML framework
- [MoonSharp](https://www.moonsharp.org/) - Lua scripting engine
- [AvaloniaEdit](https://github.com/AvaloniaUI/AvaloniaEdit) - Text editor component
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM library

---

**Note:** This is a complete rewrite using modern .NET 10 and cross-platform technologies. The original WinForms version is available in the `legacy-winforms` branch.
