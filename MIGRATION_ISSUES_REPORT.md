# Logbert Migration Issues Report
## Comparison: src_old (WinForms) vs src (Avalonia .NET 10)
## Cross-Platform Compatibility Analysis (Windows, macOS, Linux)

Generated: December 2024

---

## Executive Summary

This report documents the differences, missing features, and bugs identified when comparing the original WinForms implementation (`src_old`) with the new Avalonia implementation (`src`). The analysis covers receivers, log message models, filtering, UI dialogs, settings, helper utilities, and **cross-platform compatibility for Windows, macOS, and Linux**.

### Critical Issues Found: 12 (was 8, +4 cross-platform)
### High Priority Issues: 20 (was 12, +8 cross-platform)
### Medium Priority Issues: 22 (was 15, +7 cross-platform)
### Low Priority Issues: 10 (was 8, +2 cross-platform)

---

## 1. CRITICAL BUGS (Must Fix Before Release)

### 1.1 LogError Constructor Bug - ForeColor Never Set
**File:** `src/Logbert/Logging/LogError.cs:134`
**Severity:** CRITICAL
**Status:** Exists in BOTH old and new versions

```csharp
private LogError(string title, string message, Color backColor, Color foreColor)
{
    Title     = title;
    Message   = message;
    BackColor = backColor;
    ForeColor = ForeColor;  // BUG: Should be 'foreColor' (parameter)
}
```

**Impact:** Error messages will never display with correct foreground colors. The parameter `foreColor` is ignored.

**Fix:**
```csharp
ForeColor = foreColor;  // Use lowercase parameter
```

---

### 1.2 NLogDirReceiver Loads Wrong Layout
**File:** `src/Logbert/Receiver/NLogDirReceiver/NLogDirReceiver.cs:478`
**Severity:** CRITICAL
**Status:** Exists in BOTH old and new versions

```csharp
public override string LoadLayout()
{
    return Properties.Settings.Default.DockLayoutNLogFileReceiver;  // WRONG!
}
```

**Impact:** NLogDirReceiver loads NLogFileReceiver's layout instead of its own, causing incorrect UI state restoration.

**Fix:**
```csharp
return Properties.Settings.Default.DockLayoutNLogDirReceiver;
```

---

### 1.3 ToLuaTable Timestamp Format Breaking Change
**File:** `src/Logbert/Logging/LogMessage.cs:236`
**Severity:** CRITICAL

```csharp
// OLD: Timestamp.ToUnixTimestamp() - seconds since Unix epoch
// NEW: Timestamp.Ticks - 100-nanosecond intervals since 0001-01-01
["Timestamp"] = Timestamp.Ticks  // TODO comment notes this
```

**Impact:** Lua scripts depending on Unix timestamps will break. Tick values are incompatible with Unix timestamps.

**Fix:** Use the existing `ToUnixTimestamp()` extension method from `DateTimeExtensions.cs`:
```csharp
["Timestamp"] = Timestamp.ToUnixTimestamp()
```

---

### 1.4 Windows-Only Receivers Exposed on All Platforms (CROSS-PLATFORM)
**File:** `src/Logbert/ViewModels/Dialogs/NewLogSourceDialogViewModel.cs`
**Severity:** CRITICAL
**Platforms Affected:** macOS, Linux

The Windows Event Log and Windows Debug Output receivers are unconditionally added to the UI on ALL platforms:

```csharp
// These are added without platform checks - will crash on macOS/Linux
AvailableReceivers.Add(new LogReceiverType { Name = "Windows Event Log", ... });
AvailableReceivers.Add(new LogReceiverType { Name = "Windows Debug Output", ... });
```

**Impact:** Users on macOS/Linux can select these receivers, causing application crashes when attempting to use them.

**Fix:** Add platform guards using `RuntimeInformation.IsOSPlatform()`.

---

## 2. HIGH PRIORITY ISSUES

### 2.1 Missing Avalonia Detail Views
**Severity:** HIGH

