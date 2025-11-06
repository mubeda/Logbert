using Avalonia.Controls;
using Couchcoding.Logbert.ViewModels.Controls;

namespace Couchcoding.Logbert.Views.Controls;

public partial class ColumnizerEditorControl : UserControl
{
    public ColumnizerEditorViewModel? ViewModel => DataContext as ColumnizerEditorViewModel;

    public ColumnizerEditorControl()
    {
        InitializeComponent();
        DataContext = new ColumnizerEditorViewModel();
    }
}
