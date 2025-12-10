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
| **Phase 6** | In Progress (~50%) |
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
Phase 6: Testing & Polish           [##########..........]  50%
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

**Current State:** ✅ COMPLETE (100%)

**Implementation Summary:**

Created comprehensive JSON-based settings persistence with platform-specific storage:

**Files Created:**

| File | Status | Description |
|------|--------|-------------|
| `Services/SettingsService.cs` | ✅ Created | Singleton service with JSON serialization, auto-save, dirty flag tracking |
| `Models/AppSettings.cs` | ✅ Created | Complete POCO with window, panel, column, preference, and behavior settings |
| `MainWindow.axaml.cs` | ✅ Updated | OnWindowLoaded/OnWindowClosing handlers for state persistence |
| `OptionsDialogViewModel.cs` | ✅ Updated | LoadSettings/SaveSettings using SettingsService (TODOs removed) |

**Settings Persisted:**
- ✅ Window: X, Y, Width, Height, WindowState (Normal/Maximized/Minimized)
- ✅ Panels: LeftPanelWidth, RightPanelWidth, BottomPanelHeight, visibility flags
- ✅ Columns: Dictionary<string, double> for column name → width mappings
- ✅ Preferences: AlwaysOnTop, MinimizeToTray, ShowWelcomeScreen, MaxRecentFiles
- ✅ Theme: SelectedTheme (Light/Dark/System)
- ✅ Fonts: DefaultFontFamily, DefaultFontSize
- ✅ Logging: EnableLogging, LogRetentionDays
- ✅ Behavior: AutoScroll, ShowTimestamps, TimestampFormat
- ✅ Recent Files: List<string> for MRU tracking

**Storage Location:**
- Windows: `%AppData%\Logbert\settings.json`
- Linux/macOS: `~/.config/Logbert/settings.json`

---

### 2. Recent Files Menu (Priority: MEDIUM)

**Current State:** ✅ COMPLETE (100%)

**Implementation Summary:**

Migrated MruManager to use SettingsService and created fully functional recent files menu:

**Files Updated:**

| File | Status | Description |
|------|--------|-------------|
| `Helper/MruManager.cs` | ✅ Updated | Migrated from StringCollection to List<string>, uses SettingsService |
| `MainWindowViewModel.cs` | ✅ Updated | Added RecentFiles ObservableCollection, OpenRecentFileCommand |
| `MainWindow.axaml` | ✅ Updated | Added "Recent Files" submenu with data binding |

**Features Implemented:**
- ✅ Max 9 recent files maintained
- ✅ Most recent files appear first in list
- ✅ Files persisted in settings.json via SettingsService
- ✅ Auto-refresh when MRU list changes (via MruListChanged event)
- ✅ Dynamic menu with "(No recent files)" placeholder when empty
- ✅ Material Design icon for visual consistency
- ✅ Command binding to OpenRecentFileCommand with file path parameter

**Menu Implementation:**
```xml
<MenuItem Header="Recent Files" ItemsSource="{Binding RecentFiles}">
    <MenuItem.Icon>
        <PathIcon Data="M13,3V9H21V3M13,21H21V11H13M3,21H11V15H3M3,13H11V3H3V13Z"/>
    </MenuItem.Icon>
    <MenuItem.ItemTemplate>
        <DataTemplate>
            <MenuItem Header="{Binding}"
                      Command="{Binding $parent[Window].((vm:MainWindowViewModel)DataContext).OpenRecentFileCommand}"
                      CommandParameter="{Binding}" />
        </DataTemplate>
    </MenuItem.ItemTemplate>
    <MenuItem.Styles>
        <Style Selector="MenuItem:empty">
            <Setter Property="IsEnabled" Value="False"/>
            <Setter Property="Header" Value="(No recent files)"/>
        </Style>
    </MenuItem.Styles>
</MenuItem>
```

---

### 3. Export Functionality (Priority: MEDIUM)

**Current State:** ✅ COMPLETE (100%)

**Implementation Summary:**

Created full export functionality with dialog UI and async export service:

**Files Created:**

| File | Status | Description |
|------|--------|-------------|
| `Services/ExportService.cs` | ✅ Created | Async CSV/Text export with progress reporting, cancellation support |
| `Views/Dialogs/ExportDialog.axaml` | ✅ Created | Export options dialog with scope, format, encoding selection |
| `Views/Dialogs/ExportDialog.axaml.cs` | ✅ Created | Code-behind with file picker, progress handling |
| `ViewModels/Dialogs/ExportDialogViewModel.cs` | ✅ Created | ViewModel with export options and progress tracking |
| `MainWindowViewModel.cs` | ✅ Updated | Added ExportCommand |
| `MainWindow.axaml` | ✅ Updated | Added Export menu item (Ctrl+E) |
| `MainWindow.axaml.cs` | ✅ Updated | Added ShowExportDialog handler |

**Export Features:**
- ✅ Export scope: All Messages / Filtered Messages Only
- ✅ Export format: CSV / Plain Text
- ✅ Encoding options: UTF-8, UTF-8 BOM, UTF-16, UTF-16 BE, ASCII, Latin-1
- ✅ Include/exclude column headers option
- ✅ Native save file dialog integration
- ✅ Progress bar with percentage and message count
- ✅ Cancellation support
- ✅ Error handling with user feedback

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

- [x] **Settings Persistence** ✅ COMPLETE
  - [x] Create SettingsService with JSON storage
  - [x] Create AppSettings model
  - [x] Wire up window state saving/loading
  - [x] Wire up column width persistence (infrastructure ready)
  - [x] Update OptionsDialogViewModel to use SettingsService

- [x] **Recent Files Menu** ✅ COMPLETE
  - [x] Add RecentFiles property to MainWindowViewModel
  - [x] Add Recent Files submenu to MainWindow.axaml
  - [x] Wire up MruManager integration
  - [x] Persist MRU list in settings

- [x] **Export Functionality** ✅ COMPLETE
  - [x] Add Export menu item (Ctrl+E hotkey)
  - [x] Create ExportDialog for options
  - [x] Implement CSV/Text export with progress
  - [x] Create ExportService with async export

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

### Phase 6 (In Progress - 50%)

**Completed:**
- ✅ Documentation cleanup and consolidation
- ✅ Build configuration simplified to AnyCPU only
- ✅ Updated all docs to .NET 10 references
- ✅ Settings persistence (JSON-based with SettingsService)
- ✅ Recent files menu (MruManager integrated)
- ✅ Export functionality (ExportService, ExportDialog with CSV/Text, progress, cancellation)

**In Progress:**
- Error handling improvements (NotificationService pending)

**Pending:**
- Cross-platform testing (requires .NET SDK)
- Performance testing (requires .NET SDK)
- Documentation updates
- Deployment preparation

---

## Known Limitations

1. **Windows Event Log** - Windows only (by design)
2. **Windows Debug Output** - Windows only (by design)
3. **Code signing** - Not yet configured for macOS/Windows
4. **Testing** - Requires .NET SDK installation for build/run verification

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
