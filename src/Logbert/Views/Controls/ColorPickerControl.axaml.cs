using Avalonia.Controls;
using Couchcoding.Logbert.ViewModels.Controls;

namespace Couchcoding.Logbert.Views.Controls;

public partial class ColorPickerControl : UserControl
{
    public ColorPickerControl()
    {
        InitializeComponent();
        DataContext = new ColorPickerViewModel();
    }
}
