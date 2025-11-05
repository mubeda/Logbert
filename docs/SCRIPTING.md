# Logbert Lua Scripting Guide

Logbert includes a powerful Lua scripting engine (MoonSharp) that allows you to filter, transform, and analyze log messages programmatically.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Script Structure](#script-structure)
3. [Filter Function](#filter-function)
4. [Process Function](#process-function)
5. [Message API](#message-api)
6. [Examples](#examples)
7. [Best Practices](#best-practices)
8. [Debugging](#debugging)

## Getting Started

### Opening the Script Editor

1. Click **Tools → Script Editor**
2. The script editor window opens with a default template

### Script Editor Interface

```
┌────────────────────────────────────────────────────┐
│ [New] [Open] [Save] [Run] [Clear]                  │
├────────────────────────────────────────────────────┤
│                                                     │
│  -- Lua Script Editor                              │
│  function filter(message)                          │
│      return true                                   │
│  end                                                │
│                                                     │
├────────────────────────────────────────────────────┤
│ Output:                                             │
│ Script loaded successfully                          │
└────────────────────────────────────────────────────┘
```

**Toolbar Buttons:**
- **New** - Clear editor and start fresh
- **Open** - Load saved script
- **Save** - Save current script
- **Run** - Execute script
- **Clear** - Clear output panel

### Running Your First Script

Try this simple script:

```lua
-- Show only error messages
function filter(message)
    return message.Level == "Error"
end

print("Error filter loaded")
```

1. Copy the script above
2. Click **Run**
3. Check output panel for "Error filter loaded"
4. Return to main window - only error messages are shown

## Script Structure

A Logbert script can define two functions:

### Basic Template

```lua
-- Filter function: return true to show message, false to hide
function filter(message)
    return true  -- Show all messages
end

-- Process function: transform message before display
function process(message)
    return message  -- No transformation
end

-- Initialization code
print("Script loaded successfully")
```

**Execution Order:**
1. Script is loaded and initialized
2. For each log message:
   a. `filter(message)` is called → decides if message is shown
   b. If shown, `process(message)` is called → transforms message
   c. Message is displayed

## Filter Function

### Signature

```lua
function filter(message)
    -- Return true to show message, false to hide it
    return boolean
end
```

### Parameters

- `message` - LogMessage object with properties (see [Message API](#message-api))

### Return Value

- `true` - Message is shown
- `false` - Message is hidden

### Simple Filters

**Show only errors:**
```lua
function filter(message)
    return message.Level == "Error"
end
```

**Hide debug messages:**
```lua
function filter(message)
    return message.Level ~= "Debug"
end
```

**Show messages from specific logger:**
```lua
function filter(message)
    return message.Logger == "MyApp.Database"
end
```

### Complex Filters

**Multiple conditions:**
```lua
function filter(message)
    local isError = message.Level == "Error"
    local isDatabase = string.match(message.Logger, "Database")
    return isError and isDatabase
end
```

**Time-based filter:**
```lua
function filter(message)
    -- Only show messages during business hours
    local hour = message.Timestamp.Hour
    return hour >= 9 and hour <= 17
end
```

**Message content filter:**
```lua
function filter(message)
    -- Show only messages containing "Exception"
    return string.match(message.Message, "Exception") ~= nil
end
```

## Process Function

### Signature

```lua
function process(message)
    -- Modify message and return it
    return message
end
```

### Parameters

- `message` - LogMessage object (modifiable)

### Return Value

- Modified `message` object

### Examples

**Uppercase logger:**
```lua
function process(message)
    message.Logger = string.upper(message.Logger)
    return message
end
```

**Add prefix to message:**
```lua
function process(message)
    message.Message = "[FILTERED] " .. message.Message
    return message
end
```

**Redact sensitive data:**
```lua
function process(message)
    -- Remove email addresses
    message.Message = string.gsub(message.Message, "%S+@%S+", "[EMAIL]")
    -- Remove credit card numbers
    message.Message = string.gsub(message.Message, "%d%d%d%d%-%d%d%d%d%-%d%d%d%d%-%d%d%d%d", "[CARD]")
    return message
end
```

## Message API

### Properties

#### Basic Properties

```lua
message.Index       -- int: Message sequence number
message.Level       -- string: "Trace", "Debug", "Info", "Warning", "Error", "Fatal"
message.Logger      -- string: Logger name (e.g., "MyApp.Service")
message.Message     -- string: Log message text
```

#### Timestamp Properties

```lua
message.Timestamp.Year         -- int: 2024
message.Timestamp.Month        -- int: 1-12
message.Timestamp.Day          -- int: 1-31
message.Timestamp.Hour         -- int: 0-23
message.Timestamp.Minute       -- int: 0-59
message.Timestamp.Second       -- int: 0-59
message.Timestamp.Millisecond  -- int: 0-999
message.Timestamp.Timestamp    -- int: Unix timestamp
```

#### Time Shift Properties

```lua
message.TimeShift.Days         -- int
message.TimeShift.Hours        -- int
message.TimeShift.Minutes      -- int
message.TimeShift.Seconds      -- int
message.TimeShift.Milliseconds -- int
```

#### Raw Data

```lua
message.RawData    -- object: Original data structure (format-specific)
```

### Read-Only vs Modifiable

**Read-Only (in filter):**
- All properties are read-only in `filter()` function
- Used for decision making only

**Modifiable (in process):**
- `message.Logger` - Can be modified
- `message.Message` - Can be modified
- Other properties are effectively read-only (modifying has no effect)

## Examples

### Example 1: Business Hours Error Filter

Show only errors that occurred during business hours:

```lua
function filter(message)
    -- Check if error
    if message.Level ~= "Error" and message.Level ~= "Fatal" then
        return false
    end

    -- Check time
    local hour = message.Timestamp.Hour
    local isBusinessHours = hour >= 9 and hour <= 17

    -- Check weekday (Monday-Friday)
    -- Note: This would require adding DayOfWeek to API
    -- For now, just use hours
    return isBusinessHours
end

print("Business hours error filter active")
```

### Example 2: Critical Service Monitor

Monitor critical services and highlight:

```lua
-- List of critical services
local criticalServices = {
    "Payment",
    "Authentication",
    "Database"
}

function filter(message)
    -- Only show warnings and errors
    if message.Level ~= "Warning" and
       message.Level ~= "Error" and
       message.Level ~= "Fatal" then
        return false
    end

    -- Check if from critical service
    for _, service in ipairs(criticalServices) do
        if string.match(message.Logger, service) then
            return true
        end
    end

    return false
end

function process(message)
    -- Add [CRITICAL] prefix
    message.Message = "[CRITICAL] " .. message.Message
    return message
end

print("Critical service monitor active")
print("Watching: Payment, Authentication, Database")
```

### Example 3: Exception Analyzer

Analyze and categorize exceptions:

```lua
function filter(message)
    return message.Level == "Error" or message.Level == "Fatal"
end

function process(message)
    local msg = message.Message

    -- Categorize exception
    local category = "Unknown"

    if string.match(msg, "NullReference") then
        category = "NULL_REF"
    elseif string.match(msg, "Timeout") then
        category = "TIMEOUT"
    elseif string.match(msg, "Connection") then
        category = "NETWORK"
    elseif string.match(msg, "Permission") or string.match(msg, "Access") then
        category = "SECURITY"
    elseif string.match(msg, "OutOfMemory") then
        category = "MEMORY"
    end

    -- Prepend category
    message.Message = "[" .. category .. "] " .. msg

    return message
end

print("Exception analyzer active")
```

### Example 4: Rate Limiter

Show only first occurrence of repeated messages:

```lua
-- Track seen messages
local seenMessages = {}

function filter(message)
    local key = message.Logger .. "|" .. message.Message

    if seenMessages[key] then
        -- Message already seen, hide it
        return false
    else
        -- First time seeing this message
        seenMessages[key] = true
        return true
    end
end

print("Rate limiter active - showing unique messages only")
```

### Example 5: Performance Tracker

Extract and highlight slow operations:

```lua
function filter(message)
    -- Show messages containing timing information
    return string.match(message.Message, "%d+ms") ~= nil
end

function process(message)
    -- Extract timing
    local timing = string.match(message.Message, "(%d+)ms")

    if timing then
        local ms = tonumber(timing)

        -- Categorize by speed
        if ms > 5000 then
            message.Message = "🔴 VERY SLOW (" .. ms .. "ms): " .. message.Message
        elseif ms > 1000 then
            message.Message = "🟡 SLOW (" .. ms .. "ms): " .. message.Message
        end
    end

    return message
end

print("Performance tracker active")
```

### Example 6: Multi-Pattern Filter

Flexible filter with multiple patterns:

```lua
-- Configuration
local config = {
    showLevels = {"Error", "Fatal", "Warning"},
    includeLoggers = {"Payment", "Database"},
    excludePatterns = {"Heartbeat", "HealthCheck"},
    includePatterns = {"Exception", "Failed", "Timeout"}
}

function filter(message)
    -- Check level
    local levelMatch = false
    for _, level in ipairs(config.showLevels) do
        if message.Level == level then
            levelMatch = true
            break
        end
    end

    if not levelMatch then
        return false
    end

    -- Check logger
    local loggerMatch = false
    for _, logger in ipairs(config.includeLoggers) do
        if string.match(message.Logger, logger) then
            loggerMatch = true
            break
        end
    end

    if not loggerMatch then
        return false
    end

    -- Check exclude patterns
    for _, pattern in ipairs(config.excludePatterns) do
        if string.match(message.Message, pattern) then
            return false
        end
    end

    -- Check include patterns (at least one must match)
    for _, pattern in ipairs(config.includePatterns) do
        if string.match(message.Message, pattern) then
            return true
        end
    end

    return false
end

print("Multi-pattern filter active")
```

## Best Practices

### Performance

1. **Keep filters simple**
   - Complex filters slow down message processing
   - Test with large log files

2. **Avoid expensive operations**
   - Don't read files in filter/process
   - Don't make network calls
   - Minimize string operations

3. **Cache computed values**
   ```lua
   -- Bad: Recomputes every time
   function filter(message)
       return string.match(message.Logger, "Database") ~= nil
   end

   -- Good: Cache result
   function filter(message)
       local hasDatabase = string.match(message.Logger, "Database")
       return hasDatabase ~= nil
   end
   ```

### Debugging

1. **Use print statements**
   ```lua
   function filter(message)
       print("Checking message " .. message.Index)
       print("Level: " .. message.Level)
       print("Logger: " .. message.Logger)

       return message.Level == "Error"
   end
   ```

2. **Check output panel**
   - View print output
   - Look for error messages
   - Verify script loaded

3. **Test incrementally**
   - Start with simple filter
   - Add complexity gradually
   - Test each change

### Code Organization

1. **Use configuration tables**
   ```lua
   local config = {
       levels = {"Error", "Fatal"},
       loggers = {"Payment", "Database"}
   }

   function filter(message)
       -- Use config
   end
   ```

2. **Create helper functions**
   ```lua
   function contains(list, value)
       for _, item in ipairs(list) do
           if item == value then
               return true
           end
       end
       return false
   end

   function filter(message)
       local errorLevels = {"Error", "Fatal"}
       return contains(errorLevels, message.Level)
   end
   ```

3. **Comment your code**
   ```lua
   -- Show only errors from critical services during business hours
   function filter(message)
       -- Check level
       local isError = message.Level == "Error"

       -- Check time
       local hour = message.Timestamp.Hour
       local isBusinessHours = hour >= 9 and hour <= 17

       -- Check logger
       local isCritical = string.match(message.Logger, "Payment") or
                         string.match(message.Logger, "Database")

       return isError and isBusinessHours and isCritical
   end
   ```

## Debugging

### Common Errors

**Error: "attempt to index a nil value"**

Problem: Accessing property that doesn't exist

```lua
-- Wrong
if message.NonExistentProperty == "value" then

-- Correct
if message.Logger == "MyLogger" then
```

**Error: "filter must return boolean"**

Problem: Filter function doesn't return true/false

```lua
-- Wrong
function filter(message)
    message.Level == "Error"  -- Missing return
end

-- Correct
function filter(message)
    return message.Level == "Error"
end
```

**Error: "Script timeout"**

Problem: Infinite loop or very slow operation

```lua
-- Wrong
function filter(message)
    while true do  -- Infinite loop
        -- ...
    end
end

-- Correct: Use simple conditions
function filter(message)
    return message.Level == "Error"
end
```

### Debugging Techniques

**1. Print Everything:**
```lua
function filter(message)
    print("=== Message " .. message.Index .. " ===")
    print("Level: " .. message.Level)
    print("Logger: " .. message.Logger)
    print("Message: " .. message.Message)
    print("Hour: " .. message.Timestamp.Hour)
    print("")

    return true
end
```

**2. Incremental Testing:**
```lua
-- Step 1: Show everything
function filter(message)
    return true
end

-- Step 2: Add level check
function filter(message)
    print("Level: " .. message.Level)
    return message.Level == "Error"
end

-- Step 3: Add logger check
function filter(message)
    local levelOk = message.Level == "Error"
    print("Level OK: " .. tostring(levelOk))

    local loggerOk = string.match(message.Logger, "Database") ~= nil
    print("Logger OK: " .. tostring(loggerOk))

    return levelOk and loggerOk
end
```

**3. Safe Property Access:**
```lua
function filter(message)
    -- Check if property exists
    if message.Logger then
        return string.match(message.Logger, "Database") ~= nil
    end
    return false
end
```

## Lua Reference

### String Functions

```lua
string.match(str, pattern)     -- Find pattern in string
string.find(str, pattern)      -- Find pattern, returns position
string.gsub(str, pattern, rep) -- Replace pattern
string.upper(str)              -- Convert to uppercase
string.lower(str)              -- Convert to lowercase
string.len(str)                -- String length
```

### Table Functions

```lua
table.insert(list, value)      -- Add to list
table.remove(list, index)      -- Remove from list
#list                          -- List length
```

### Conditionals

```lua
if condition then
    -- code
elseif other_condition then
    -- code
else
    -- code
end
```

### Loops

```lua
-- For loop
for i = 1, 10 do
    print(i)
end

-- While loop
while condition do
    -- code
end

-- For each
for index, value in ipairs(list) do
    print(value)
end
```

### Operators

```lua
==    -- Equal
~=    -- Not equal
>     -- Greater than
<     -- Less than
>=    -- Greater or equal
<=    -- Less or equal
and   -- Logical AND
or    -- Logical OR
not   -- Logical NOT
```

## Resources

- [Lua 5.2 Reference](https://www.lua.org/manual/5.2/)
- [MoonSharp Documentation](https://www.moonsharp.org/)
- [Lua String Patterns](https://www.lua.org/pil/20.2.html)
- Example scripts in `examples/scripts/` folder (planned)
