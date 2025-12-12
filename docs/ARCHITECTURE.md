# Logbert Architecture Documentation

## Overview

Logbert is built using modern .NET technologies with a focus on cross-platform compatibility, maintainability, and extensibility. This document provides a comprehensive overview of the system architecture, design patterns, and key components.

## Technology Stack

### Core Technologies
- **.NET 10.0** - Cross-platform runtime (Latest)
- **C# 13** - Programming language with latest features
- **Avalonia UI 11.3.8** - Cross-platform XAML-based UI framework

### Key Dependencies
| Package | Version | Purpose |
|---------|---------|---------|
| Avalonia | 11.3.8 | UI framework |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM infrastructure with source generators |
| Dock.Avalonia | 11.3.6.5 | Docking panel system |
| AvaloniaEdit | 11.3.0 | Code editor control |
| MoonSharp | Latest | Lua scripting engine |

### Platform Support
- **Windows** 10/11 (x64, ARM64)
- **macOS** 11+ (x64, ARM64)
- **Linux** (x64, ARM64) - GTK, X11, Wayland

## Architectural Patterns

### MVVM (Model-View-ViewModel)

Logbert strictly follows the MVVM pattern for UI components:

```
┌─────────────┐      ┌──────────────────┐      ┌─────────────┐
│    View     │─────▶│   ViewModel      │─────▶│    Model    │
│   (.axaml)  │      │  (Observable     │      │  (Business  │
│             │◀─────│   Properties)    │◀─────│   Logic)    │
└─────────────┘      └──────────────────┘      └─────────────┘
     │                        │
     │                        │
     ▼                        ▼
  Data Binding          INotifyPropertyChanged
  Commands              RelayCommand
```

**Benefits:**
- Clear separation of concerns
- Testable business logic
- Reusable ViewModels
- Designer-friendly XAML

### Component Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                        MainWindow                             │
│  ┌────────────────────────────────────────────────────────┐  │
│  │                  MainWindowViewModel                    │  │
│  │  • DockFactory (manages docking layout)                │  │
│  │  • DockLayoutManager (persists layout)                 │  │
│  │  • Documents (collection of log tabs)                  │  │
│  │  • ActiveDocument (currently selected)                 │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                                │
│  ┌─────────┐  ┌──────────────────┐  ┌──────────────────┐    │
│  │ Filter  │  │  LogViewerControl│  │   Logger Tree    │    │
│  │ Panel   │  │                  │  │   & Bookmarks    │    │
│  │         │  │  • DataGrid      │  │                  │    │
│  │ ViewModel  │  ViewModel       │  │   ViewModel      │    │
│  └─────────┘  └──────────────────┘  └──────────────────┘    │
│                                                                │
│  ┌────────────────────────────────────────────────────────┐  │
│  │            EnhancedLogDetailsView                       │  │
│  │  • Formatted message display                           │  │
│  │  • Exception visualization                             │  │
│  │  • Metadata sections                                   │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. Log Message System

#### LogMessage Hierarchy

```
                    LogMessage (abstract)
                          |
        ┌─────────────────┼─────────────────┐
        │                 │                 │
LogMessageLog4Net  LogMessageSyslog  LogMessageCustom
LogMessageNLog     LogMessageEventlog  LogMessageWinDebug
```

**Base Properties:**
- `Index` - Message sequence number
- `Timestamp` - When message was created
- `Level` - Log level (Trace, Debug, Info, Warning, Error, Fatal)
- `Logger` - Source logger name
- `Message` - Log message text
- `RawData` - Original data structure

**Specialized Implementations:**

**LogMessageLog4Net** (`src/Logbert/Logging/LogMessageLog4Net.cs`)
- Parses Log4Net XML format (`<log4j:event>`)
- Supports thread info, location data, custom properties
- Column layout: Number, Level, Timestamp, Logger, Thread, Message

**LogMessageSyslog** (`src/Logbert/Logging/LogMessageSyslog.cs`)
- Parses RFC 3164 syslog format
- Extracts Priority (Facility + Severity)
- Column layout: Number, Severity, Timestamp, Facility, Sender, Message

