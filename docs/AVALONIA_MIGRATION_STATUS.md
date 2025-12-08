# Logbert Avalonia Migration Status

**Last Updated:** December 2025
**Target Framework:** .NET 10.0
**UI Framework:** Avalonia 11.3.8
**Build Platform:** AnyCPU (cross-platform)

---

## Current Status: Phase 6 - Testing & Polish

| Metric | Status |
|--------|--------|
| **Build** | 0 errors, 0 warnings |
| **Phase 5** | Complete (100%) |
| **Phase 6** | In Progress (~20%) |
| **Receivers** | 16/16 implemented |
| **Core Features** | All functional |

---

## Migration Progress

```text
Phase 1: Core Infrastructure        [####################] 100%
Phase 2: Models & Interfaces        [####################] 100%
Phase 3: Log Viewer Components      [####################] 100%
Phase 4: WinForms Elimination       [####################] 100%
Phase 5: Avalonia Implementation    [####################] 100%
Phase 6: Testing & Polish           [####................]  20%
```

---

## What's Complete

### All 16 Receiver Types

**File-based (6):**

- Log4Net File - XML files with FileSystemWatcher
- Log4Net Directory - Multiple files as one stream
- NLog File - XML files
- NLog Directory - Multiple files monitoring
- Syslog File - RFC 3164 format
- Custom File - Configurable columnizer/regex

**Network (7):**

- Log4Net UDP
- NLog TCP
- NLog UDP
- Syslog UDP
- Custom TCP
- Custom UDP
- Custom HTTP

**System (2):**

- Windows Event Log (Windows only)
- Windows Debug Output (Windows only)

**Custom (1):**

- Regex-based with Columnizer support

### Core Features

- Custom docking layout (Grid-based, replaced Dock.Avalonia)
- Search with regex, case-sensitive, whole word options
- Statistics dialog with visual analytics
- ColorMap log level visualization
- Logger tree with hierarchical view
- Bookmarks panel
- Filter panel (log levels)
- Lua scripting (MoonSharp)
- Enhanced log details view
- **Advanced filters** - LogFilterString & LogFilterRegex fully implemented

### Architecture

```text
Logbert (Avalonia)
  +-- ViewModels/
  |     +-- MainWindowViewModel.cs
  |     +-- Docking/ (Filter, Bookmarks, LoggerTree)
  |     +-- Dialogs/ (Search, Statistics, Options, Receivers)
  |     +-- Controls/ (LogViewer, ScriptEditor, ColorMap)
  +-- Views/
  |     +-- MainWindow.axaml
  |     +-- Docking/*.axaml
  |     +-- Dialogs/*.axaml
  |     +-- Controls/*.axaml
  +-- Receiver/ (16 implementations)
```

---

## Phase 6: Detailed Implementation Plan

### 1. Settings Persistence (Priority: HIGH)

**Current State:** NOT IMPLEMENTED (0%)

**Problem:** `OptionsDialogViewModel.cs` has TODO stubs:
```csharp
private void LoadSettings()
{
    // TODO: Load from application settings
    // For now, using defaults
}

private void SaveSettings()
{
    // TODO: Save to application settings
}
```

**Implementation Required:**

| Task | File(s) | Description |
|------|---------|-------------|
| Create Settings Service | `Services/SettingsService.cs` (new) | JSON-based settings with auto-save |
| Settings Model | `Models/AppSettings.cs` (new) | POCO for all user preferences |
| Window State | `MainWindowViewModel.cs` | Track window position, size, state |
| Panel Layout | `MainWindowViewModel.cs` | Splitter positions, panel visibility |
| Column Widths | `LogViewerViewModel.cs` | DataGrid column widths |
| User Preferences | `OptionsDialogViewModel.cs` | Theme, font, logging level |
| Save on Exit | `MainWindow.axaml.cs` | Trigger save on window closing |
| Load on Start | `App.axaml.cs` | Load settings before UI creation |

**Settings to Persist:**
- Window: X, Y, Width, Height, WindowState (Normal/Maximized)
- Panels: LeftPanelWidth, RightPanelWidth, BottomPanelHeight
- Columns: Array of column name → width mappings
- Preferences: Theme (Light/Dark/System), Font family/size
- Behavior: AutoScroll, ShowTimestamps, TimestampFormat

