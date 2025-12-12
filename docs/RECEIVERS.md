# Log Receiver Configuration Guide

This guide explains how to configure different types of log receivers in Logbert.

## Table of Contents

1. [Log4Net Receivers](#log4net-receivers)
2. [NLog Receivers](#nlog-receivers)
3. [Syslog Receivers](#syslog-receivers)
4. [Custom Receivers](#custom-receivers)
5. [Windows Event Log](#windows-event-log)
6. [Troubleshooting](#troubleshooting)

## Log4Net Receivers

### Log4Net File Receiver

Monitors a Log4Net XML log file for new messages.

#### Configuration

1. Click **File → New** or press `Ctrl+N`
2. Select **Log4Net File**
3. Configure settings:
   - **Log File Path**: Path to your Log4Net XML file
   - **Start from beginning**: Load existing messages (checked) or only new ones (unchecked)
   - **Text Encoding**: Select file encoding (default: UTF-8)

#### Log4Net Appender Configuration

Configure your Log4Net appender to use XML output:

```xml
<log4net>
  <appender name="XmlFileAppender" type="log4net.Appender.RollingFileAppender">
    <file value="logs/application.log" />
    <appendToFile value="true" />
    <rollingStyle value="Size" />
    <maxSizeRollBackups value="10" />
    <maximumFileSize value="10MB" />
    <staticLogFileName value="true" />

    <!-- Use XMLLayoutSchemaLog4j layout -->
    <layout type="log4net.Layout.XmlLayoutSchemaLog4j, log4net.Layout.XmlLayoutSchemaLog4j">
      <locationInfo value="true" />
    </layout>
  </appender>

  <root>
    <level value="DEBUG" />
    <appender-ref ref="XmlFileAppender" />
  </root>
</log4net>
```

#### Expected Format

```xml
<log4j:event logger="MyApp.Service" timestamp="1705334425000" level="ERROR" thread="10">
  <log4j:message>An error occurred</log4j:message>
  <log4j:throwable>System.NullReferenceException: Object reference not set...</log4j:throwable>
  <log4j:locationInfo class="MyApp.Service" method="ProcessData" file="Service.cs" line="42"/>
  <log4j:properties>
    <log4j:data name="User" value="admin"/>
  </log4j:properties>
</log4j:event>
```

#### Displayed Columns

| Column | Description | Example |
|--------|-------------|---------|
| Number | Message sequence | 1, 2, 3, ... |
| Level | Log level | ERROR, WARN, INFO |
| Timestamp | When logged | 2024-01-15 14:23:45 |
| Logger | Logger name | MyApp.Service |
| Thread | Thread ID | 10 |
| Message | Log message | An error occurred |

### Log4Net UDP Receiver

Receives Log4Net messages over UDP network.

#### Configuration

1. Select **Log4Net UDP**
2. Configure:
   - **Port**: UDP port to listen on (default: 8080)
   - **Multicast Group**: (Optional) Multicast IP address
   - **Buffer Size**: Receive buffer size (default: 8192)

#### Log4Net Appender Configuration

```xml
<appender name="UdpAppender" type="log4net.Appender.UdpAppender">
  <remoteAddress value="127.0.0.1" />
  <remotePort value="8080" />
  <layout type="log4net.Layout.XmlLayoutSchemaLog4j" />
</appender>
```

## NLog Receivers

### NLog File Receiver

Monitors an NLog XML log file.

#### Configuration

1. Select **NLog File**
2. Configure same as Log4Net File (path, encoding, start position)

#### NLog Target Configuration

```xml
<nlog>
  <targets>
    <target name="xmlfile"
            xsi:type="File"
            fileName="${basedir}/logs/app.log">
      <!-- Use Log4JXmlEventLayout -->
      <layout xsi:type="Log4JXmlEventLayout" />
    </target>
  </targets>

  <rules>
    <logger name="*" minlevel="Debug" writeTo="xmlfile" />
  </rules>
</nlog>
```

#### Expected Format

Same as Log4Net XML format (`<log4j:event>`).

### NLog TCP Receiver

Receives NLog messages over TCP network.

#### Configuration

1. Select **NLog TCP**
2. Configure:
   - **Port**: TCP port to listen on
   - **Keep Connection Alive**: Maintain persistent connections

#### NLog Target Configuration

```xml
<target name="tcpTarget"
        xsi:type="Network"
        address="tcp://127.0.0.1:4505">
  <layout xsi:type="Log4JXmlEventLayout" />
</target>
```

### NLog UDP Receiver

Receives NLog messages over UDP network.

#### Configuration

Same as NLog TCP but uses UDP protocol.

```xml
<target name="udpTarget"
        xsi:type="Network"
        address="udp://127.0.0.1:4505">
  <layout xsi:type="Log4JXmlEventLayout" />
</target>
```

## Syslog Receivers

### Syslog File Receiver

Reads Syslog files in RFC 3164 format.

#### Configuration

1. Select **Syslog File**
2. Configure file path and encoding

#### Expected Format

```
<34>Jan 15 14:23:45 myhost MyApp: Connection timeout
<165>Jan 15 14:23:46 myhost MyApp: User login failed
```

**Priority Format:** `<Priority>Timestamp Hostname Application: Message`

**Priority Calculation:** `Priority = (Facility × 8) + Severity`

#### Severity Levels

| Code | Name | Description |
|------|------|-------------|
| 0 | Emergency | System unusable |
| 1 | Alert | Immediate action required |
| 2 | Critical | Critical conditions |
| 3 | Error | Error conditions |
| 4 | Warning | Warning conditions |
| 5 | Notice | Normal but significant |
| 6 | Informational | Informational messages |
| 7 | Debug | Debug messages |

#### Facility Codes

| Code | Facility | Description |
|------|----------|-------------|
| 0 | kern | Kernel messages |
| 1 | user | User-level messages |
| 2 | mail | Mail system |
| 3 | daemon | System daemons |
| 4 | auth | Security/auth messages |
| 16 | local0 | Local use 0 |
| 17 | local1 | Local use 1 |
| 23 | local7 | Local use 7 |

### Syslog UDP Receiver

Receives Syslog messages over UDP.

#### Configuration

1. Select **Syslog UDP**
2. Configure:
   - **Port**: UDP port (default: 514)
   - **Protocol Version**: RFC 3164 or RFC 5424

#### Syslog Configuration (rsyslog)

```conf
# Forward to Logbert
*.* @127.0.0.1:514
```

## Custom Receivers

Custom receivers allow parsing of proprietary or non-standard log formats.

### Custom File Receiver

Parses custom format log files using configurable patterns.

#### Configuration

1. Select **Custom File**
2. Configure file path
3. Define **Columnizer** pattern

#### Columnizer Configuration

Define how to parse each log line using regex:

**Example: Apache Access Log**

Pattern:
```regex
^(?<IP>[\d.]+) - - \[(?<Timestamp>[^\]]+)\] "(?<Method>\w+) (?<Path>[^\s]+) HTTP/[\d.]+" (?<Status>\d+) (?<Size>\d+)
```

Columns:
- IP → Column 0
- Timestamp → Column 1
- Method → Column 2
- Path → Column 3
- Status → Column 4
- Size → Column 5

**Example: Custom Application Log**

Log format:
```
2024-01-15 14:23:45 [ERROR] MyApp.Service: Connection failed
```

Pattern:
```regex
^(?<Timestamp>[\d-]+ [\d:]+) \[(?<Level>\w+)\] (?<Logger>[\w.]+): (?<Message>.+)$
```

### Custom UDP/TCP Receivers

Receive custom format logs over network.

#### Configuration

Similar to standard network receivers but with custom parsing patterns.

### Custom HTTP Receiver

Receives logs via HTTP POST requests.

#### Configuration

1. Select **Custom HTTP**
2. Configure:
   - **Port**: HTTP listening port
   - **Path**: Endpoint path (e.g., `/logs`)
   - **Authentication**: Basic auth settings
   - **Format**: JSON, XML, or custom

#### Example Client

```bash
curl -X POST http://localhost:9000/logs \
  -H "Content-Type: application/json" \
  -d '{
    "timestamp": "2024-01-15T14:23:45Z",
    "level": "ERROR",
    "logger": "MyApp",
    "message": "Something went wrong"
  }'
```

## Windows Event Log

**Platform:** Windows only

Reads from Windows Event Log system.

### Configuration

1. Select **Windows Event Log**
2. Configure:
   - **Log Name**: Application, System, Security, or custom
   - **Source Filter**: (Optional) Filter by event source
   - **Level Filter**: Minimum level to show

### Event Log Names

- **Application**: Application events
- **System**: System events
- **Security**: Security audit events
- **Setup**: Setup events
- Custom logs created by applications

### Example

Reading Application log, only errors from "MyApp":

- Log Name: `Application`
- Source Filter: `MyApp`
- Level Filter: `Error`

## Troubleshooting

### File Receiver Not Updating

**Problem:** New messages don't appear

**Checklist:**
- [ ] File is being written to (check timestamp)
- [ ] File encoding matches configuration
- [ ] "Start from beginning" is unchecked for new messages
- [ ] File permissions allow reading
- [ ] File is not locked by another process

**Solution:**
```bash
# Check if file is being updated
tail -f /path/to/logfile

# Check file permissions
ls -l /path/to/logfile
```

### Network Receiver Not Receiving

**Problem:** No messages received over network

**Checklist:**
- [ ] Port is not already in use
- [ ] Firewall allows incoming connections
- [ ] Correct IP address/port configuration
- [ ] Sender is actually transmitting

**Test Connection:**
```bash
# Test UDP
echo "test" | nc -u localhost 8080

# Test TCP
echo "test" | nc localhost 8080

# Check listening ports
netstat -an | grep LISTEN
```

### Malformed Messages

**Problem:** Messages appear garbled or incomplete

**Solutions:**

1. **Wrong Encoding**
   - Try different encoding (UTF-8, Windows-1252, ASCII)
   - Check source application encoding

2. **Wrong Format**
   - Verify XML structure for Log4Net/NLog
   - Check Syslog RFC compliance
   - Validate custom regex pattern

3. **Buffering Issues**
   - Increase buffer size
   - Check for message truncation
   - Verify network MTU for UDP

### Performance Issues

**Problem:** Slow performance with large files

**Solutions:**

1. **Filter aggressively**
   - Use level filtering
   - Use logger tree filtering
   - Create Lua filter script

2. **Start position**
   - Uncheck "Start from beginning"
   - Only load recent messages

3. **File splitting**
   - Use rolling file appenders
   - Limit file size to <100MB

## Best Practices

### For Developers

1. **Always use structured logging**
   - XML formats (Log4Net, NLog) are better than plain text
   - Include context (logger, thread, timestamp)

2. **Include location information**
   - Class, method, file, line number
   - Helps debugging

3. **Use appropriate log levels**
   - Trace: Very detailed debugging
   - Debug: Diagnostic information
   - Info: General information
   - Warning: Potential issues
   - Error: Errors and exceptions
   - Fatal: Critical failures

4. **Add correlation IDs**
   - Track requests across components
   - Include in log context

### For Operations

1. **Centralize logs**
   - Use network receivers for central monitoring
   - Aggregate from multiple services

2. **Rotate log files**
   - Prevent disk space issues
   - Use size-based or time-based rotation

3. **Monitor in real-time**
   - Keep Logbert running during operations
   - Watch for errors and warnings

4. **Set up filters**
   - Create Lua scripts for critical alerts
   - Use bookmarks for important events

## Advanced Configurations

### Multiple Sources

Monitor multiple log sources simultaneously:

1. Open each source in a separate tab
2. Name tabs descriptively
3. Use logger tree to identify sources
4. Export combined logs if needed

### Log Aggregation

Combine logs from multiple servers:

1. Configure each server to send to Logbert
2. Use network receivers (UDP/TCP)
3. Differentiate by logger name or custom field
4. Use Lua scripts to add server identifiers

### Remote Monitoring

Monitor logs from remote machines:

**Option 1: File Share**
- Share log directory via SMB/NFS
- Open file over network
- Higher latency than local

**Option 2: Network Receiver**
- Configure app to send over network
- Better for real-time
- Works across WAN

**Option 3: Log Forwarding**
- Use syslog forwarding
- rsyslog, syslog-ng, etc.
- Standard protocol

## Example Configurations

### ASP.NET Core Application

```csharp
// Program.cs
builder.Logging.AddLog4Net("log4net.config");
```

```xml
<!-- log4net.config -->
<log4net>
  <appender name="XmlFile" type="log4net.Appender.RollingFileAppender">
    <file value="logs/webapp.log" />
    <appendToFile value="true" />
    <rollingStyle value="Date" />
    <datePattern value="yyyyMMdd" />
    <layout type="log4net.Layout.XmlLayoutSchemaLog4j" />
  </appender>
  <root>
    <level value="INFO" />
    <appender-ref ref="XmlFile" />
  </root>
</log4net>
```

### Python Application (logging library)

```python
import logging
from logging.handlers import SysLogHandler

logger = logging.getLogger()
handler = SysLogHandler(address=('localhost', 514))
logger.addHandler(handler)
logger.setLevel(logging.INFO)

logger.info("Application started")
```

### Docker Container Logs

```yaml
# docker-compose.yml
version: '3'
services:
  app:
    image: myapp
    logging:
      driver: syslog
      options:
        syslog-address: "udp://localhost:514"
        tag: "myapp"
```

## References

- [Log4Net Documentation](https://logging.apache.org/log4net/)
- [NLog Documentation](https://nlog-project.org/)
- [RFC 3164 (Syslog)](https://tools.ietf.org/html/rfc3164)
- [RFC 5424 (Syslog Protocol)](https://tools.ietf.org/html/rfc5424)