**LogMessageCustom** (`src/Logbert/Logging/LogMessageCustom.cs`)
- User-configurable regex-based parsing
- Dynamic column definitions via Columnizer
- Flexible for proprietary log formats

### 2. Receiver Architecture

#### ILogProvider Interface

All log receivers implement `ILogProvider`:

```csharp
public interface ILogProvider
{
    string Name { get; }
    string Description { get; }
    string Tooltip { get; }
    ILogSettingsCtrl? Settings { get; }
    Dictionary<int, LogColumnData> Columns { get; }
    LogLevel SupportedLevels { get; }

    void Initialize(ILogHandler logHandler);
    void Shutdown();
    void Clear();
    void Reset();
}
```

#### ReceiverBase Abstract Class

Common functionality for all receivers:

```csharp
public abstract class ReceiverBase : ILogProvider, IDisposable
{
    protected ILogHandler mLogHandler;
    protected Encoding mEncoding;

    // Abstract properties for derived classes
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract ILogSettingsCtrl Settings { get; }
}
```

#### Receiver Types

**File-Based Receivers:**
```
FileSystemWatcher → File Change Event → StreamReader → Parse → LogMessage[]
                                                                      ↓
                                                              ILogHandler.HandleMessage()
```

**Network Receivers:**
```
UdpClient/TcpListener → Async Receive → Parse → LogMessage[]
                                                      ↓
                                              ILogHandler.HandleMessage()
```

**Example: Log4NetFileReceiver** (`src/Logbert/Receiver/Log4NetFileReceiver/`)

```csharp
public class Log4NetFileReceiver : ReceiverBase
{
    private FileSystemWatcher mFileWatcher;
    private StreamReader mFileReader;
    private long mLastFileOffset;

    private void OnLogFileChanged(object sender, FileSystemEventArgs e)
    {
        // Read new data from file
        mFileReader.BaseStream.Seek(mLastFileOffset, SeekOrigin.Begin);

        // Parse XML messages
        while (/* more data */)
        {
            string xmlData = ReadUntil("</log4j:event>");
            LogMessage msg = LogMessageLog4Net.ParseData(xmlData, index);
            messages.Add(msg);
        }

        // Send to handler
        mLogHandler.HandleMessage(messages.ToArray());
    }
}
```

### 3. UI Components

#### MainWindow

**Location:** `src/Logbert/Views/MainWindow.axaml`

**Responsibilities:**
- Application shell and menu system
- Dialog coordination
- Document lifecycle management

**Key Methods:**
- `ShowNewLogSourceDialog()` - Two-step receiver selection and configuration
- `ShowAboutDialog()` - About dialog
- `ShowOptionsDialog()` - Settings
- `ShowFindDialog()` - Search
- `ShowScriptEditor()` - Lua script editor
- `ShowStatisticsDialog()` - Log statistics

#### Docking System

**DockFactory** (`src/Logbert/Docking/DockFactory.cs`)
- Creates and manages dock layout
- Tool panels: Filter, Logger Tree, Bookmarks
- Document area for log tabs

```csharp
public class DockFactory
{
    public void AddDocument(LogDocumentViewModel document)
    {
        var dockDocument = new Document
        {
            Id = $"Doc_{document.GetHashCode()}",
            Title = document.Title,
            Context = document
        };
        documentDock.VisibleDockables.Add(dockDocument);
    }
}
```

**DockLayoutManager** (`src/Logbert/Docking/DockLayoutManager.cs`)
- Persists layout to `%APPDATA%/Logbert/layout.json`
- Restores layout on startup

#### LogViewerControl

**Location:** `src/Logbert/Views/Controls/LogViewerControl.axaml`

**Features:**
- DataGrid with virtualization (handles 100k+ messages)
- Color-coded log levels
- ColorMap visualization on right side
- Context menu for copy, bookmark, etc.

**Data Flow:**
```
ILogProvider → LogViewerViewModel.HandleMessage()
                       ↓
              Messages.Add(logMsg)
                       ↓
              FilteredMessages (based on level filters)
                       ↓
              DataGrid ItemsSource binding
```

