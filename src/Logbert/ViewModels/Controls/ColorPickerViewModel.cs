using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Logbert.ViewModels.Controls;

/// <summary>
/// ViewModel for the ColorPicker control.
/// </summary>
public partial class ColorPickerViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ColorItem> _standardColors = new();

    [ObservableProperty]
    private ColorItem? _selectedColor;

    [ObservableProperty]
    private Color _customColor = Colors.Black;

    public IRelayCommand PickCustomColorCommand { get; } = null!;

    public ColorPickerViewModel()
    {
        PickCustomColorCommand = new RelayCommand(OnPickCustomColor);
        InitializeStandardColors();
    }

    private void InitializeStandardColors()
    {
        // Add standard colors
        StandardColors.Add(new ColorItem { Name = "Black", Color = Colors.Black });
        StandardColors.Add(new ColorItem { Name = "White", Color = Colors.White });
        StandardColors.Add(new ColorItem { Name = "Red", Color = Colors.Red });
        StandardColors.Add(new ColorItem { Name = "Green", Color = Colors.Green });
        StandardColors.Add(new ColorItem { Name = "Blue", Color = Colors.Blue });
        StandardColors.Add(new ColorItem { Name = "Yellow", Color = Colors.Yellow });
        StandardColors.Add(new ColorItem { Name = "Orange", Color = Colors.Orange });
        StandardColors.Add(new ColorItem { Name = "Purple", Color = Colors.Purple });
        StandardColors.Add(new ColorItem { Name = "Pink", Color = Colors.Pink });
        StandardColors.Add(new ColorItem { Name = "Gray", Color = Colors.Gray });
        StandardColors.Add(new ColorItem { Name = "Brown", Color = Colors.Brown });
        StandardColors.Add(new ColorItem { Name = "Cyan", Color = Colors.Cyan });
        StandardColors.Add(new ColorItem { Name = "Magenta", Color = Colors.Magenta });
        StandardColors.Add(new ColorItem { Name = "Lime", Color = Colors.Lime });
        StandardColors.Add(new ColorItem { Name = "Navy", Color = Colors.Navy });
        StandardColors.Add(new ColorItem { Name = "Maroon", Color = Colors.Maroon });
    }

    private void OnPickCustomColor()
    {
        // TODO: Show color picker dialog
        // For now, this is a placeholder
    }

    public Color GetSelectedColor()
    {
        return SelectedColor?.Color ?? CustomColor;
    }

    public void SetSelectedColor(Color color)
    {
        var standardColor = StandardColors.FirstOrDefault(c => c.Color == color);
        if (standardColor != null)
        {
            SelectedColor = standardColor;
        }
        else
        {
            CustomColor = color;
        }
    }
}

/// <summary>
/// Represents a color item with name and color.
/// </summary>
public partial class ColorItem : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private Color _color;
}
