# Logbert Testing Checklist

**Version:** 2.0 (Avalonia .NET 10)
**Last Updated:** December 2025

---

## 🎯 Testing Overview

This checklist provides systematic test procedures for verifying Logbert functionality across all platforms and receiver types.

---

## 📋 Pre-Testing Setup

### Environment Preparation

**Install Required Software:**

- [ ] .NET 10.0 SDK or Runtime
- [ ] Avalonia UI 11.3.8 (included with project)
- [ ] Test log file generators (optional)

**Test Data Preparation:**
```bash
# Create test log directories
mkdir -p ~/logbert-tests/{log4net,nlog,syslog,custom}

# Download sample logs (if available)
# Or generate test logs using provided scripts
```

**Platform-Specific Setup:**

**Windows:**
- [ ] Enable Windows Event Log service
- [ ] Install DebugView (for Debug Output testing)
- [ ] Configure Windows Firewall to allow UDP/TCP receivers

**macOS:**
- [ ] Grant file access permissions
- [ ] Allow incoming network connections

**Linux:**
- [ ] Ensure rsyslog/syslog-ng is running
- [ ] Check firewall rules (ufw/firewalld)
- [ ] Verify /var/log read permissions

---

## 🧪 Receiver Testing

### Test Template

For each receiver, follow this procedure:

#### 1. Configuration Dialog Test
- [ ] Open "New Log Source" dialog
- [ ] Select receiver type
- [ ] Fill in valid configuration
- [ ] Test validation with invalid inputs
- [ ] Save configuration
- [ ] Verify no errors

#### 2. Startup Test
- [ ] Start receiver
- [ ] Check status indicator (should show active)
- [ ] Verify no exceptions in logs
- [ ] Confirm receiver is listening/monitoring

#### 3. Message Reception Test
- [ ] Generate test log messages
- [ ] Verify messages appear in DataGrid
- [ ] Check message parsing (timestamp, level, logger, message)
- [ ] Verify color coding by log level
- [ ] Test with various log levels

#### 4. Real-time Monitoring Test
- [ ] Generate continuous log messages
- [ ] Verify real-time updates (no delay)
- [ ] Check memory usage stays stable
- [ ] Test auto-scroll behavior

#### 5. Stop/Cleanup Test
- [ ] Stop receiver
- [ ] Verify clean shutdown (no errors)
- [ ] Check resources are released
- [ ] Restart receiver to verify repeatability

---

### Log4Net File Receiver

**Configuration:**
```
File Path: /path/to/log4net.xml
Encoding: UTF-8
Start from Beginning: ✓
```

**Test Steps:**
- [ ] Create Log4Net XML log file
- [ ] Configure receiver with file path
- [ ] Start monitoring
- [ ] Append log entries to file
- [ ] Verify real-time updates
- [ ] Test with rotated log files
- [ ] Stop receiver

**Sample Log4Net XML:**
```xml
<log4j:event logger="MyApp.Class" timestamp="1699999999000" level="INFO" thread="1">
  <log4j:message>Test message</log4j:message>
  <log4j:properties>
    <log4j:data name="log4jmachinename" value="MYPC"/>
    <log4j:data name="log4japp" value="MyApp.exe"/>
  </log4j:properties>
</log4j:event>
```

**Expected Results:**
- [x] Messages parsed correctly
- [x] Timestamp displayed properly
- [x] Logger name shown
- [x] Log level correct
- [x] Message content visible

---

### Log4Net Directory Receiver

**Configuration:**
```
Directory: /path/to/log4net-logs/
File Pattern: *.log
Encoding: UTF-8
Start from Beginning: ✓
```

**Test Steps:**
- [ ] Create directory with multiple Log4Net files
- [ ] Configure receiver with directory path
- [ ] Start monitoring
- [ ] Add new log file to directory
- [ ] Append to existing files
- [ ] Verify all files monitored
- [ ] Stop receiver

**Test Cases:**
- [ ] Single file in directory
- [ ] Multiple files (5+)
- [ ] New file added while monitoring
- [ ] File deleted while monitoring
- [ ] Pattern matching (*.log, *-2024-*.xml)

---

### Log4Net UDP Receiver

**Configuration:**
```
Port: 8080
Listen Interface: 0.0.0.0
Multicast IP: (optional)
Encoding: UTF-8
```

**Test Steps:**
- [ ] Configure receiver with port 8080
- [ ] Start receiver
- [ ] Send Log4Net UDP messages using test tool
- [ ] Verify messages received
- [ ] Test with multicast (optional)
- [ ] Stop receiver

**Test Tool (PowerShell):**
```powershell
$udp = New-Object System.Net.Sockets.UdpClient
$bytes = [System.Text.Encoding]::UTF8.GetBytes("<log4j:event>...</log4j:event>")
$udp.Send($bytes, $bytes.Length, "localhost", 8080)
$udp.Close()
```