**Suggested Storage:** `%AppData%/Logbert/settings.json` (Windows), `~/.config/Logbert/settings.json` (Linux/macOS)

---

### 2. Recent Files Menu (Priority: MEDIUM)

**Current State:** PARTIALLY IMPLEMENTED (40%)

**What Exists:**
- `Helper/MruManager.cs` - Fully functional MRU logic (max 9 files)
- Methods: `AddFile()`, `RemoveFile()`, `ClearFiles()`
- Events: `MruListChanged`
- Legacy integration in `MainForm.cs` (WinForms, excluded from build)

**Implementation Required:**

| Task | File(s) | Description |
|------|---------|-------------|
| Add MRU Property | `MainWindowViewModel.cs` | `ObservableCollection<string> RecentFiles` |
| Initialize MruManager | `MainWindowViewModel.cs` | Create instance, subscribe to events |
| Menu Binding | `MainWindow.axaml` | Add "Recent Files" submenu under File |
| Open Recent Command | `MainWindowViewModel.cs` | `OpenRecentFileCommand` with file path parameter |
| Update on Open | `MainWindowViewModel.cs` | Call `MruManager.AddFile()` when opening files |
| Persist MRU List | `SettingsService.cs` | Include RecentFiles in settings JSON |

**Menu Structure:**
```xml
<MenuItem Header="_File">
    <MenuItem Header="_New Log Source..." />
    <MenuItem Header="_Open..." />
    <MenuItem Header="Recent Files" ItemsSource="{Binding RecentFiles}">
        <MenuItem.ItemTemplate>
            <DataTemplate>
                <MenuItem Header="{Binding}" Command="{Binding $parent.OpenRecentFileCommand}" CommandParameter="{Binding}" />
            </DataTemplate>
        </MenuItem.ItemTemplate>
    </MenuItem>
    <Separator />
    <MenuItem Header="E_xit" />
</MenuItem>
```

---

### 3. Export Functionality (Priority: MEDIUM)

**Current State:** PARTIALLY IMPLEMENTED (60%)

**What Exists:**
- `LogMessage.GetCsvLine()` - Abstract method implemented in all LogMessage subclasses
- `LogMessageLog4Net.GetCsvLine()` - Full CSV export with proper escaping
- `LogMessageSyslog.GetCsvLine()`, `LogMessageWinDebug.GetCsvLine()`, etc.
- `Helper/StringExtensions.ToCsv()` - CSV escaping utility
- `ILogProvider.GetCsvHeader()` - Column headers for CSV

**Implementation Required:**

| Task | File(s) | Description |
|------|---------|-------------|
| Export Command | `MainWindowViewModel.cs` | `ExportToCsvCommand` |
| Export Dialog | `Views/Dialogs/ExportDialog.axaml` (new) | Options: All/Filtered, delimiter, encoding |
| Save File Dialog | Export command handler | Show native save dialog |
| Export Logic | `Services/ExportService.cs` (new) | Write CSV with progress |
| Menu Integration | `MainWindow.axaml` | Add "Export..." menu item |
| Progress Indicator | `MainWindowViewModel.cs` | Show export progress for large files |

**Export Options:**
- Scope: All Messages / Filtered Messages Only / Selected Messages
- Format: CSV / Original Format
- Encoding: UTF-8 / UTF-16 / ASCII
- Include Headers: Yes/No

---

### 4. Error Handling Improvements (Priority: MEDIUM)

**Current State:** INCOMPLETE (40%)

**What Exists:**
- Validation messages in receiver dialogs (good)
- `ValidationResult` with user-friendly messages

**Problems Found:**
- Many `// TODO: Show error notification to user` comments
- Generic exception catching without user feedback
- Locations with TODOs:
  - `SearchDialogViewModel.cs` - Search errors
  - `LogViewerViewModel.cs` - Load/parse errors
  - Multiple receiver ViewModels - Validation errors

**Implementation Required:**