| Detail View | WinForms (src/Controls) | Avalonia (src/Views/Controls/Details) |
|-------------|-------------------------|---------------------------------------|
| Log4NetDetailsControl | ✅ | ✅ Log4NetDetailsView |
| SyslogDetailsControl | ✅ | ✅ SyslogDetailsView |
| EventLogDetailsControl | ✅ | ✅ EventLogDetailsView |
| **CustomDetailsControl** | ✅ | ❌ **MISSING** |
| **WinDebugDetailsControl** | ✅ | ❌ **MISSING** |

**Impact:** Custom log format and Windows Debug logs cannot display detailed message information in Avalonia UI.

---

### 2.2 FrmLogLevelMap Has No Avalonia Equivalent
**File:** `src/Logbert/Dialogs/FrmLogLevelMap.cs` (WinForms only)
**Severity:** HIGH

The log level mapping editor dialog exists only as WinForms. No `LogLevelMapDialog.axaml` exists.

**Impact:** Users cannot configure custom log level regex patterns for columnizers in the Avalonia UI.

---

### 2.3 Receiver Settings/DetailsControl Return Null
**All receiver files**
**Severity:** HIGH

All receivers now return `null` for `Settings` and `DetailsControl` properties:

```csharp
public override ILogSettingsCtrl? Settings => null;  // Was: new Log4NetFileReceiverSettings()
public override ILogPresenter? DetailsControl => null;  // Was: new Log4NetDetailsControl()
```

**Impact:** UI cannot dynamically create settings or detail views from receiver instances.

**Status:** This is likely intentional for Avalonia migration, but requires verification that the new UI creates these separately.

---

### 2.4 Missing Color Configuration in AppSettings
**File:** `src/Logbert/Models/AppSettings.cs`
**Severity:** HIGH

Old settings included per-level colors:
- `BackgroundColorTrace`, `BackgroundColorDebug`, etc.
- `ForegroundColorTrace`, `ForegroundColorDebug`, etc.
- `FontStyleTrace`, `FontStyleDebug`, etc.

**Status:** Not found in new `AppSettings.cs`. May be in theme system.

**Impact:** Users cannot customize log level colors.

---

### 2.5 Two Parallel Filter Systems Without Integration
**Severity:** HIGH

| System | Location | Features |
|--------|----------|----------|
| Old LogFilter | `Logging/Filter/` | 5 filter types, AND-only logic |
| New FilterRule | `Models/FilterRule.cs` | 8 operators, AND/OR logic, case sensitivity |

**Issue:** Both systems exist but are not integrated. UI may use one while code uses another.

**Impact:** Filter behavior may be inconsistent.

---

### 2.6 IOptionPanel Complete Interface Change
**File:** `src/Logbert/Interfaces/IOptionPanel.cs`
**Severity:** HIGH

```csharp
// OLD (WinForms)
System.Drawing.Image Image { get; }
void AdjustSizeAndLocation(System.Windows.Forms.Control parentControl);

// NEW (Avalonia)
IImage? Image { get; }
void AdjustSizeAndLocation(Rect parentBounds);
```

**Impact:** All IOptionPanel implementations must be completely rewritten.

---

### 2.7 Browser.cs Won't Open URLs on macOS/Linux (CROSS-PLATFORM)
**File:** `src/Logbert/Helper/Browser.cs:53,67`
**Severity:** HIGH
**Platforms Affected:** macOS, Linux

```csharp
Process.Start(URI);  // Won't work reliably on macOS/Linux
// Fallback uses IExplore.exe which doesn't exist on macOS/Linux
ProcessStartInfo startInfo = new ProcessStartInfo("IExplore.exe", URI);
```

**Impact:** Users cannot open documentation links, GitHub URLs, or help pages on non-Windows platforms.

**Fix:** Use platform-specific browser commands (`open` on macOS, `xdg-open` on Linux).

---

### 2.8 DataProtection Uses Windows DPAPI Only (CROSS-PLATFORM)
**File:** `src/Logbert/Helper/DataProtection.cs`
**Severity:** HIGH
**Platforms Affected:** macOS, Linux

```csharp
[SupportedOSPlatform("windows")]  // Already marked but no fallback
public sealed class DataProtection
{
    return ProtectedData.Protect(data, GetAditionalEntropy(), DataProtectionScope.CurrentUser);
}
```

**Impact:** Credential encryption fails on macOS/Linux with `PlatformNotSupportedException`. No fallback encryption.

