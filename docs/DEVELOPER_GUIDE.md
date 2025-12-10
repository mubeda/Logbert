# Logbert Developer Guide

This guide provides information for developers who want to contribute to Logbert or understand its internals.

## Table of Contents

1. [Development Setup](#development-setup)
2. [Project Structure](#project-structure)
3. [Building and Running](#building-and-running)
4. [Adding New Features](#adding-new-features)
5. [Testing](#testing)
6. [Contributing](#contributing)

## Development Setup

### Prerequisites

- **.NET 10.0 SDK** or later - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
- **IDE** - Choose one:
  - Visual Studio 2022 17.9+ with .NET desktop development workload
  - JetBrains Rider 2024.2+
  - Visual Studio Code with C# Dev Kit extension

### Optional Tools

- **Git** - Version control
- **Avalonia XAML IntelliSense** - IDE extension for XAML editing
- **ReSharper** or **Roslyn Analyzers** - Code quality

### Initial Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/couchcoding/Logbert.git
   cd Logbert
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the solution:**
   ```bash
   dotnet build
   ```

4. **Run the application:**
   ```bash
   dotnet run --project src/Logbert/Logbert.csproj
   ```

## Project Structure

```
Logbert/
├── src/
│   └── Logbert/
│       ├── App.axaml                  # Application definition
│       ├── Program.cs                 # Entry point
│       ├── Docking/                   # Dock layout management
│       │   ├── DockFactory.cs
│       │   └── DockLayoutManager.cs
│       ├── Interfaces/                # Core interfaces
│       │   ├── ILogProvider.cs
│       │   ├── ILogHandler.cs
│       │   ├── ILogSettingsCtrl.cs
│       │   └── ILogPresenter.cs
│       ├── Logging/                   # Log message models
│       │   ├── LogMessage.cs
│       │   ├── LogMessageLog4Net.cs
│       │   ├── LogMessageNLog.cs
│       │   ├── LogMessageSyslog.cs
│       │   └── ...
│       ├── Receiver/                  # Log receivers
│       │   ├── ReceiverBase.cs
│       │   ├── Log4NetFileReceiver/
│       │   ├── NLogFileReceiver/
│       │   ├── SyslogUdpReceiver/
│       │   └── ...
│       ├── Scripting/                 # Lua scripting
│       │   └── ScriptEngine.cs
│       ├── ViewModels/                # MVVM ViewModels
│       │   ├── MainWindowViewModel.cs
│       │   ├── LogDocumentViewModel.cs
│       │   ├── Controls/
│       │   │   ├── LogViewerViewModel.cs
│       │   │   ├── FilterPanelViewModel.cs
│       │   │   └── ...
│       │   └── Dialogs/
│       │       ├── AboutDialogViewModel.cs
│       │       ├── SearchDialogViewModel.cs
│       │       └── ReceiverSettings/
│       │           ├── Log4NetFileReceiverSettingsViewModel.cs
│       │           └── ...
│       └── Views/                     # Avalonia UI Views
│           ├── MainWindow.axaml
│           ├── Controls/
│           │   ├── LogViewerControl.axaml
│           │   ├── EnhancedLogDetailsView.axaml
│           │   └── ...
│           └── Dialogs/
│               ├── AboutDialog.axaml
│               ├── SearchDialog.axaml
│               └── ReceiverSettings/
│                   ├── Log4NetFileReceiverSettingsView.axaml
│                   └── ...
├── docs/                              # Documentation
│   ├── ARCHITECTURE.md
│   ├── USER_GUIDE.md
│   ├── DEVELOPER_GUIDE.md (this file)
│   ├── RECEIVERS.md
│   └── SCRIPTING.md
└── README.md
```

## Building and Running

### Debug Build

```bash
dotnet build
dotnet run --project src/Logbert/Logbert.csproj
```

### Release Build

```bash
dotnet build -c Release
```

### Platform-Specific Builds

**Windows:**
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

**macOS:**
```bash
dotnet publish -c Release -r osx-x64 --self-contained
# Create app bundle
dotnet publish -c Release -r osx-x64 -p:CreatePackage=true
```

**Linux:**
```bash
dotnet publish -c Release -r linux-x64 --self-contained
```

### Hot Reload

Use the `--watch` flag for hot reload during development:

```bash
dotnet watch run --project src/Logbert/Logbert.csproj
```

## Adding New Features

### Adding a New Log Receiver

Let's walk through adding a new JSON log file receiver.

#### Step 1: Create LogMessage Implementation

Create `src/Logbert/Logging/LogMessageJson.cs`:

```csharp
using System;
using System.Text.Json;

namespace Couchcoding.Logbert.Logging
{
    public class LogMessageJson : LogMessage
    {
        private readonly JsonElement _data;

        public override DateTime Timestamp =>
            _data.TryGetProperty("timestamp", out var ts)
                ? DateTime.Parse(ts.GetString())
                : DateTime.Now;

        public override string Message =>
            _data.TryGetProperty("message", out var msg)
                ? msg.GetString() ?? string.Empty
                : string.Empty;

        public override LogLevel Level =>
            _data.TryGetProperty("level", out var lvl)
                ? ParseLevel(lvl.GetString())
                : LogLevel.Info;

        public LogMessageJson(JsonElement data, int index)
            : base(data, index)
        {
            _data = data;
            mLogger = _data.TryGetProperty("logger", out var logger)
                ? logger.GetString() ?? string.Empty
                : string.Empty;
        }

        private static LogLevel ParseLevel(string? level)
        {
            return level?.ToUpperInvariant() switch
            {
                "TRACE" => LogLevel.Trace,
                "DEBUG" => LogLevel.Debug,
                "INFO" => LogLevel.Info,
                "WARN" or "WARNING" => LogLevel.Warning,
                "ERROR" => LogLevel.Error,
                "FATAL" => LogLevel.Fatal,
                _ => LogLevel.Info
            };
        }

        public override object? GetValueForColumn(int columnIndex)
        {
            return columnIndex switch
            {
                0 => Index,
                1 => Level,
                2 => Timestamp,
                3 => Logger,
                4 => Message,
                _ => null
            };
        }
    }
}
```

#### Step 2: Create Receiver Implementation

Create `src/Logbert/Receiver/JsonFileReceiver/JsonFileReceiver.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Logging;

namespace Couchcoding.Logbert.Receiver.JsonFileReceiver
{
    public class JsonFileReceiver : ReceiverBase
    {
        private readonly string mFileToObserve;
        private readonly bool mStartFromBeginning;
        private FileSystemWatcher mFileWatcher;
        private StreamReader mFileReader;
        private long mLastFileOffset;
        private int mLogNumber;

        public override string Name => "JSON File Receiver";

        public override string Description =>
            $"{Name} ({Path.GetFileName(mFileToObserve)})";

        public override string ExportFileName => Description;

        public override string Tooltip => mFileToObserve;

        public override ILogSettingsCtrl Settings =>
            new JsonFileReceiverSettings();

        // ... (implement other required properties)

        public JsonFileReceiver(
            string fileToObserve,
            bool startFromBeginning,
            int codepage)
        {
            mFileToObserve = fileToObserve;
            mStartFromBeginning = startFromBeginning;
            mEncoding = Encoding.GetEncoding(codepage);
        }

        public override void Initialize(ILogHandler logHandler)
        {
            mLogHandler = logHandler;

            // Open file
            mFileReader = new StreamReader(
                new FileStream(mFileToObserve, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite),
                mEncoding);

            if (!mStartFromBeginning)
            {
                mFileReader.BaseStream.Seek(0, SeekOrigin.End);
                mLastFileOffset = mFileReader.BaseStream.Position;
            }

            // Setup FileSystemWatcher
            mFileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(mFileToObserve),
                Filter = Path.GetFileName(mFileToObserve),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            mFileWatcher.Changed += OnLogFileChanged;
            mFileWatcher.EnableRaisingEvents = true;

            // Read initial content
            if (mStartFromBeginning)
            {
                ReadNewLogMessagesFromFile();
            }
        }

        private void OnLogFileChanged(object sender, FileSystemEventArgs e)
        {
            ReadNewLogMessagesFromFile();
        }

        private void ReadNewLogMessagesFromFile()
        {
            lock (mFileReader)
            {
                mFileReader.BaseStream.Seek(mLastFileOffset, SeekOrigin.Begin);

                var messages = new List<LogMessage>();
                string? line;

                while ((line = mFileReader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var jsonElement = JsonSerializer.Deserialize<JsonElement>(line);
                        messages.Add(new LogMessageJson(jsonElement, ++mLogNumber));
                    }
                    catch (JsonException)
                    {
                        // Skip malformed JSON
                    }
                }

                mLastFileOffset = mFileReader.BaseStream.Position;

                if (messages.Count > 0 && mLogHandler != null)
                {
                    mLogHandler.HandleMessage(messages.ToArray());
                }
            }
        }

        public override void Shutdown()
        {
            if (mFileWatcher != null)
            {
                mFileWatcher.EnableRaisingEvents = false;
                mFileWatcher.Changed -= OnLogFileChanged;
                mFileWatcher.Dispose();
            }

            mFileReader?.Dispose();
        }
    }
}
```

#### Step 3: Create Settings ViewModel

Create `src/Logbert/ViewModels/Dialogs/ReceiverSettings/JsonFileReceiverSettingsViewModel.cs`:

```csharp
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Receiver;
using Couchcoding.Logbert.Receiver.JsonFileReceiver;

namespace Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings
{
    public partial class JsonFileReceiverSettingsViewModel : ObservableObject, ILogSettingsCtrl
    {
        [ObservableProperty]
        private string _filePath = string.Empty;

        [ObservableProperty]
        private bool _startFromBeginning = false;

        [ObservableProperty]
        private EncodingInfo? _selectedEncoding;

        [ObservableProperty]
        private ObservableCollection<EncodingInfo> _availableEncodings = new();

        public JsonFileReceiverSettingsViewModel()
        {
            foreach (var encoding in Encoding.GetEncodings().OrderBy(e => e.DisplayName))
            {
                AvailableEncodings.Add(encoding);
            }

            SelectedEncoding = AvailableEncodings.FirstOrDefault(e => e.CodePage == Encoding.UTF8.CodePage);
        }

        public ValidationResult ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                return ValidationResult.Error("Please select a log file.");

            if (!File.Exists(FilePath))
                return ValidationResult.Error($"File '{FilePath}' does not exist.");

            return ValidationResult.Success;
        }

        public ILogProvider GetConfiguredInstance()
        {
            return new JsonFileReceiver(
                FilePath,
                StartFromBeginning,
                SelectedEncoding?.CodePage ?? Encoding.UTF8.CodePage);
        }

        public void Dispose() { }
    }
}
```

#### Step 4: Create Settings View

Create `src/Logbert/Views/Dialogs/ReceiverSettings/JsonFileReceiverSettingsView.axaml`:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Couchcoding.Logbert.ViewModels.Dialogs.ReceiverSettings"
             x:Class="Couchcoding.Logbert.Views.Dialogs.ReceiverSettings.JsonFileReceiverSettingsView"
             x:DataType="vm:JsonFileReceiverSettingsViewModel">

    <StackPanel Spacing="15" Margin="20">
        <StackPanel Spacing="8">
            <TextBlock Text="JSON Log File:" FontWeight="SemiBold"/>
            <Grid ColumnDefinitions="*,Auto">
                <TextBox Grid.Column="0"
                         Text="{Binding FilePath}"
                         Watermark="Select a JSON log file..."
                         Margin="0,0,10,0"/>
                <Button Grid.Column="1"
                        Content="Browse..."
                        Click="BrowseFile"
                        Width="100"/>
            </Grid>
        </StackPanel>

        <CheckBox IsChecked="{Binding StartFromBeginning}"
                  Content="Start reading from beginning"/>

        <StackPanel Spacing="8">
            <TextBlock Text="Text Encoding:" FontWeight="SemiBold"/>
            <ComboBox ItemsSource="{Binding AvailableEncodings}"
                      SelectedItem="{Binding SelectedEncoding}"
                      MinWidth="300">
                <ComboBox.ItemTemplate>
                    <DataTemplate>
                        <TextBlock Text="{Binding DisplayName}"/>
                    </DataTemplate>
                </ComboBox.ItemTemplate>
            </ComboBox>
        </StackPanel>
    </StackPanel>
</UserControl>
```

#### Step 5: Register in ReceiverConfigurationDialog

Update `src/Logbert/Views/Dialogs/ReceiverConfigurationDialog.axaml.cs`:

```csharp
case "JSON File":
    var jsonViewModel = new JsonFileReceiverSettingsViewModel();
    _settingsControl = jsonViewModel;
    SettingsContent.Content = new JsonFileReceiverSettingsView
    {
        DataContext = jsonViewModel
    };
    break;
```

#### Step 6: Add to NewLogSourceDialog

Update `src/Logbert/ViewModels/Dialogs/NewLogSourceDialogViewModel.cs`:

```csharp
AvailableReceivers.Add(new LogReceiverType
{
    Name = "JSON File",
    Description = "Read log messages from a JSON log file",
    Category = "File"
});
```

### Adding a New Dialog

Example: Adding a "Log Export" dialog

#### 1. Create ViewModel

```csharp
public partial class ExportDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _exportPath = string.Empty;

    [ObservableProperty]
    private string _selectedFormat = "CSV";

    public ObservableCollection<string> AvailableFormats { get; } =
        new() { "CSV", "JSON", "XML", "Text" };

    [RelayCommand]
    private async Task Export()
    {
        // Export logic
    }
}
```

#### 2. Create View (AXAML)

```xml
<Window xmlns="https://github.com/avaloniaui"
        x:Class="Couchcoding.Logbert.Views.Dialogs.ExportDialog"
        Title="Export Logs">
    <StackPanel Margin="20" Spacing="15">
        <ComboBox ItemsSource="{Binding AvailableFormats}"
                  SelectedItem="{Binding SelectedFormat}"/>
        <Button Content="Export" Command="{Binding ExportCommand}"/>
    </StackPanel>
</Window>
```

#### 3. Wire up in MainWindow

```csharp
public async void ShowExportDialog(object? sender, RoutedEventArgs e)
{
    var dialog = new ExportDialog();
    await dialog.ShowDialog(this);
}
```

## Testing

### Unit Tests (Planned)

Create test project:

```bash
dotnet new xunit -n Logbert.Tests
cd Logbert.Tests
dotnet add reference ../src/Logbert/Logbert.csproj
dotnet add package FluentAssertions
```

Example test:

```csharp
public class LogMessageLog4NetTests
{
    [Fact]
    public void ParseData_ValidXml_ReturnsLogMessage()
    {
        // Arrange
        string xml = @"<log4j:event ...>...</log4j:event>";

        // Act
        var message = LogMessageLog4Net.ParseData(xml, 1);

        // Assert
        message.Should().NotBeNull();
        message.Level.Should().Be(LogLevel.Error);
    }
}
```

### Manual Testing Checklist

- [ ] Open Log4Net file
- [ ] Open NLog file
- [ ] Filter by log level
- [ ] Search with regex
- [ ] Create bookmark
- [ ] Run Lua script
- [ ] View statistics
- [ ] Export logs
- [ ] Test on Windows/macOS/Linux

## Contributing

### Workflow

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/my-new-feature
   ```
3. **Make your changes**
4. **Test thoroughly**
5. **Commit with clear messages**
   ```bash
   git commit -m "Add JSON file receiver support"
   ```
6. **Push to your fork**
   ```bash
   git push origin feature/my-new-feature
   ```
7. **Create a Pull Request**

### Commit Message Guidelines

Format: `<type>: <description>`

**Types:**
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `style:` Formatting, no code change
- `refactor:` Code restructuring
- `test:` Adding tests
- `chore:` Build process, dependencies

**Examples:**
```
feat: Add JSON file receiver
fix: Correct timestamp parsing in SyslogReceiver
docs: Update user guide with scripting examples
refactor: Extract message parsing into separate class
```

### Code Style

**C# Conventions:**
- Use PascalCase for public members
- Use camelCase with `_` prefix for private fields
- Use `var` for local variables when type is obvious
- Add XML documentation for public APIs

**XAML Conventions:**
- Use 4-space indentation
- Order properties: Name, Grid placement, Layout, Styling, Binding
- Use descriptive names for controls

**MVVM Guidelines:**
- ViewModels should not reference Views
- Use ObservableObject base class
- Use [ObservableProperty] for bindable properties
- Use [RelayCommand] for commands

### Pull Request Process

1. Ensure all tests pass
2. Update documentation if needed
3. Add entry to CHANGELOG.md (planned)
4. Request review from maintainers
5. Address review feedback
6. Squash commits if requested

## Debugging

### Avalonia DevTools

Enable Avalonia DevTools for runtime inspection:

1. Add package:
   ```bash
   dotnet add package Avalonia.Diagnostics --version 11.3.8
   ```

2. Enable in code:
   ```csharp
   public static AppBuilder BuildAvaloniaApp()
       => AppBuilder.Configure<App>()
           .UsePlatformDetect()
           .LogToTrace()
           .UseReactiveUI()
           .AttachDevTools();  // Add this
   ```

3. Press F12 at runtime to open DevTools

### Logging

Add logging for debugging:

```csharp
using System.Diagnostics;

Debug.WriteLine($"Processing message: {message.Index}");
```

### Breakpoints

Set breakpoints in:
- `OnLogFileChanged()` - File monitoring
- `HandleMessage()` - Message processing
- ViewModel constructors - Initialization
- Command handlers - User actions

## Resources

- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [MVVM Toolkit Docs](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [MoonSharp Documentation](https://www.moonsharp.org/)

## Getting Help

- Open an issue on GitHub
- Join discussions
- Check existing documentation
- Review source code examples
