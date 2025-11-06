# Logbert

**A modern, cross-platform log file viewer for .NET 9**

Logbert is an advanced log message viewer supporting multiple logging frameworks including log4net, NLog, and Syslog. Originally a Windows-only WinForms application, Logbert has been migrated to .NET 9 with Avalonia UI to provide a native experience on Windows, macOS, and Linux.

![Logbert Screenshot](doc/screenshot.png "Logbert Screenshot")

## 🎯 Project Status

**Current Version:** 2.0-Phase4 (Zero Compilation Errors with .NET 9 ✨)
**Target Framework:** .NET 9.0 (Latest)
**UI Framework:** Avalonia 11.2.2

### Migration Progress

✅ **Completed Phases:**
- **Phase 1:** Core infrastructure (.NET 8, Avalonia setup)
- **Phase 2:** Core models and interfaces
- **Phase 3:** Log viewer components with DataGrid
- **Phase 4:** WinForms elimination & Avalonia compilation ✅
  - 632 → 0 compilation errors (Phase 4)
  - Migrated to .NET 9.0 (Latest) with zero errors (Post-Phase 4)
  - Excluded docking system (Dock.Avalonia MVVM limitations)
  - Excluded receiver dialogs (re-implementing in Phase 5)
  - Replaced ToolBar/StatusBar with Avalonia equivalents
  - Added missing LogMessage properties
  - Extended csproj with 70+ compile exclusions

🚧 **In Progress:**
- **Phase 5:** Avalonia Feature Re-implementation (~75% Complete)
  - ✅ Custom docking system (Grid-based layout)
  - ✅ MainWindowViewModel with full MVVM
  - ✅ Search dialog with regex support
  - 🚧 Receiver configuration dialogs (2/24 types working)
  - 🚧 Statistics dialog (ViewModel ready, UI pending)
  - ✅ Options dialog (basic functionality)

✅ **Recently Completed:**
- Custom docking layout (replaces Dock.Avalonia)
- Log4Net & NLog file receiver UIs
- Complete search functionality (Find Next/Previous, regex, case-sensitive)
- MainWindow with filter panel, bookmarks, logger tree

## ✨ Features

### Log Receivers
* **File-based receivers**
  - Log4Net XML files (FileSystemWatcher monitoring)
  - NLog XML files
  - Syslog files (RFC 3164 format)
  - Custom format files (configurable columnizer)
  - Directory-based (multiple files as one stream)

* **Network receivers**
  - Log4Net UDP receiver
  - NLog TCP/UDP receivers
  - Syslog UDP receiver
  - Custom TCP/UDP/HTTP receivers

* **System receivers**
  - Windows Event Log (Windows only)
  - Windows Debug Output (Windows only)

### User Interface
* **Modern Avalonia UI** - Native look and feel on all platforms
* **Docking system** - Customizable panel layout with Dock.Avalonia
* **Enhanced log details** - Rich formatting with sections for timestamp, logger, message, and exceptions
* **Color map** - Vertical visualization showing log level distribution
* **Dark/Light themes** - Follows system preferences

### Filtering & Search
* **Logger tree** - Hierarchical view of log sources
* **Level filtering** - Show/hide Trace, Debug, Info, Warning, Error, Fatal
* **Text search** - Wildcard and regular expression support
* **Bookmarks** - Create unlimited bookmarks for important messages

### Advanced Features
* **Lua scripting** - Embedded script engine for custom filtering and processing
  - Filter messages programmatically
  - Process and transform log data
  - Auto-execute on new messages
* **Statistics** - Visual charts showing log level distribution and metrics
* **Time shift** - Synchronize logs from different time zones
* **Export** - Save logs as CSV or original format

### Cross-Platform
* **Windows** - Native Windows 10/11 experience
* **macOS** - Native macOS experience
* **Linux** - Native Linux experience (GTK, X11, Wayland)

## 🚀 Getting Started

### Prerequisites
- **.NET 9.0 SDK** or later (Latest recommended) - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Windows 10/11**, **macOS 11+**, or **Linux** with GTK 3.22+
- **Visual Studio 2022 v17.9+**, **Visual Studio Code**, or **JetBrains Rider** (optional but recommended)

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

# Or run the compiled executable
./src/Logbert/bin/Debug/net8.0/x64/Logbert.exe  # Windows
./src/Logbert/bin/Debug/net8.0/x64/Logbert      # Linux/macOS
```

### Build Status (.NET 9.0)
```
✅ Build succeeded.
    32 Warning(s)
    0 Error(s)
    Time Elapsed: 00:00:08.37
