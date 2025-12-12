using Avalonia.Controls;
using Logbert.ViewModels.Controls;

namespace Logbert.Views.Controls;

public partial class ColorPickerControl : UserControl
{
    public ColorPickerControl()
    {
        InitializeComponent();
        DataContext = new ColorPickerViewModel();
    }
}