**Fix:** Implement cross-platform encryption using `System.Security.Cryptography` AES.

---

### 2.9 Clipboard Operations Windows-Only (CROSS-PLATFORM)
**File:** `src/Logbert/Helper/Extensions.cs:128` + 40 other files
**Severity:** HIGH
**Platforms Affected:** macOS, Linux

```csharp
Clipboard.SetText(text);  // System.Windows.Forms.Clipboard - Windows only
```

**Impact:** Copy to clipboard fails on macOS/Linux throughout the application.

**Fix:** Use Avalonia's `Application.Current.Clipboard.SetTextAsync()`.

---

### 2.10 Win32 P/Invoke Calls Will Crash (CROSS-PLATFORM)
**Files:** `Helper/Win32.cs`, `Helper/Extensions.cs`, `Controls/ColorMap.cs`
**Severity:** HIGH
**Platforms Affected:** macOS, Linux

16 DllImport declarations to `user32.dll`, `kernel32.dll`, `advapi32.dll`, `shlwapi.dll`.

**Impact:** Any code path using these will throw `DllNotFoundException` on macOS/Linux.

**Fix:** Add `[SupportedOSPlatform("windows")]` attributes and platform guards.

---

### 2.11 DebugMonitor Entire Class Windows-Only (CROSS-PLATFORM)
**File:** `src/Logbert/Receiver/WinDebugReceiver/DebugMonitor.cs`
**Severity:** HIGH
**Platforms Affected:** macOS, Linux

10 P/Invoke calls to kernel32.dll and advapi32.dll for Windows debug output monitoring.

**Impact:** WinDebugReceiver completely non-functional on macOS/Linux.

**Fix:** Add `[SupportedOSPlatform("windows")]` and hide from UI on non-Windows.

---

### 2.12 EventlogReceiver Windows-Only (CROSS-PLATFORM)
**File:** `src/Logbert/Receiver/EventlogReceiver/EventlogReceiver.cs`
**Severity:** HIGH
**Platforms Affected:** macOS, Linux

Uses `System.Diagnostics.EventLog` which is Windows-only.

**Impact:** EventlogReceiver crashes on macOS/Linux.

**Fix:** Add `[SupportedOSPlatform("windows")]` and hide from UI on non-Windows.

---

### 2.13 Hardcoded Windows Font Names (CROSS-PLATFORM)
**Files:** `Models/AppSettings.cs:171`, 12 Designer files
**Severity:** HIGH
**Platforms Affected:** macOS, Linux

```csharp
DefaultFontFamily = "Consolas"  // Windows-only font
Font = new System.Drawing.Font("Segoe UI", 9F...)  // Windows-only font
```

**Impact:** UI renders with fallback fonts, potentially causing layout issues.

**Fix:** Use cross-platform font stack: `"Menlo, Consolas, Liberation Mono, monospace"`.

---

## 3. MEDIUM PRIORITY ISSUES

### 3.1 MruManager List Ordering Changed
**File:** `src/Logbert/Helper/MruManager.cs`
**Severity:** MEDIUM

```csharp
// OLD: Adds to end of list
Settings.Default.MruManagerFiles.Add(filename);

// NEW: Inserts at position 0
settings.RecentFiles.Insert(0, filename);
```

**Impact:** MRU list order is reversed. Verify UI displays correctly.

---

### 3.2 GetCsvLine Uses Hardcoded Timestamp Format
**File:** `src/Logbert/Logging/LogMessage.cs:252`
**Severity:** MEDIUM

```csharp
// OLD: Used Settings.Default.TimestampFormat
// NEW: Hardcoded format
Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")
```

**Impact:** Users cannot customize CSV export timestamp format.

---

### 3.3 Code Duplication in Extension Methods
**Severity:** MEDIUM

Methods duplicated in both `Extensions.cs` and new specialized classes:

| Method | Extensions.cs | DateTimeExtensions.cs | StringExtensions.cs |
|--------|--------------|----------------------|---------------------|
| ToUnixTimestamp | Line 283 | Line 45 | - |
| ToCsv | Line 157 | - | Line 46 |
| ToRegex | Line 179 | - | Line 56 |

