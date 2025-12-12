using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Logbert.Models;
using Logbert.ViewModels.Docking;
using Logbert.Views.Dialogs;

namespace Logbert.Views.Docking;

public partial class FilterPanelView : UserControl
{
    public FilterPanelView()
    {
        InitializeComponent();

        // Add converters to resources
        Resources["AdvancedToggleConverter"] = new AdvancedToggleConverter();
        Resources["InverseBoolConverter"] = new InverseBoolConverter();
        Resources["ZeroToBoolConverter"] = new ZeroToBoolConverter();

        // Subscribe to ViewModel events when DataContext is set
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is FilterPanelViewModel vm)
        {
            vm.AddFilterRequested += OnAddFilterRequested;
            vm.EditFilterRequested += OnEditFilterRequested;
            vm.ImportFiltersRequested += OnImportFiltersRequested;
            vm.ExportFiltersRequested += OnExportFiltersRequested;
        }
    }

    private void OnToggleAdvancedFilters(object? sender, RoutedEventArgs e)
    {
        if (DataContext is FilterPanelViewModel vm)
        {
            vm.ShowAdvancedFilters = !vm.ShowAdvancedFilters;
        }
    }

    private async void OnAddFilterRequested(object? sender, EventArgs e)
    {
        var dialog = new FilterEditorDialog();
        dialog.InitializeForAdd();

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is Window window)
        {
            var result = await dialog.ShowDialog<bool?>(window);
            if (result == true && dialog.ResultRule != null && DataContext is FilterPanelViewModel vm)
            {
                vm.AddFilterRule(dialog.ResultRule);
            }
        }
    }

    private async void OnEditFilterRequested(object? sender, FilterRule rule)
    {
        var dialog = new FilterEditorDialog();
        dialog.InitializeForEdit(rule);

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is Window window)
        {
            var result = await dialog.ShowDialog<bool?>(window);
            if (result == true && dialog.ResultRule != null && DataContext is FilterPanelViewModel vm)
            {
                vm.UpdateFilterRule(rule, dialog.ResultRule);
            }
        }
    }

    private async void OnImportFiltersRequested(object? sender, EventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import Filter Rules",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("JSON Files")
                {
                    Patterns = new[] { "*.json" }
                },
                new FilePickerFileType("All Files")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (files.Count > 0 && DataContext is FilterPanelViewModel vm)
        {
            var filePath = files[0].Path.LocalPath;
            var success = await vm.LoadFiltersFromFileAsync(filePath);
            if (!success && topLevel is Window window)
            {
                // Show error dialog
                var errorDialog = new Window
                {
                    Title = "Import Error",
                    Width = 350,
                    Height = 130,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    Content = new StackPanel
                    {
                        Margin = new Avalonia.Thickness(20),
                        Spacing = 15,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = "Failed to import filter rules. The file may be invalid or corrupted.",
                                TextWrapping = Avalonia.Media.TextWrapping.Wrap
                            },
                            new Button
                            {
                                Content = "OK",
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                Padding = new Avalonia.Thickness(30, 8)
                            }
                        }
                    }
                };
                await errorDialog.ShowDialog(window);
            }
        }
    }

    private async void OnExportFiltersRequested(object? sender, EventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Filter Rules",
            DefaultExtension = "json",
            SuggestedFileName = "logbert_filters.json",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("JSON Files")
                {
                    Patterns = new[] { "*.json" }
                },
                new FilePickerFileType("All Files")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (file != null && DataContext is FilterPanelViewModel vm)
        {
            try
            {
                await vm.SaveFiltersToFileAsync(file.Path.LocalPath);
            }
            catch (Exception ex)
            {
                if (topLevel is Window window)
                {
                    var errorDialog = new Window
                    {
                        Title = "Export Error",
                        Width = 350,
                        Height = 130,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        CanResize = false,
                        Content = new StackPanel
                        {
                            Margin = new Avalonia.Thickness(20),
                            Spacing = 15,
                            Children =
                            {
                                new TextBlock
                                {
                                    Text = $"Failed to export filter rules: {ex.Message}",
                                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                                },
                                new Button
                                {
                                    Content = "OK",
                                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                    Padding = new Avalonia.Thickness(30, 8)
                                }
                            }
                        }
                    };
                    await errorDialog.ShowDialog(window);
                }
            }
        }
    }
}

/// <summary>
/// Converter for the advanced filters toggle button text.
/// </summary>
public class AdvancedToggleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isExpanded)
        {
            return isExpanded ? "▼ Advanced Filters" : "▶ Advanced Filters";
        }
        return "▶ Advanced Filters";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converter that inverts a boolean value.
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }
}

/// <summary>
/// Converter that returns true if the value is 0 (for showing empty state).
/// </summary>
public class ZeroToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue == 0;
        }
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