**Test Tool (bash):**
```bash
echo "<log4j:event>...</log4j:event>" | nc -u localhost 8080
```

---

### NLog File Receiver

**Configuration:**
```
File Path: /path/to/nlog.xml
Encoding: UTF-8
Start from Beginning: ✓
```

**Test Steps:**
- [ ] Create NLog XML log file
- [ ] Configure receiver with file path
- [ ] Start monitoring
- [ ] Append log entries
- [ ] Verify parsing
- [ ] Stop receiver

**Sample NLog XML:**
```xml
<log4net:event logger="MyApp" timestamp="2024-11-06T10:30:00" level="INFO" thread="1">
  <log4net:message>Test NLog message</log4net:message>
</log4net:event>
```

---

### NLog Directory Receiver

**Configuration:**
```
Directory: /path/to/nlog-logs/
File Pattern: *.nlog
Encoding: UTF-8
Start from Beginning: ✓
```

**Test Steps:**
- [ ] Follow same procedure as Log4Net Dir receiver
- [ ] Test with NLog-formatted files
- [ ] Verify pattern matching

---

### NLog TCP Receiver

**Configuration:**
```
Port: 4505
Listen Interface: 0.0.0.0
Encoding: UTF-8
```

**Test Steps:**
- [ ] Configure receiver with port 4505
- [ ] Start receiver
- [ ] Connect TCP client and send NLog XML
- [ ] Verify messages received
- [ ] Test multiple simultaneous connections
- [ ] Test connection close/reconnect
- [ ] Stop receiver

**Test Tool (telnet):**
```bash
telnet localhost 4505
# Paste NLog XML message
```

---

### NLog UDP Receiver

**Configuration:**
```
Port: 9999
Listen Interface: 0.0.0.0
Multicast IP: (optional)
Encoding: UTF-8
```

**Test Steps:**
- [ ] Same as Log4Net UDP
- [ ] Use port 9999
- [ ] Send NLog-formatted messages

---

### Syslog File Receiver

**Configuration:**
```
File Path: /var/log/syslog (Linux) or /path/to/syslog.log
Timestamp Format: MMM dd HH:mm:ss
Encoding: UTF-8
Start from Beginning: ✓
```

**Test Steps:**
- [ ] Configure with syslog file
- [ ] Start monitoring
- [ ] Append syslog messages
- [ ] Verify RFC 3164 parsing
- [ ] Test various facilities and severities
- [ ] Stop receiver

**Sample Syslog (RFC 3164):**
```
<34>Oct 11 22:14:15 mymachine su: 'su root' failed for user on /dev/pts/8
<13>Oct 11 22:14:15 mymachine kernel: eth0: link down
```

---

### Syslog UDP Receiver

**Configuration:**
```
Port: 514
Listen Interface: 0.0.0.0
Multicast IP: (optional)
Timestamp Format: MMM dd HH:mm:ss
Encoding: UTF-8
```

**Test Steps:**
- [ ] Configure receiver with port 514 (or 5514 if permissions)
- [ ] Start receiver
- [ ] Send syslog UDP messages
- [ ] Verify priority matrix parsing
- [ ] Test all 8 severities
- [ ] Test all 24 facilities
- [ ] Stop receiver

**Test Tool (logger command on Linux):**
```bash
logger -n localhost -P 514 "Test syslog message"
```

---

### Windows Event Log Receiver

**Platform:** Windows Only

**Configuration:**
```
Log Name: Application / System / Security / Custom
Machine: . (local) or remote computer
Source Filter: (optional)
```

**Test Steps:**
- [ ] Configure receiver for Application log
- [ ] Start receiver
- [ ] Generate test events:
  ```powershell
  Write-EventLog -LogName Application -Source "Logbert Test" -EventID 1000 -EntryType Information -Message "Test event"
  ```
- [ ] Verify events appear
- [ ] Test filtering by source
- [ ] Test remote computer (if available)
- [ ] Stop receiver

**Test Cases:**
- [ ] Application log
- [ ] System log
- [ ] Security log (requires admin)
- [ ] Custom event log
- [ ] Remote computer event log
- [ ] Filter by event source

---

### Windows Debug Output Receiver

**Platform:** Windows Only

**Configuration:**
```
Capture Mode: All Processes / Specific Process
Process ID: (if specific)
```

**Test Steps:**
- [ ] Configure receiver for all processes
- [ ] Start receiver
- [ ] Run test application that outputs debug strings:
  ```csharp
  System.Diagnostics.Debug.WriteLine("Test debug message");
  ```