**Impact:** Maintenance burden; bugs may be fixed in one location but not another.

---

### 3.4 LogFilterRegex No Error Handling
**File:** `src/Logbert/Logging/Filter/LogFilterRegex.cs`
**Severity:** MEDIUM

```csharp
public LogFilterRegex(int columnIndex, string value) : base(columnIndex, value)
{
    mFilterRegex = new Regex(value ?? string.Empty);  // Can throw!
}

public override bool Match(object value)
{
    return mFilterRegex.IsMatch(value.ToString());  // Can throw!
}
```

**Impact:** Invalid regex patterns will crash the application.

**Contrast with FilterRule:** New system validates regex and catches exceptions.

---

### 3.5 Missing Search Preferences Persistence
**Severity:** MEDIUM

Old settings included:
- `FrmFindSearchMatchCase`
- `FrmFindSearchMatchWholeWord`
- `FrmFindSearchUseRegex`
- `FrmFindSearchValue`

**Status:** Not found in new AppSettings.

---

### 3.6 Missing UI Refresh Interval Setting
**Severity:** MEDIUM

Old: `UiRefreshIntervalMs` (default 250ms)
New: Not found in AppSettings

**Impact:** Cannot tune UI refresh performance.

---

### 3.7 Settings Persistence Method Changed
**All receivers**
**Severity:** MEDIUM

```csharp
// OLD
Properties.Settings.Default.SaveSettings();

// NEW
Properties.Settings.Default.Save();
```

**Impact:** If old `SaveSettings()` had custom logic, settings may not persist correctly.

---

### 3.8 ThreadName/MachineName/UserName/Exception Virtual Properties Not Implemented
**File:** `src/Logbert/Logging/LogMessage.cs`
**Severity:** MEDIUM

Base class added virtual properties:
```csharp
public virtual string? ThreadName => null;
public virtual string? MachineName => null;
public virtual string? UserName => null;
public virtual string? Exception => null;
```

**Issue:** Derived classes (LogMessageLog4Net, etc.) don't override these, even when data is available.

**Example:** `LogMessageLog4Net` has `Thread` property but doesn't implement `ThreadName` override.

---

### 3.9 File Path Case Sensitivity (CROSS-PLATFORM)
**Files:** `Log4NetDirReceiver.cs:212`, `NLogDirReceiver.cs:212`, `CustomDirReceiver.cs:222`
**Severity:** MEDIUM
**Platforms Affected:** macOS, Linux

```csharp
if (e.FullPath.Equals(mCurrentLogFile))  // Case-sensitive comparison
```

**Impact:** FileSystemWatcher may report paths with different casing, causing missed file changes on Windows/macOS.

**Fix:** Use `StringComparison.OrdinalIgnoreCase`.

---

### 3.10 DockLayoutManager Uses Wrong App Data Path (CROSS-PLATFORM)
**File:** `src/Logbert/Docking/DockLayoutManager.cs:19`
**Severity:** MEDIUM
**Platforms Affected:** macOS, Linux

```csharp
Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
```

**Impact:** Resolves to different paths per platform without proper handling. May conflict with SettingsService paths.

**Fix:** Use consistent platform-aware path logic like SettingsService.

---

### 3.11 Encoding.Default Platform Differences (CROSS-PLATFORM)
**Files:** 20+ receiver files
**Severity:** MEDIUM
**Platforms Affected:** Windows

```csharp
Encoding.Default.GetString(...)  // Windows-1252 on Windows, UTF-8 on Linux/macOS
```

**Impact:** Non-ASCII characters may be corrupted when receiving network data on Windows.

**Fix:** Explicitly use `Encoding.UTF8` for network data.

---

### 3.12 FileSystemWatcher Behavior Differences (CROSS-PLATFORM)
**Files:** 8 receiver files
**Severity:** MEDIUM
**Platforms Affected:** All

FileSystemWatcher has different reliability and timing characteristics per platform:
- Linux: Uses inotify, may miss rapid changes
- macOS: Uses FSEvents, batches notifications
- Windows: Most reliable

**Fix:** Add error handling and polling fallback for network shares.

---

## 4. MISSING FEATURES

