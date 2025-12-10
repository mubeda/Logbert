using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Logbert.Models;

namespace Logbert.Services;

/// <summary>
/// Service for managing application settings with JSON persistence.
/// </summary>
public class SettingsService
{
    private static SettingsService? _instance;
    private static readonly object _lock = new();

    private AppSettings _settings;
    private readonly string _settingsPath;
    private bool _isDirty;

    /// <summary>
    /// Gets the singleton instance of the SettingsService.
    /// </summary>
    public static SettingsService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new SettingsService();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Gets the current application settings.
    /// </summary>
    public AppSettings Settings => _settings;

    /// <summary>
    /// Occurs when settings are changed.
    /// </summary>
    public event EventHandler? SettingsChanged;

    private SettingsService()
    {
        _settingsPath = GetSettingsFilePath();
        _settings = LoadSettingsFromDisk();
        _isDirty = false;
    }

    /// <summary>
    /// Gets the platform-specific settings file path.
    /// </summary>
    /// <returns>The full path to the settings.json file.</returns>
    private static string GetSettingsFilePath()
    {
        string appDataPath;

        if (OperatingSystem.IsWindows())
        {
            // Windows: %AppData%\Logbert\settings.json
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        else if (OperatingSystem.IsMacOS())
        {
            // macOS: ~/.config/Logbert/settings.json
            appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config");
        }
        else
        {
            // Linux: ~/.config/Logbert/settings.json
            appDataPath = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                ?? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".config");
        }

        string logbertDir = Path.Combine(appDataPath, "Logbert");

        // Ensure directory exists
        if (!Directory.Exists(logbertDir))
        {
            Directory.CreateDirectory(logbertDir);
        }

        return Path.Combine(logbertDir, "settings.json");
    }

    /// <summary>
    /// Loads settings from disk or creates default settings if file doesn't exist.
    /// </summary>
    /// <returns>The loaded or default AppSettings instance.</returns>
    private AppSettings LoadSettingsFromDisk()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var settings = JsonSerializer.Deserialize<AppSettings>(json, options);
                return settings ?? new AppSettings();
            }
        }
        catch (Exception ex)
        {
            // Log error and return defaults
            Console.WriteLine($"Failed to load settings from {_settingsPath}: {ex.Message}");
        }

        return new AppSettings();
    }

    /// <summary>
    /// Saves the current settings to disk.
    /// </summary>
    public void Save()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string json = JsonSerializer.Serialize(_settings, options);
            File.WriteAllText(_settingsPath, json);

            _isDirty = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save settings to {_settingsPath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Saves settings asynchronously.
    /// </summary>
    public async System.Threading.Tasks.Task SaveAsync()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string json = JsonSerializer.Serialize(_settings, options);
            await File.WriteAllTextAsync(_settingsPath, json);

            _isDirty = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save settings to {_settingsPath}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates settings and marks as dirty for auto-save.
    /// </summary>
    /// <param name="updateAction">Action to update settings.</param>
    public void UpdateSettings(Action<AppSettings> updateAction)
    {
        updateAction(_settings);
        _isDirty = true;
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Saves settings if they have been modified (dirty flag set).
    /// </summary>
    public void SaveIfDirty()
    {
        if (_isDirty)
        {
            Save();
        }
    }

    /// <summary>
    /// Resets settings to default values.
    /// </summary>
    public void Reset()
    {
        _settings = new AppSettings();
        Save();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Reloads settings from disk, discarding any unsaved changes.
    /// </summary>
    public void Reload()
    {
        _settings = LoadSettingsFromDisk();
        _isDirty = false;
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
}
