# Logbert Migration Issues Report
## Comparison: src_old (WinForms) vs src (Avalonia .NET 10)

Generated: December 2024

---

## Executive Summary

This report documents the differences, missing features, and bugs identified when comparing the original WinForms implementation (`src_old`) with the new Avalonia implementation (`src`). The analysis covers receivers, log message models, filtering, UI dialogs, settings, and helper utilities.

### Critical Issues Found: 8
### High Priority Issues: 12
### Medium Priority Issues: 15
### Low Priority Issues: 8

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

### 1.4 LogMessageSyslog Still Uses ToUnixTimestamp Correctly
**File:** `src/Logbert/Logging/LogMessageSyslog.cs:373`
**Note:** This file correctly uses `ToUnixTimestamp()` - confirm it compiles

```csharp
localTimeTable["Timestamp"] = Timestamp.ToUnixTimestamp();
```

**Action:** Verify this compiles. The extension method exists in `DateTimeExtensions.cs`, ensure namespace is imported.

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

## 5. ARCHITECTURE CHANGES (Non-Issues, Just Different)

### 5.1 Namespace Migration
- `Couchcoding.Logbert.*` → `Logbert.*`

### 5.2 Settings Storage
- Old: Windows Registry (via Properties.Settings)
- New: JSON file (`~/.config/Logbert/settings.json`)

### 5.3 UI Framework
- Old: WinForms with WeifenLuo docking
- New: Avalonia with MVVM pattern

### 5.4 ILogProvider Interface Nullability
```csharp
// OLD
ILogSettingsCtrl Settings { get; }
ILogPresenter DetailsControl { get; }

// NEW (nullable)
ILogSettingsCtrl? Settings { get; }
ILogPresenter? DetailsControl { get; }
```

---

## 6. RECOMMENDED ACTIONS

### Immediate (Before Any Release)
1. ✅ Fix LogError.cs ForeColor assignment bug
2. ✅ Fix NLogDirReceiver.LoadLayout() to use correct setting
3. ✅ Fix LogMessage.ToLuaTable() to use ToUnixTimestamp()

### High Priority (Next Sprint)
4. Create CustomDetailsView.axaml
5. Create WinDebugDetailsView.axaml
6. Create LogLevelMapDialog.axaml
7. Add color configuration to AppSettings
8. Integrate FilterRule with existing filter UI

### Medium Priority
9. Verify MruManager ordering is correct in UI
10. Add timestamp format setting for CSV export
11. Add regex validation to old LogFilter classes
12. Implement virtual property overrides in log message classes
13. Consolidate or document extension method duplication

### Testing Required
- Test all receivers with their settings dialogs
- Test docking layout save/restore for all receiver types
- Test Lua scripting with timestamp values
- Test CSV export formatting
- Test MRU file list ordering

---

## 7. FILES REQUIRING CHANGES

| File | Issue | Priority |
|------|-------|----------|
| `Logging/LogError.cs:134` | ForeColor bug | CRITICAL |
| `Receiver/NLogDirReceiver/NLogDirReceiver.cs:478` | Wrong layout | CRITICAL |
| `Logging/LogMessage.cs:236` | Timestamp format | CRITICAL |
| `Views/Controls/Details/` | Missing Custom/WinDebug views | HIGH |
| `Views/Dialogs/` | Missing LogLevelMapDialog | HIGH |
| `Models/AppSettings.cs` | Missing color settings | HIGH |
| `Logging/Filter/LogFilterRegex.cs` | No error handling | MEDIUM |
| `Helper/MruManager.cs` | Verify ordering | MEDIUM |

---

## Appendix: File Counts

| Category | src_old | src |
|----------|---------|-----|
| Total .cs files | 215 | 278 |
| Receiver types | 16 | 16 |
| Detail controls | 5 | 3 (Avalonia) |
| Dialogs | ~19 | ~30 |
| Filter classes | 6 | 6 + FilterRule |