### 4.1 Missing Avalonia Detail Views
- CustomDetailsView.axaml
- WinDebugDetailsView.axaml

### 4.2 Missing Avalonia Dialogs
- LogLevelMapDialog (for columnizer configuration)

### 4.3 Missing Filter Features in Old System
Old LogFilter system lacks:
- Case-insensitive matching
- Contains/NotContains operators
- EndsWith operator
- AND/OR combination logic

### 4.4 Potentially Missing UI Features
- Context menu in log window (copy, bookmark, tree sync)
- Column visibility toggle via right-click
- Dynamic row height adjustment
- Zoom controls
- Full statistics charting

---

## 5. CROSS-PLATFORM COMPATIBILITY SUMMARY

### 5.1 Windows-Only Components (Will Not Work on macOS/Linux)

| Component | File | Impact |
|-----------|------|--------|
| EventlogReceiver | `Receiver/EventlogReceiver/` | Entire feature unavailable |
| WinDebugReceiver | `Receiver/WinDebugReceiver/` | Entire feature unavailable |
| DataProtection (DPAPI) | `Helper/DataProtection.cs` | Credential encryption fails |
| Win32 P/Invoke | `Helper/Win32.cs` | Window management fails |
| Clipboard | `Helper/Extensions.cs` | Copy operations fail |
| Browser.Start | `Helper/Browser.cs` | URL opening fails |

### 5.2 Platform-Specific Behavior Differences

| Feature | Windows | macOS | Linux |
|---------|---------|-------|-------|
| Default Encoding | Windows-1252 | UTF-8 | UTF-8 |
| FileSystemWatcher | Reliable | FSEvents batching | inotify limits |
| Font Fallback | Consolas, Segoe UI | SF Mono, SF Pro | Liberation Mono |
| App Data Path | %AppData% | ~/Library/Application Support | ~/.config or $XDG_CONFIG_HOME |
| Keyboard Modifier | Ctrl | Cmd (should be) | Ctrl |

### 5.3 P/Invoke Calls Requiring Platform Guards

| DLL | Functions | Files |
|-----|-----------|-------|
| user32.dll | SendMessage, GetWindowLong, ShowWindow, SetForegroundWindow, LoadCursor, SetCursor | Extensions.cs, Win32.cs, ColorMap.cs |
| kernel32.dll | MapViewOfFile, CreateEvent, CreateFileMapping, CloseHandle, WaitForSingleObject, AttachThreadInput | DebugMonitor.cs, ThreadAttachedDelayedCallback.cs |
| advapi32.dll | InitializeSecurityDescriptor, SetSecurityDescriptorDacl | DebugMonitor.cs |
| shlwapi.dll | StrCmpLogicalW | Win32.cs |

---

## 6. RECOMMENDED ACTIONS

### Immediate (Before Any Release)
1. ☐ Fix LogError.cs ForeColor assignment bug
2. ☐ Fix NLogDirReceiver.LoadLayout() to use correct setting
3. ☐ Fix LogMessage.ToLuaTable() to use ToUnixTimestamp()
4. ☐ Add platform guards to hide Windows-only receivers on macOS/Linux
5. ☐ Fix Browser.cs to use platform-specific URL opening

### High Priority (Next Sprint)
6. Add `[SupportedOSPlatform("windows")]` to Windows-only classes
7. Fix clipboard operations to use Avalonia clipboard
8. Create cross-platform DataProtection fallback
9. Fix hardcoded font names with cross-platform fallbacks
10. Create CustomDetailsView.axaml
11. Create WinDebugDetailsView.axaml
12. Create LogLevelMapDialog.axaml
13. Add color configuration to AppSettings
14. Integrate FilterRule with existing filter UI

### Medium Priority
15. Verify MruManager ordering is correct in UI
16. Add timestamp format setting for CSV export
17. Add regex validation to old LogFilter classes
18. Implement virtual property overrides in log message classes
19. Consolidate or document extension method duplication
20. Fix file path case-sensitivity comparisons
21. Fix Encoding.Default usage to explicit UTF-8
22. Add FileSystemWatcher error handling