#### EnhancedLogDetailsView

**Location:** `src/Logbert/Views/Controls/EnhancedLogDetailsView.axaml`

**Sections:**
1. **Header** - Color-coded level badge
2. **Metadata** - Timestamp, logger, thread
3. **Message** - Main log message with wrapping
4. **Exception** - Red background for exceptions
5. **Raw Data** - Expandable section for full data

### 4. Dialogs

#### Receiver Configuration Flow

```
1. NewLogSourceDialog
      ↓ (user selects "Log4Net File")
2. ReceiverConfigurationDialog
      ↓ (hosts Log4NetFileReceiverSettingsView)
3. File picker dialog
      ↓ (user selects log file)
4. Validation
      ↓ (settings validated)
5. ILogProvider instance created
      ↓
6. LogDocumentViewModel created and initialized
      ↓
7. Document added to docking system
```

**ReceiverConfigurationDialog** (`src/Logbert/Views/Dialogs/ReceiverConfigurationDialog.axaml.cs`)

```csharp
public ReceiverConfigurationDialog(string receiverType)
{
    // Create appropriate settings control
    switch (receiverType)
    {
        case "Log4Net File":
            _settingsControl = new Log4NetFileReceiverSettingsViewModel();
            break;
        case "NLog File":
            _settingsControl = new NLogFileReceiverSettingsViewModel();
            break;
    }
}
```

### 5. Scripting Engine

#### MoonSharp Integration

**Location:** `src/Logbert/Scripting/ScriptEngine.cs`

**Architecture:**
```
Lua Script
    ↓
MoonSharp Interpreter
    ↓
C# Script Engine Wrapper
    ↓
LogMessage Processing
```

**API Surface:**

```lua
-- Filter function (return true to show message)
function filter(message)
    return message.Level == "Error"
end

-- Process function (transform message)
function process(message)
    message.Logger = string.upper(message.Logger)
    return message
end

-- Access message properties
message.Index
message.Level
message.Logger
message.Message
message.Timestamp.Year
message.Timestamp.Hour
```

**Implementation:**

```csharp
public class ScriptEngine
{
    private Script _script;

    public void LoadScript(string scriptText)
    {
        _script = new Script();

        // Register types
        UserData.RegisterType<LogMessage>();
        UserData.RegisterType<LogLevel>();

        // Add print function
        _script.Globals["print"] = (Action<string>)Print;

        // Execute script
        _script.DoString(scriptText);
    }

    public bool ExecuteFilter(LogMessage message)
    {
        DynValue filterFunc = _script.Globals.Get("filter");
        DynValue result = _script.Call(filterFunc, message);
        return result.Boolean;
    }
}
```

### 6. Data Persistence

#### Settings Storage

**Windows:** `%APPDATA%/Logbert/`
**macOS:** `~/Library/Application Support/Logbert/`
**Linux:** `~/.config/Logbert/`

**Files:**
- `layout.json` - Dock layout configuration
- `settings.json` - Application settings (JSON-based persistence)
- `scripts/` - Saved Lua scripts (planned)

#### Layout Persistence

```csharp
public class DockLayoutManager
{
    private readonly string _layoutFilePath;

    public void SaveLayout(IDock? layout)
    {
        var json = SerializeLayout(layout);
        File.WriteAllText(_layoutFilePath, json);
    }

    public IDock? LoadLayout()
    {
        if (File.Exists(_layoutFilePath))
        {
            var json = File.ReadAllText(_layoutFilePath);
            return DeserializeLayout(json);
        }
        return null;
    }
}
```

## Design Patterns

### 1. Observable Pattern

Using `CommunityToolkit.Mvvm` source generators:

```csharp
public partial class LogViewerViewModel : ObservableObject
{
    [ObservableProperty]
    private double _fontSize = 11;  // Generates OnPropertyChanged

    [RelayCommand]
    private void OnZoomIn()  // Generates ZoomInCommand
    {
        FontSize += 2;
    }
}
```

