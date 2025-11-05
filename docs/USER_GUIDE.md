# Logbert User Guide

Welcome to Logbert, a cross-platform log file viewer for developers and system administrators. This guide will help you get started and make the most of Logbert's features.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Opening Log Files](#opening-log-files)
3. [Viewing and Filtering Logs](#viewing-and-filtering-logs)
4. [Search and Navigation](#search-and-navigation)
5. [Bookmarks](#bookmarks)
6. [Scripting](#scripting)
7. [Statistics](#statistics)
8. [Tips and Tricks](#tips-and-tricks)

## Getting Started

### First Launch

When you first launch Logbert, you'll see a welcome screen with options to:
- **New Log Source** - Configure a new log receiver
- **Open Log File** - Quickly open a log file

### Main Window Layout

```
┌────────────────────────────────────────────────────────────┐
│ File  Edit  View  Tools  Help                              │
├────────┬───────────────────────────────┬───────────────────┤
│ Filter │                               │  Logger Tree      │
│ Panel  │      Log Messages             │                   │
│        │      (DataGrid)               │  Bookmarks        │
│        │                               │                   │
├────────┴───────────────────────────────┴───────────────────┤
│              Log Message Details                            │
└────────────────────────────────────────────────────────────┘
```

**Key Areas:**
- **Filter Panel** (Left) - Control which log levels to show
- **Log Messages** (Center) - Main log message grid
- **Logger Tree** (Right) - Hierarchical view of log sources
- **Bookmarks** (Right) - Quick access to marked messages
- **Details View** (Bottom) - Detailed view of selected message

### Customizing Layout

You can drag and drop panels to rearrange them:
1. Click and hold panel header
2. Drag to desired location
3. Drop when highlight appears
4. Layout is automatically saved

## Opening Log Files

### Method 1: New Log Source Dialog

**Best for:** Configuring advanced log sources

1. Click **File → New** or press `Ctrl+N`
2. Select a log source type:
   - **Log4Net File** - Log4Net XML log files
   - **NLog File** - NLog XML log files
   - **Syslog File** - Syslog RFC 3164 format
   - **Network receivers** - UDP/TCP log streams
   - **Windows Event Log** - System event logs (Windows only)
3. Configure the selected source
4. Click **OK**

### Method 2: Open File Dialog

**Best for:** Quick access to log files

1. Click **File → Open** or press `Ctrl+O`
2. Select a log file
3. Logbert auto-detects format when possible

### Log4Net File Configuration

When selecting "Log4Net File":

1. **Log File Path** - Browse to your Log4Net XML file
2. **Start from beginning** - Check to load existing messages, uncheck for only new messages
3. **Text Encoding** - Select appropriate encoding (default: UTF-8)

**Log4Net Format Requirements:**
Your Log4Net appender should use XMLLayoutSchemaLog4j:

```xml
<appender name="LogFileAppender" type="log4net.Appender.RollingFileAppender">
  <layout type="log4net.Layout.XmlLayoutSchemaLog4j"/>
  <!-- Other configuration -->
</appender>
```

### NLog File Configuration

Similar to Log4Net, but uses Log4JXmlEventLayout:

```xml
<targets>
  <target name="logfile" xsi:type="File" fileName="app.log">
    <layout xsi:type="Log4JXmlEventLayout" />
  </target>
</targets>
```

## Viewing and Filtering Logs

### Log Level Filtering

The **Filter Panel** lets you show/hide messages by level:

- ☑ **Trace** - Detailed debug information
- ☑ **Debug** - Debug messages
- ☑ **Info** - Informational messages
- ☑ **Warning** - Warning messages
- ☑ **Error** - Error messages
- ☑ **Fatal** - Critical errors

**Color Coding:**
- 🟣 Trace - Purple/Light
- 🔵 Debug - Blue
- 🟢 Info - Green
- 🟡 Warning - Yellow/Orange
- 🔴 Error - Red
- ⚫ Fatal - Dark Red/Black

### Logger Tree

The **Logger Tree** shows a hierarchical view of all loggers:

```
▼ MyApp
  ▼ Services
    ► DatabaseService
    ► ApiService
  ▼ Controllers
    ► UserController
    ► OrderController
```

**Usage:**
- Click a logger to filter messages to that logger and its children
- Check/uncheck to show/hide specific loggers
- Hierarchy is determined by logger names (e.g., `MyApp.Services.DatabaseService`)

### Color Map

The vertical **Color Map** on the right side of the log grid provides:
- Visual overview of entire log file
- Color-coded by log level
- Click to jump to that position in the log

## Search and Navigation

### Find Dialog

Press `Ctrl+F` or click **Edit → Find** to open the search dialog.

**Search Options:**
- **Search Text** - Enter text to find
- **Match Case** - Case-sensitive search
- **Use Regular Expression** - Enable regex patterns
- **Search History** - Recently searched terms

**Examples:**

**Simple Text Search:**
```
Exception
```

**Regex Search:**
```
Error \d{4}           # Find "Error" followed by 4 digits
NullReferenceException|ArgumentException  # Multiple exceptions
```

### Keyboard Navigation

- `↑` `↓` - Navigate messages
- `PageUp` `PageDown` - Scroll page
- `Home` `End` - Go to first/last message
- `Ctrl+F` - Find
- `F3` - Find next
- `Shift+F3` - Find previous

## Bookmarks

Bookmarks let you mark important log messages for quick access.

### Creating Bookmarks

**Method 1: Right-Click Menu**
1. Right-click a log message
2. Select "Add Bookmark"
3. (Optional) Enter a description

**Method 2: Keyboard**
1. Select a message
2. Press `Ctrl+B`

### Using Bookmarks

The **Bookmarks Panel** shows all bookmarked messages:

```
📌 Bookmark 1 - "Application started"
📌 Bookmark 2 - "Critical error in payment"
📌 Bookmark 3 - "User authentication failed"
```

**Features:**
- Click a bookmark to jump to that message
- Double-click to edit description
- Right-click to delete

## Scripting

Logbert includes a Lua scripting engine for advanced log processing.

### Opening Script Editor

Click **Tools → Script Editor** to open the script editor.

### Script Editor Interface

```
┌────────────────────────────────────────────────────────┐
│ [New] [Open] [Save] [Run] [Clear]                      │
├────────────────────────────────────────────────────────┤
│                                                         │
│  -- Your Lua Script Here                               │
│  function filter(message)                              │
│      return message.Level == "Error"                   │
│  end                                                    │
│                                                         │
├────────────────────────────────────────────────────────┤
│ Output:                                                 │
│ Script loaded successfully                              │
└────────────────────────────────────────────────────────┘
```

### Example Scripts

**Filter Only Errors:**
```lua
function filter(message)
    return message.Level == "Error" or message.Level == "Fatal"
end
```

**Filter by Logger:**
```lua
function filter(message)
    return string.match(message.Logger, "Database")
end
```

**Filter by Time Range:**
```lua
function filter(message)
    local hour = message.Timestamp.Hour
    return hour >= 9 and hour <= 17  -- Business hours only
end
```

**Transform Messages:**
```lua
function process(message)
    -- Convert logger to uppercase
    message.Logger = string.upper(message.Logger)
    return message
end
```

**Complex Filtering:**
```lua
function filter(message)
    -- Only show errors from specific services during business hours
    local isBusinessHour = message.Timestamp.Hour >= 9 and message.Timestamp.Hour <= 17
    local isError = message.Level == "Error"
    local isImportantService = string.match(message.Logger, "Payment") or
                               string.match(message.Logger, "Database")

    return isBusinessHour and isError and isImportantService
end

function process(message)
    print("Processing message: " .. message.Index)
    return message
end
```

### Available Message Properties

```lua
message.Index       -- Message number
message.Level       -- "Trace", "Debug", "Info", "Warning", "Error", "Fatal"
message.Logger      -- Logger name
message.Message     -- Log message text
message.Timestamp.Year
message.Timestamp.Month
message.Timestamp.Day
message.Timestamp.Hour
message.Timestamp.Minute
message.Timestamp.Second
```

### Testing Scripts

1. Write your script
2. Click **Run** button
3. Check **Output** panel for errors
4. View results in main log viewer

## Statistics

View log statistics to analyze patterns and distribution.

### Opening Statistics

Click **View → Statistics** to open the statistics dialog.

### Statistics Display

```
┌────────────────────────────────────────────────────────┐
│                  Log Statistics                         │
├────────────────────────────────────────────────────────┤
│  Total Messages: 45,234                                 │
│  Time Range: 2024-01-15 09:00 - 2024-01-15 17:30      │
│  Duration: 8h 30m                                       │
│  Messages/Second: 1.47                                  │
├────────────────────────────────────────────────────────┤
│  Log Levels:                                            │
│                                                         │
│  Error   █████████████████████ 45.2% (20,456)         │
│  Warning ████████████ 25.8% (11,670)                   │
│  Info    ██████ 15.3% (6,921)                          │
│  Debug   ████ 10.1% (4,569)                            │
│  Trace   ██ 3.4% (1,538)                               │
│  Fatal   ▌ 0.2% (80)                                   │
└────────────────────────────────────────────────────────┘
```

**Metrics Shown:**
- Total message count
- First and last message timestamps
- Time range duration
- Messages per second rate
- Breakdown by log level with percentages and counts

## Tips and Tricks

### Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl+N` | New log source |
| `Ctrl+O` | Open file |
| `Ctrl+W` | Close current tab |
| `Ctrl+F` | Find |
| `F3` | Find next |
| `Shift+F3` | Find previous |
| `Ctrl+B` | Add bookmark |
| `F11` | Full screen |
| `Ctrl++` | Zoom in |
| `Ctrl+-` | Zoom out |

### Performance Tips

**For Large Log Files (>100MB):**
1. Uncheck "Start from beginning" when opening
2. Use level filtering to reduce visible messages
3. Use logger tree to focus on specific components
4. Consider splitting large files

**For Real-Time Monitoring:**
1. Enable "Scroll to bottom on new message" (planned feature)
2. Use level filtering to reduce noise
3. Create Lua filters for important messages
4. Use bookmarks to mark significant events

### Multiple Log Sources

**Scenario:** Monitoring multiple services

1. Open each log file as a separate tab
2. Arrange windows side-by-side (planned feature)
3. Use consistent color schemes
4. Synchronize with time shift (planned feature)

### Exporting Logs

**Export Filtered Logs:**
1. Apply filters (level, logger, script)
2. Click **File → Export**
3. Choose format:
   - CSV (for Excel/spreadsheets)
   - Text (original format)
   - JSON (structured data)

**CSV Format:**
```csv
Index,Level,Timestamp,Logger,Message
1,Error,2024-01-15 14:23:45,MyApp.Database,"Connection timeout"
2,Warning,2024-01-15 14:23:46,MyApp.API,"Slow response time"
```

### Dark Mode

Logbert respects your system theme settings:

**Windows:** Settings → Personalization → Colors → Choose your mode
**macOS:** System Preferences → General → Appearance
**Linux:** Depends on desktop environment

### Regular Expressions Quick Reference

**Common Patterns:**

```regex
Exception$              # Lines ending with "Exception"
^Error                  # Lines starting with "Error"
\d{4}-\d{2}-\d{2}      # Date format YYYY-MM-DD
\b\w+Exception\b        # Any word ending in Exception
ERROR|WARN|FATAL        # Multiple alternatives
User \d+                # "User" followed by numbers
```

## Troubleshooting

### Log File Not Updating

**Problem:** New messages don't appear

**Solutions:**
1. Check "Start from beginning" is unchecked
2. Verify log file is being written to
3. Check file permissions
4. Restart Logbert

### Characters Display Incorrectly

**Problem:** Special characters show as �

**Solution:**
1. Close the log source
2. Open again with correct encoding:
   - UTF-8 (most common)
   - Windows-1252 (Western European)
   - ASCII
   - Other encodings available in dropdown

### Performance is Slow

**Problem:** Application is slow with large files

**Solutions:**
1. Use level filtering to reduce visible messages
2. Use logger tree to show only relevant loggers
3. Create Lua filter to show only important messages
4. Consider splitting log files by size or date

### Script Not Working

**Problem:** Lua script doesn't filter messages

**Solutions:**
1. Check Output panel for syntax errors
2. Ensure function is named `filter` or `process`
3. Verify return value is boolean (for filter)
4. Use `print()` for debugging:
   ```lua
   function filter(message)
       print("Level: " .. message.Level)
       return message.Level == "Error"
   end
   ```

## Getting Help

- **Documentation:** Check [docs/](../) folder
- **Issues:** Report bugs on GitHub
- **Discussions:** Ask questions on GitHub Discussions
- **Examples:** See `examples/` folder for sample scripts and configurations

## Next Steps

- Learn about [Log Receiver Configuration](RECEIVERS.md)
- Explore [Lua Scripting Guide](SCRIPTING.md)
- Review [Architecture Documentation](ARCHITECTURE.md) for advanced usage
- Check out [Developer Guide](DEVELOPER_GUIDE.md) if you want to contribute