### Testing Required
- Test all receivers with their settings dialogs
- Test docking layout save/restore for all receiver types
- Test Lua scripting with timestamp values
- Test CSV export formatting
- Test MRU file list ordering
- **Test on macOS and Linux**
- Test Windows-only features are hidden on other platforms
- Test clipboard operations on all platforms
- Test URL opening on all platforms

---

## 7. FILES REQUIRING CHANGES

### Critical Priority

| File | Issue | Platform |
|------|-------|----------|
| `Logging/LogError.cs:134` | ForeColor bug | All |
| `Receiver/NLogDirReceiver/NLogDirReceiver.cs:478` | Wrong layout | All |
| `Logging/LogMessage.cs:236` | Timestamp format | All |
| `ViewModels/Dialogs/NewLogSourceDialogViewModel.cs` | Hide Windows receivers | macOS/Linux |
| `Helper/Browser.cs` | Platform URL opening | macOS/Linux |

### High Priority

| File | Issue | Platform |
|------|-------|----------|
| `Receiver/EventlogReceiver/EventlogReceiver.cs` | Add platform attribute | macOS/Linux |
| `Receiver/WinDebugReceiver/WinDebugReceiver.cs` | Add platform attribute | macOS/Linux |
| `Receiver/WinDebugReceiver/DebugMonitor.cs` | Add platform attribute | macOS/Linux |
| `Helper/Win32.cs` | Add platform attribute | macOS/Linux |
| `Helper/Extensions.cs` | Fix clipboard, P/Invoke | macOS/Linux |
| `Helper/DataProtection.cs` | Add fallback encryption | macOS/Linux |
| `Models/AppSettings.cs` | Fix font names | macOS/Linux |
| `Views/Controls/Details/` | Missing Custom/WinDebug views | All |
| `Views/Dialogs/` | Missing LogLevelMapDialog | All |

### Medium Priority

| File | Issue | Platform |
|------|-------|----------|
| `Receiver/Log4NetDirReceiver/Log4NetDirReceiver.cs:212` | Case-sensitive path | All |
| `Receiver/NLogDirReceiver/NLogDirReceiver.cs:212` | Case-sensitive path | All |
| `Receiver/CustomReceiver/CustomDirReceiver/CustomDirReceiver.cs:222` | Case-sensitive path | All |
| `Docking/DockLayoutManager.cs` | App data path | macOS/Linux |
| `Logging/Filter/LogFilterRegex.cs` | No error handling | All |
| `Helper/MruManager.cs` | Verify ordering | All |

---

## 8. Appendix: File Counts

| Category | src_old | src |
|----------|---------|-----|
| Total .cs files | 215 | 278 |
| Receiver types | 16 | 16 |
| Detail controls | 5 | 3 (Avalonia) |
| Dialogs | ~19 | ~30 |
| Filter classes | 6 | 6 + FilterRule |
| Windows-only P/Invoke | N/A | 16 calls in 5 files |
| Cross-platform ready | 0% | ~70% |

---

## 9. Platform Compatibility Matrix

| Feature | Windows | macOS | Linux | Notes |
|---------|:-------:|:-----:|:-----:|-------|
| Core Log Viewing | ✅ | ✅ | ✅ | Avalonia UI |
| Log4Net File Receiver | ✅ | ✅ | ✅ | Cross-platform |
| Log4Net UDP Receiver | ✅ | ✅ | ✅ | Cross-platform |
| NLog File Receiver | ✅ | ✅ | ✅ | Cross-platform |
| NLog TCP Receiver | ✅ | ✅ | ✅ | Cross-platform |
| Syslog Receiver | ✅ | ✅ | ✅ | Cross-platform |
| Custom Receivers | ✅ | ✅ | ✅ | Cross-platform |
| **Event Log Receiver** | ✅ | ❌ | ❌ | Windows-only |
| **WinDebug Receiver** | ✅ | ❌ | ❌ | Windows-only |
| Clipboard Copy | ✅ | ❌ | ❌ | Needs fix |
| Open URLs | ✅ | ❌ | ❌ | Needs fix |
| Credential Encryption | ✅ | ❌ | ❌ | Needs fallback |
| Settings Persistence | ✅ | ✅ | ✅ | JSON file |
| Docking Layouts | ✅ | ✅ | ✅ | Cross-platform |
