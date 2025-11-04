using System;
using System.IO;
using System.Text.Json;
using Dock.Model.Core;

namespace Couchcoding.Logbert.Docking;

/// <summary>
/// Manages saving and loading dock layouts.
/// </summary>
public class DockLayoutManager
{
    private const string LayoutFileName = "dock-layout.json";
    private readonly string _layoutFilePath;

    public DockLayoutManager()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Logbert"
        );

        Directory.CreateDirectory(appDataPath);
        _layoutFilePath = Path.Combine(appDataPath, LayoutFileName);
    }

    /// <summary>
    /// Saves the current dock layout.
    /// </summary>
    public void SaveLayout(IDock? layout)
    {
        if (layout == null)
            return;

        try
        {
            var json = SerializeLayout(layout);
            File.WriteAllText(_layoutFilePath, json);
        }
        catch (Exception ex)
        {
            // Log error but don't crash
            Console.WriteLine($"Failed to save dock layout: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads a previously saved dock layout.
    /// </summary>
    public IDock? LoadLayout(DockFactory factory)
    {
        if (!File.Exists(_layoutFilePath))
            return null;

        try
        {
            var json = File.ReadAllText(_layoutFilePath);
            return DeserializeLayout(json, factory);
        }
        catch (Exception ex)
        {
            // Log error but don't crash
            Console.WriteLine($"Failed to load dock layout: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Checks if a saved layout exists.
    /// </summary>
    public bool HasSavedLayout()
    {
        return File.Exists(_layoutFilePath);
    }

    /// <summary>
    /// Deletes the saved layout.
    /// </summary>
    public void ClearLayout()
    {
        try
        {
            if (File.Exists(_layoutFilePath))
            {
                File.Delete(_layoutFilePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to clear dock layout: {ex.Message}");
        }
    }

    private string SerializeLayout(IDock layout)
    {
        // For now, we'll save basic layout information
        // A more complete implementation would use Dock.Avalonia's serialization
        var layoutData = new DockLayoutData
        {
            Version = "1.0",
            SavedAt = DateTime.UtcNow
        };

        return JsonSerializer.Serialize(layoutData, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private IDock? DeserializeLayout(string json, DockFactory factory)
    {
        // For now, just validate that we can read the layout file
        var layoutData = JsonSerializer.Deserialize<DockLayoutData>(json);

        if (layoutData?.Version != "1.0")
            return null;

        // Return null to use default layout
        // A more complete implementation would reconstruct the layout
        return null;
    }

    private class DockLayoutData
    {
        public string Version { get; set; } = "1.0";
        public DateTime SavedAt { get; set; }
    }
}
