using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.ViewModels.Controls;

namespace Logbert.ViewModels.Docking;

/// <summary>
/// ViewModel for the dockable script editor panel.
/// </summary>
public partial class ScriptPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    private ScriptEditorViewModel _scriptEditorViewModel = new();

    /// <summary>
    /// Gets the cursor position text.
    /// </summary>
    public string CursorPositionText =>
        $"Ln {ScriptEditorViewModel.CursorLine}, Col {ScriptEditorViewModel.CursorColumn}";

    public ScriptPanelViewModel()
    {
        ScriptEditorViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ScriptEditorViewModel.CursorLine) ||
                e.PropertyName == nameof(ScriptEditorViewModel.CursorColumn))
            {
                OnPropertyChanged(nameof(CursorPositionText));
            }
        };
    }

    /// <summary>
    /// Gets the script editor view model.
    /// </summary>
    public ScriptEditorViewModel GetScriptEditor() => ScriptEditorViewModel;
}