### 2. Factory Pattern

**DockFactory** creates and manages dockable components:

```csharp
public class DockFactory
{
    public IDock CreateLayout()
    {
        return new ProportionalDock
        {
            Proportion = double.NaN,
            Orientation = Orientation.Horizontal,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock { /* tools */ },
                new SplitterDock(),
                new DocumentDock { /* documents */ }
            )
        };
    }
}
```

### 3. Strategy Pattern

Different log message parsers implement different parsing strategies:

```csharp
public abstract class LogMessage
{
    public abstract DateTime Timestamp { get; }
    public abstract string Message { get; }
    public abstract LogLevel Level { get; }
}

// Concrete strategies
public class LogMessageLog4Net : LogMessage
{
    // XML parsing strategy
}

public class LogMessageSyslog : LogMessage
{
    // RFC 3164 parsing strategy
}
```

### 4. Observer Pattern

ILogHandler receives updates from ILogProvider:

```csharp
public interface ILogHandler
{
    void HandleMessage(LogMessage logMsg);
    void HandleMessage(LogMessage[] logMsgs);
    void HandleError(LogError error);
}

// Receiver sends updates
mLogHandler.HandleMessage(newMessages.ToArray());
```

## Threading Model

### UI Thread

- All UI operations run on Avalonia's UI thread
- Uses `Dispatcher.UIThread.Post()` for cross-thread updates

```csharp
public void HandleMessage(LogMessage logMsg)
{
    Dispatcher.UIThread.Post(() =>
    {
        Messages.Add(logMsg);
        FilteredMessages.Add(logMsg);
    });
}
```

### Background Threads

**File Receivers:**
- FileSystemWatcher events on ThreadPool threads
- StreamReader operations on background threads

**Network Receivers:**
- Async UDP/TCP operations using BeginReceive/EndReceive
- Socket operations on ThreadPool threads

## Performance Considerations

### DataGrid Virtualization

Avalonia DataGrid uses UI virtualization:
- Only visible rows are rendered
- Handles 100,000+ messages efficiently
- Scroll performance maintained

### Message Batching

Receivers batch messages before sending:

```csharp
private FixedSizedQueue<LogMessage> mMessages = new(100);

// Collect messages
while (hasMore)
{
    mMessages.Enqueue(ParseMessage());
}

// Send batch
mLogHandler.HandleMessage(mMessages.ToArray());
```

### Observable Collections

Uses `ObservableCollection<T>` with change notifications:
- Minimal UI updates
- CollectionChanged events only for actual changes

## Error Handling

### Receiver Errors

```csharp
public void HandleError(LogError error)
{
    Dispatcher.UIThread.Post(() =>
    {
        // Log to debug output
        Debug.WriteLine($"Receiver error: {error.Message}");

        // TODO: Show user notification
    });
}
```

### Validation

Settings dialogs validate before creating receivers:

```csharp
public ValidationResult ValidateSettings()
{
    if (!File.Exists(FilePath))
    {
        return ValidationResult.Error("File does not exist");
    }
    return ValidationResult.Success;
}
```

## Testing Strategy

### Unit Tests
- ViewModel logic (without UI dependencies)
- Log message parsers
- Receiver data processing

### Integration Tests
- Receiver end-to-end (file monitoring, network listening)
- Script engine execution
- Data persistence

### UI Tests (Planned)
- Avalonia UI testing framework
- Automated interaction tests

## Future Architecture Enhancements

### Plugin System
```
ILogReceiverPlugin
    ↓
Plugin Discovery (MEF or custom)
    ↓
Dynamic Loading
    ↓
Configuration UI Generation
```

### Dependency Injection
- Use Microsoft.Extensions.DependencyInjection
- Service registration in Program.cs
- Constructor injection in ViewModels

### Event Aggregation
- Weak event pattern for loose coupling
- Pub/sub messaging between components

## References

- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [MVVM Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [Dock.Avalonia](https://github.com/wieslawsoltes/Dock)
- [MoonSharp Documentation](https://www.moonsharp.org/getting_started.html)
