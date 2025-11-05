using Avalonia.Controls;
using Couchcoding.Logbert.ViewModels.Controls;

namespace Couchcoding.Logbert.Views.Dialogs;

public partial class ScriptEditorDialog : Window
{
    public ScriptEditorDialog()
    {
        InitializeComponent();
        DataContext = new ScriptEditorViewModel();
    }

    public ScriptEditorDialog(ScriptEditorViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