- [ ] Verify debug messages captured
- [ ] Test with specific process ID
- [ ] Stop receiver

**Test Tool:**
- Use DebugView from Sysinternals
- Or create simple C# app with Debug.WriteLine

---

### Custom File Receiver

**Configuration:**
```
File Path: /path/to/custom.log
Encoding: UTF-8
Start from Beginning: ✓

Columnizer:
  Name: Custom Parser
  DateTime Format: yyyy-MM-dd HH:mm:ss.fff
  Columns:
    - Timestamp: (\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3})
    - Level: (TRACE|DEBUG|INFO|WARN|ERROR|FATAL)
    - Logger: \[([^\]]+)\]
    - Message: (.+)
  Log Level Mapping:
    Trace: TRACE
    Debug: DEBUG
    Info: INFO
    Warning: WARN
    Error: ERROR
    Fatal: FATAL
```

**Test Steps:**
- [ ] Create custom format log file
- [ ] Configure receiver with Columnizer
- [ ] Test regex patterns in Columnizer editor
- [ ] Start monitoring
- [ ] Verify custom parsing works
- [ ] Test with various log formats
- [ ] Stop receiver

**Sample Custom Log:**
```
2024-11-06 10:30:45.123 INFO [MyApp.Service] Application started
2024-11-06 10:30:46.456 DEBUG [MyApp.Database] Connection established
2024-11-06 10:30:47.789 ERROR [MyApp.API] Request failed: timeout
```

**Test Cases:**
- [ ] Simple format (timestamp level message)
- [ ] Complex format with multiple fields
- [ ] Multi-line messages
- [ ] Different datetime formats
- [ ] Unicode characters
- [ ] Very long messages (>1000 chars)

---

### Custom Directory Receiver

**Configuration:**
```
Directory: /path/to/custom-logs/
File Pattern: *.custom
Encoding: UTF-8
Start from Beginning: ✓
Columnizer: (same as Custom File)
```

**Test Steps:**
- [ ] Follow Custom File procedure
- [ ] Test with multiple files
- [ ] Test pattern matching
- [ ] Verify directory monitoring

---

### Custom UDP Receiver

**Configuration:**
```
Port: 10000
Listen Interface: 0.0.0.0
Multicast IP: (optional)
Encoding: UTF-8
Columnizer: (configured)
```

**Test Steps:**
- [ ] Configure receiver with custom columnizer
- [ ] Start receiver
- [ ] Send custom format messages via UDP
- [ ] Verify parsing with Columnizer
- [ ] Test multicast (optional)
- [ ] Stop receiver

**Test Tool:**
```bash
echo "2024-11-06 10:30:45.123 INFO [Test] UDP message" | nc -u localhost 10000
```

---

### Custom TCP Receiver

**Configuration:**
```
Port: 10001
Listen Interface: 0.0.0.0
Encoding: UTF-8
Columnizer: (configured)
```

**Test Steps:**
- [ ] Configure receiver with custom columnizer
- [ ] Start receiver
- [ ] Connect TCP client
- [ ] Send custom format messages
- [ ] Test persistent connections
- [ ] Test reconnection
- [ ] Stop receiver

---

### Custom HTTP Receiver

**Configuration:**
```
URL: http://localhost:8000/logs
Poll Interval: 10 seconds
Authentication:
  ☑ Enable Basic Auth
  Username: admin
  Password: password
Encoding: UTF-8
Columnizer: (configured)
```

**Test Steps:**
- [ ] Set up HTTP server serving log data
- [ ] Configure receiver with URL
- [ ] Start receiver
- [ ] Verify periodic polling (every 10 seconds)
- [ ] Test authentication
- [ ] Test with invalid credentials
- [ ] Test with server down
- [ ] Test with large responses
- [ ] Stop receiver

**Test Server (Python):**
```python
from http.server import HTTPServer, BaseHTTPRequestHandler
import base64

class LogHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        auth = self.headers.get('Authorization')
        if auth:
            encoded = auth.split(' ')[1]
            decoded = base64.b64decode(encoded).decode()
            if decoded == "admin:password":
                self.send_response(200)
                self.end_headers()
                self.wfile.write(b"2024-11-06 10:30:45.123 INFO [API] Log entry\n")
                return
        self.send_response(401)
        self.send_header('WWW-Authenticate', 'Basic realm="Logs"')
        self.end_headers()

HTTPServer(('', 8000), LogHandler).serve_forever()
```

---

## 🔍 Search & Filter Testing

### Search Functionality

**Test Cases:**
1. **Simple Text Search**
   - [ ] Search for "error"
   - [ ] Find next/previous
   - [ ] Verify wrap-around
   - [ ] Check match counter