| Task | File(s) | Description |
|------|---------|-------------|
| Notification Service | `Services/NotificationService.cs` (new) | Centralized error/info display |
| Error Dialog | `Views/Dialogs/ErrorDialog.axaml` (new) | Standard error display with details |
| Toast Notifications | Consider Avalonia.Notifications | Non-blocking info messages |
| Replace TODOs | Multiple ViewModels | Wire up NotificationService calls |
| Exception Handling | Receiver classes | User-friendly messages for common errors |

**Error Categories:**
- **Validation Errors**: Show inline or dialog
- **Runtime Errors**: Show dialog with "Details" expander
- **Recoverable Errors**: Show toast with retry option
- **Fatal Errors**: Show dialog with "Exit" option

---

### 5. Cross-Platform Testing

**Current State:** NOT STARTED

| Platform | Build | Run | Receivers | UI | Notes |
|----------|-------|-----|-----------|-----|-------|
| Windows x64 | ⏳ | ⏳ | ⏳ | ⏳ | Primary dev platform |
| Windows ARM64 | ⏳ | ⏳ | ⏳ | ⏳ | Surface Pro X, etc. |
| macOS x64 | ⏳ | ⏳ | ⏳ | ⏳ | Intel Macs |
| macOS ARM64 | ⏳ | ⏳ | ⏳ | ⏳ | Apple Silicon |
| Linux x64 | ⏳ | ⏳ | ⏳ | ⏳ | Ubuntu, Fedora |

**Test Procedure per Platform:**
1. Build: `dotnet build`
2. Run: `dotnet run`
3. Test each receiver type applicable to platform
4. Test all UI interactions (resize, menus, dialogs)
5. Test theme following system preferences

**Platform-Specific Considerations:**
- **Windows**: Event Log and Debug Output receivers work
- **macOS**: File permissions, Gatekeeper signing
- **Linux**: GTK theme integration, firewall rules for network receivers

---

### 6. Performance Testing

**Current State:** NOT STARTED

**Performance Targets:**

| Metric | Target | Test Method |
|--------|--------|-------------|
| Load 100K messages | < 3 seconds | Stopwatch + profiler |
| Load 1M messages | < 10 seconds | Stopwatch + profiler |
| Scrolling FPS | > 30 FPS | Avalonia diagnostics |
| Search 1M messages | < 3 seconds | Stopwatch |
| Memory (1M messages) | < 500 MB | Process.WorkingSet64 |

**Test Files Needed:**
- Generate 100K, 500K, 1M message log files
- Include variety of log levels and message lengths
- Test both file and network receivers

**Optimization Areas:**
- DataGrid virtualization verification
- Lazy loading for large files
- Search algorithm efficiency
- Memory pooling for LogMessage objects

---

### 7. Documentation Updates

**Current State:** PENDING

| Document | Status | Updates Needed |
|----------|--------|----------------|
| USER_GUIDE.md | ⏳ | Screenshots, new features |
| DEVELOPER_GUIDE.md | ⏳ | Build instructions, architecture |
| RECEIVERS.md | ⏳ | Verify all 16 receivers documented |
| SCRIPTING.md | ⏳ | Verify Lua API reference |
| Release Notes | ❌ | Create for v2.0 |

---

### 8. Deployment Preparation

**Current State:** NOT STARTED

| Platform | Package Type | Tool | Status |
|----------|-------------|------|--------|
| Windows | ZIP / MSIX | dotnet publish | ⏳ |
| macOS | .app bundle | dotnet publish + bundler | ⏳ |
| macOS | .dmg | create-dmg | ⏳ |
| Linux | AppImage | appimage-builder | ⏳ |
| Linux | .deb | dpkg-deb | ⏳ |
| Linux | .rpm | rpmbuild | ⏳ |

**Publish Commands:**
```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained -o publish/win-x64
dotnet publish -c Release -r win-arm64 --self-contained -o publish/win-arm64

# macOS
dotnet publish -c Release -r osx-x64 --self-contained -o publish/osx-x64
dotnet publish -c Release -r osx-arm64 --self-contained -o publish/osx-arm64

# Linux
dotnet publish -c Release -r linux-x64 --self-contained -o publish/linux-x64
dotnet publish -c Release -r linux-arm64 --self-contained -o publish/linux-arm64
```

---

## Phase 6 Summary Checklist

### Implementation Tasks

