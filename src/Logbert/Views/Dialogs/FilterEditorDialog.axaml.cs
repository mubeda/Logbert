using System;
using Avalonia.Controls;
using Logbert.Converters;
using Logbert.Models;
using Logbert.ViewModels.Dialogs;

namespace Logbert.Views.Dialogs;

public partial class FilterEditorDialog : Window
{
    /// <summary>
    /// Gets the view model.
    /// </summary>
    public FilterEditorDialogViewModel ViewModel { get; }

    /// <summary>
    /// Gets the resulting filter rule after dialog closes.
    /// </summary>
    public FilterRule? ResultRule { get; private set; }

    public FilterEditorDialog()
    {
        InitializeComponent();

        ViewModel = new FilterEditorDialogViewModel();
        DataContext = ViewModel;

        // Add converters to resources
        Resources["BoolToBackgroundConverter"] = new BoolToBackgroundConverter();
        Resources["BoolToForegroundConverter"] = new BoolToForegroundConverter();
        Resources["BoolToIconConverter"] = new BoolToIconConverter();

        // Subscribe to ViewModel events
        ViewModel.Accepted += OnAccepted;
        ViewModel.Cancelled += OnCancelled;
    }

    /// <summary>
    /// Initializes the dialog for adding a new filter rule.
    /// </summary>
    public void InitializeForAdd()
    {
        ViewModel.InitializeForAdd();
    }

    /// <summary>
    /// Initializes the dialog for editing an existing filter rule.
    /// </summary>
    /// <param name="rule">The rule to edit.</param>
    public void InitializeForEdit(FilterRule rule)
    {
        ViewModel.InitializeForEdit(rule);
    }

    private void OnAccepted(object? sender, FilterRule rule)
    {
        ResultRule = rule;
        Close(true);
    }

    private void OnCancelled(object? sender, EventArgs e)
    {
        ResultRule = null;
        Close(false);
    }
}
