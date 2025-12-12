using Avalonia.Controls;
using Logbert.ViewModels.Controls;

namespace Logbert.Views.Dialogs;

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