- [ ] **Settings Persistence**
  - [ ] Create SettingsService with JSON storage
  - [ ] Create AppSettings model
  - [ ] Wire up window state saving/loading
  - [ ] Wire up column width persistence
  - [ ] Update OptionsDialogViewModel to use SettingsService

- [ ] **Recent Files Menu**
  - [ ] Add RecentFiles property to MainWindowViewModel
  - [ ] Add Recent Files submenu to MainWindow.axaml
  - [ ] Wire up MruManager integration
  - [ ] Persist MRU list in settings

- [ ] **Export Functionality**
  - [ ] Add Export menu item
  - [ ] Create ExportDialog for options
  - [ ] Implement CSV export with progress
  - [ ] Test with large files

- [ ] **Error Handling**
  - [ ] Create NotificationService
  - [ ] Create ErrorDialog view
  - [ ] Replace all TODO error comments
  - [ ] Add user-friendly exception messages

### Testing Tasks

- [ ] Cross-platform: Windows x64
- [ ] Cross-platform: Windows ARM64
- [ ] Cross-platform: macOS x64
- [ ] Cross-platform: macOS ARM64
- [ ] Cross-platform: Linux x64
- [ ] Performance: 100K messages
- [ ] Performance: 1M messages
- [ ] Memory profiling
- [ ] All 16 receivers functional test

### Documentation Tasks

- [ ] Update USER_GUIDE.md
- [ ] Update DEVELOPER_GUIDE.md
- [ ] Verify RECEIVERS.md
- [ ] Create Release Notes

### Deployment Tasks

- [ ] Windows ZIP package
- [ ] macOS .app bundle
- [ ] macOS .dmg installer
- [ ] Linux AppImage
- [ ] Linux .deb package
- [ ] Linux .rpm package

---

## Technical Details

### Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Avalonia | 11.3.8 | UI framework |
| Avalonia.Desktop | 11.3.8 | Desktop support |
| Avalonia.Themes.Fluent | 11.3.8 | Theme |
| Avalonia.Controls.DataGrid | 11.3.8 | Log display |
| AvaloniaEdit | 11.3.0 | Script editor |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM pattern |
| System.Configuration.ConfigurationManager | 10.0.0 | Settings |
| MoonSharp | - | Lua scripting |

### Build Configuration

- **Platform:** AnyCPU only (supports x64/ARM on all OS)
- **Target:** net10.0
- **Self-contained publish supported**

### Compile Exclusions

Legacy WinForms files are excluded from compilation but retained for reference:

- Old WinForms dialogs (Frm*.cs)
- WinForms UserControl settings classes
- Legacy helper classes with GDI+ dependencies

---

## Migration History

### Phase 4 (Complete)

- Achieved zero compilation errors (632 to 0)
- Removed WinForms dependencies
- Replaced ToolBar/StatusBar with Avalonia equivalents
- Added missing LogMessage properties

### Phase 5 (Complete)

- Implemented custom docking system
- Created all 16 receiver configuration dialogs
- Re-enabled all receiver backends
- Implemented search, statistics, options dialogs
- Created ColorMap visualization
- Added LogMessage subclasses (Syslog, WinDebug)

### Phase 6 (In Progress - 20%)

- Documentation cleanup and consolidation
- Build configuration simplified to AnyCPU only
- Updated all docs to .NET 10 references
- Settings persistence (pending)
- Recent files menu (pending)
- Export UI integration (pending)
- Cross-platform testing (pending)
- Deployment preparation (pending)

---

## Known Limitations

1. **Windows Event Log** - Windows only (by design)
2. **Windows Debug Output** - Windows only (by design)
3. **Settings persistence** - Implementation pending (Phase 6)
4. **Code signing** - Not yet configured for macOS/Windows

---

## Build Instructions

```bash
# Prerequisites: .NET 10 SDK

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

## Related Documentation

- [Testing Checklist](TESTING_CHECKLIST.md) - Detailed test procedures for all 16 receivers
- [User Guide](USER_GUIDE.md) - How to use Logbert
- [Developer Guide](DEVELOPER_GUIDE.md) - Contributing and development
- [Architecture](ARCHITECTURE.md) - System design overview
- [Receivers](RECEIVERS.md) - Receiver configuration details
- [Scripting](SCRIPTING.md) - Lua API reference
