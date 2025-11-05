using MoonSharp.Interpreter;
using Couchcoding.Logbert.Logging;
using System;
using System.Text;

namespace Couchcoding.Logbert.Scripting;

/// <summary>
/// Manages Lua script execution using MoonSharp.
/// </summary>
public class ScriptEngine
{
    private Script? _script;
    private readonly StringBuilder _output;

    public string Output => _output.ToString();

    public ScriptEngine()
    {
        _output = new StringBuilder();
    }

    /// <summary>
    /// Loads and compiles a Lua script.
    /// </summary>
    public void LoadScript(string scriptText)
    {
        _output.Clear();

        try
        {
            _script = new Script();

            // Register custom types
            UserData.RegisterType<LogMessage>();
            UserData.RegisterType<LogLevel>();

            // Register print function to capture output
            _script.Globals["print"] = (Action<string>)Print;

            // Load the script
            _script.DoString(scriptText);

            _output.AppendLine("Script loaded successfully");
        }
        catch (ScriptRuntimeException ex)
        {
            _output.AppendLine($"Runtime Error: {ex.DecoratedMessage}");
            throw;
        }
        catch (SyntaxErrorException ex)
        {
            _output.AppendLine($"Syntax Error: {ex.DecoratedMessage}");
            throw;
        }
    }

    /// <summary>
    /// Executes the filter function for a log message.
    /// </summary>
    public bool ExecuteFilter(LogMessage message)
    {
        if (_script == null)
            throw new InvalidOperationException("No script loaded");

        try
        {
            DynValue filterFunc = _script.Globals.Get("filter");

            if (filterFunc == null || filterFunc.IsNil())
            {
                return true; // No filter function, show all messages
            }

            // Call the filter function with the message
            DynValue result = _script.Call(filterFunc, message);

            // Convert result to boolean
            if (result.Type == DataType.Boolean)
            {
                return result.Boolean;
            }

            return true; // Default to showing message
        }
        catch (ScriptRuntimeException ex)
        {
            _output.AppendLine($"Filter Error: {ex.DecoratedMessage}");
            return true; // Show message on error
        }
    }

    /// <summary>
    /// Executes the process function for a log message.
    /// </summary>
    public LogMessage? ExecuteProcess(LogMessage message)
    {
        if (_script == null)
            throw new InvalidOperationException("No script loaded");

        try
        {
            DynValue processFunc = _script.Globals.Get("process");

            if (processFunc == null || processFunc.IsNil())
            {
                return message; // No process function, return original
            }

            // Call the process function with the message
            DynValue result = _script.Call(processFunc, message);

            // Handle result
            if (result.IsNil())
            {
                return null; // Remove message
            }

            if (result.Type == DataType.UserData)
            {
                return result.UserData.Object as LogMessage;
            }

            return message; // Return original on unexpected result
        }
        catch (ScriptRuntimeException ex)
        {
            _output.AppendLine($"Process Error: {ex.DecoratedMessage}");
            return message; // Return original on error
        }
    }

    /// <summary>
    /// Executes a script function by name.
    /// </summary>
    public DynValue ExecuteFunction(string functionName, params object[] args)
    {
        if (_script == null)
            throw new InvalidOperationException("No script loaded");

        try
        {
            DynValue func = _script.Globals.Get(functionName);

            if (func == null || func.IsNil())
            {
                throw new InvalidOperationException($"Function '{functionName}' not found");
            }

            return _script.Call(func, args);
        }
        catch (ScriptRuntimeException ex)
        {
            _output.AppendLine($"Execution Error: {ex.DecoratedMessage}");
            throw;
        }
    }

    /// <summary>
    /// Captures print output from scripts.
    /// </summary>
    private void Print(string message)
    {
        _output.AppendLine(message);
    }

    /// <summary>
    /// Clears the output buffer.
    /// </summary>
    public void ClearOutput()
    {
        _output.Clear();
    }

    /// <summary>
    /// Gets a global variable from the script.
    /// </summary>
    public DynValue GetGlobal(string name)
    {
        if (_script == null)
            throw new InvalidOperationException("No script loaded");

        return _script.Globals.Get(name);
    }

    /// <summary>
    /// Sets a global variable in the script.
    /// </summary>
    public void SetGlobal(string name, object value)
    {
        if (_script == null)
            throw new InvalidOperationException("No script loaded");

        _script.Globals[name] = value;
    }
}