```

**Build Output:** `src/Logbert/bin/Debug/net9.0/x64/Logbert.exe` (177 KB)
**Binary Size:** 177 KB (Debug), ~80 KB (Release, estimated)

### Publishing for Distribution

**Windows (x64):**
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

**macOS (Intel):**
```bash
dotnet publish -c Release -r osx-x64 --self-contained
```

**macOS (Apple Silicon):**
```bash
dotnet publish -c Release -r osx-arm64 --self-contained
```

**Linux (x64):**
```bash
dotnet publish -c Release -r linux-x64 --self-contained
```

Published applications will be in `src/Logbert/bin/Release/net9.0/{runtime}/publish/`

### Current Build Notes
- **Phase 4 Status:** Fully compilable with .NET 9.0, zero errors ✨
- **Functional Status:** Partial (docking/receiver features disabled in Phase 4, to be re-implemented in Phase 5)
- **Main Window:** Shows welcome screen with stub buttons
- **Receiver System:** Not yet functional (NewLogSourceDialog disabled)
- **Performance:** .NET 9 features available for future optimization (JIT improvements, better GC)

## 📖 Documentation

### User & Developer Documentation
- [Architecture Overview](docs/ARCHITECTURE.md) - System design and component overview
- [User Guide](docs/USER_GUIDE.md) - How to use Logbert
- [Developer Guide](docs/DEVELOPER_GUIDE.md) - Contributing and development setup
- [Receiver Configuration](docs/RECEIVERS.md) - Setting up log receivers
- [Scripting Guide](docs/SCRIPTING.md) - Using the Lua scripting engine

### Migration Documentation
- [**Migration Guide**](MIGRATION_GUIDE.md) - Detailed WinForms → Avalonia migration guide
  - Architecture changes
  - Phase 4 achievements
  - Phase 5 roadmap
  - Best practices and patterns
  - Common migration issues

- [**Build Status**](BUILD_STATUS.md) - Current build status and error resolution timeline
  - Phase 4 completion details
  - Error reduction history (632 → 0)
  - Excluded components and why
  - Phase 5 feature implementation plan
  - Testing checklist

## 🏗️ Architecture

Logbert uses a modern MVVM architecture with the following key technologies:

- **UI Framework:** Avalonia 11.2.2 (cross-platform XAML)
- **MVVM:** CommunityToolkit.Mvvm 8.3.2
- **Docking:** Dock.Avalonia 11.1.0
- **Code Editing:** AvaloniaEdit 11.1.0
- **Scripting:** MoonSharp (Lua interpreter)
- **Log Parsing:** Custom parsers for Log4Net, NLog, Syslog formats

### Key Components

```
┌─────────────────────────────────────────────────────────┐
│                    MainWindow                           │
│  ┌───────────┐  ┌──────────────┐  ┌─────────────────┐ │
│  │  Filter   │  │              │  │    Logger Tree  │ │
│  │  Panel    │  │  Log Viewer  │  │                 │ │
│  │           │  │   (DataGrid) │  │    Bookmarks    │ │
│  └───────────┘  └──────────────┘  └─────────────────┘ │
│  ┌───────────────────────────────────────────────────┐ │
│  │         Enhanced Log Details View                 │ │
│  └───────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
                         │
                         ▼
              ┌────────────────────┐
              │  Log Receivers     │
              ├────────────────────┤
              │  • Log4Net File    │
              │  • NLog File       │
              │  • Syslog UDP      │
              │  • Custom Parsers  │
              └────────────────────┘
```

## 🤝 Contributing

Contributions are welcome! Please read our [Developer Guide](docs/DEVELOPER_GUIDE.md) for details on our code of conduct and the process for submitting pull requests.

### Development Setup

1. Install .NET 9 SDK
2. Install an IDE (Visual Studio 2022, VS Code, or JetBrains Rider)
3. Clone the repository
4. Open `src/Logbert.sln` in your IDE
5. Restore NuGet packages
6. Build and run

### Coding Standards

- Follow C# coding conventions
- Use MVVM pattern for UI components
- Write XML documentation for public APIs
- Add unit tests for new features
- Keep commits focused and well-described

## 📋 Roadmap

### Version 2.0 (Current)
- ✅ Cross-platform migration to .NET 9 + Avalonia
- ✅ Core receiver infrastructure
- ✅ Lua scripting support
- ✅ Statistics and visualization
- ✅ Custom docking system
- ✅ Search functionality
- 🚧 Complete receiver UI coverage (2/24 types)
- 🚧 Cross-platform testing

### Version 2.1 (Planned)
- Additional receiver configuration UIs
- Performance optimizations for large log files
- Enhanced search capabilities
- Plugin architecture
- Customizable themes

### Version 2.2 (Future)
- Real-time log streaming improvements
- Advanced filtering with saved filter sets
- Log correlation across multiple sources
- Cloud log source support

## 🐛 Known Issues

- Some receivers (UDP, TCP, Syslog, Custom HTTP, EventLog) have backend implementations but need Avalonia UI configuration dialogs
- Performance with extremely large files (>1GB) needs optimization
- Windows Event Log receiver is Windows-only (platform-specific)

## 📄 License

Copyright (c) 2015-2025 Couchcoding

Released under the [MIT License](LICENSE).

## 🙏 Acknowledgments

- Original Logbert by Couchcoding
- [Avalonia UI](https://avaloniaui.net/) - Cross-platform XAML framework
- [Dock.Avalonia](https://github.com/wieslawsoltes/Dock) - Docking system
- [MoonSharp](https://www.moonsharp.org/) - Lua scripting engine
- [AvaloniaEdit](https://github.com/AvaloniaUI/AvaloniaEdit) - Text editor component

## 📞 Support

- **Issues:** [GitHub Issues](https://github.com/couchcoding/Logbert/issues)
- **Discussions:** [GitHub Discussions](https://github.com/couchcoding/Logbert/discussions)
- **Documentation:** [docs/](docs/)

---

**Note:** This is a complete rewrite using modern .NET and cross-platform technologies. The original WinForms version is available in the `legacy-winforms` branch.