2. **Case-Sensitive Search**
   - [ ] Search for "Error" (case-sensitive on)
   - [ ] Verify "error" not matched
   - [ ] Search for "ERROR" (case-sensitive on)

3. **Whole Word Search**
   - [ ] Search for "log" (whole word on)
   - [ ] Verify "logger" not matched
   - [ ] Verify "log" matched

4. **Regular Expression Search**
   - [ ] Search for `\berror\b` (word boundary)
   - [ ] Search for `\d{3}-\d{3}-\d{4}` (phone number)
   - [ ] Search for `\[.*?\]` (bracketed text)
   - [ ] Test invalid regex (should show error)

5. **Search History**
   - [ ] Perform multiple searches
   - [ ] Verify history dropdown shows last 10
   - [ ] Select from history

6. **Search in Filtered Results**
   - [ ] Apply log level filter
   - [ ] Perform search
   - [ ] Verify search only in filtered messages

---

### Filter Testing

**Level Filtering:**
- [ ] Show only ERROR level
- [ ] Show ERROR and FATAL
- [ ] Show all except TRACE
- [ ] Clear filters

**Logger Filtering:**
- [ ] Filter by single logger name
- [ ] Filter by logger prefix
- [ ] Filter with wildcard pattern
- [ ] Clear filter

**Combined Filters:**
- [ ] Level + Logger filter
- [ ] Level + Search filter
- [ ] All filters combined

---

## 📊 Statistics Testing

**Test Cases:**
- [ ] Open statistics dialog
- [ ] Verify total message count
- [ ] Check breakdown by level
- [ ] Verify percentages add to 100%
- [ ] Check time range (first/last message)
- [ ] Verify messages/second calculation
- [ ] Test with empty log
- [ ] Test with single message
- [ ] Test with filtered results

---

## 🎨 UI/UX Testing

### Window Management
- [ ] Resize window (minimum, maximum, custom)
- [ ] Move window
- [ ] Minimize/restore
- [ ] Maximize/restore
- [ ] Close and reopen

### Panel Resizing
- [ ] Resize left panel (filter/bookmarks/logger tree)
- [ ] Resize right panel (if present)
- [ ] Collapse/expand panels
- [ ] Verify splitter positions saved (if persistence implemented)

### Keyboard Shortcuts
- [ ] Ctrl+N - New log source
- [ ] Ctrl+O - Open (if applicable)
- [ ] Ctrl+F - Find/Search
- [ ] Ctrl+W - Close document
- [ ] F5 - Refresh
- [ ] Esc - Close dialog

### Theme Testing
- [ ] Light mode rendering
- [ ] Dark mode rendering (if supported)
- [ ] System theme following (if supported)
- [ ] Color consistency across dialogs

---

## ⚡ Performance Testing

### Large File Tests

**100K Messages:**
- [ ] Load time < 3 seconds
- [ ] Scrolling smooth (>30 FPS)
- [ ] Search time < 1 second
- [ ] Memory usage < 100MB

**1M Messages:**
- [ ] Load time < 10 seconds
- [ ] Scrolling smooth (>30 FPS)
- [ ] Search time < 3 seconds
- [ ] Memory usage < 500MB

**10M Messages:**
- [ ] Load without crash
- [ ] Monitor memory usage
- [ ] Test virtualization working

### Stress Tests

**Continuous Monitoring:**
- [ ] Monitor log file for 1 hour
- [ ] Append 1000 messages/minute
- [ ] Verify no memory leaks
- [ ] Check CPU usage stays reasonable

**Multiple Receivers:**
- [ ] Open 5 different receivers simultaneously
- [ ] Verify all receive messages
- [ ] Check resource usage
- [ ] No cross-contamination

---

## 🐛 Error Handling

### Invalid Configurations
- [ ] Empty file path
- [ ] Non-existent file
- [ ] Invalid port number (0, 65536)
- [ ] Invalid IP address
- [ ] Invalid regex pattern
- [ ] Invalid URL format

### Runtime Errors
- [ ] File deleted while monitoring
- [ ] File permissions changed
- [ ] Network connection lost
- [ ] Server shutdown (network receivers)
- [ ] Disk full
- [ ] Out of memory

### Error Message Quality
- [ ] Error messages are clear
- [ ] Suggest solutions when possible
- [ ] Don't expose technical details to end users
- [ ] Log detailed errors for debugging

---

## ✅ Sign-Off

**Tester Information:**
- Name: _________________
- Date: _________________
- Platform: _________________
- Version: _________________

**Overall Assessment:**
- [ ] All critical tests passed
- [ ] No critical bugs found
- [ ] Performance acceptable
- [ ] Ready for release

**Notes:**
_______________________________________________________________
_______________________________________________________________
_______________________________________________________________

---

**End of Testing Checklist**
