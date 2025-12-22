using System;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Logbert.ViewModels.Docking;

namespace Logbert.Views.Docking;

/// <summary>
/// Dockable panel for the Lua script editor.
/// </summary>
public partial class ScriptPanelView : UserControl
{
    public ScriptPanelView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is ScriptPanelViewModel vm)
        {
            vm.ScriptEditorViewModel.OpenScriptRequested += OnOpenScriptRequested;
            vm.ScriptEditorViewModel.SaveScriptRequested += OnSaveScriptRequested;
        }
    }

    private async void OnOpenScriptRequested(object? sender, EventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Lua Script",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Lua Scripts")
                {
                    Patterns = new[] { "*.lua" }
                },
                new FilePickerFileType("All Files")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (files.Count > 0 && DataContext is ScriptPanelViewModel vm)
        {
            var filePath = files[0].Path.LocalPath;
            vm.ScriptEditorViewModel.LoadFromFile(filePath);
        }
    }

    private async void OnSaveScriptRequested(object? sender, EventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null || DataContext is not ScriptPanelViewModel vm) return;

        // If we have a current file path, save directly
        if (!string.IsNullOrEmpty(vm.ScriptEditorViewModel.CurrentFilePath))
        {
            vm.ScriptEditorViewModel.SaveToFile(vm.ScriptEditorViewModel.CurrentFilePath);
            return;
        }

        // Otherwise, show save dialog
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Lua Script",
            DefaultExtension = "lua",
            SuggestedFileName = vm.ScriptEditorViewModel.FileName,
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Lua Scripts")
                {
                    Patterns = new[] { "*.lua" }
                },
                new FilePickerFileType("All Files")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (file != null)
        {
            var filePath = file.Path.LocalPath;
            vm.ScriptEditorViewModel.SaveToFile(filePath);
        }
    }
}
