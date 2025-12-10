using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Logbert.ViewModels.Dialogs;

/// <summary>
/// Represents a column configuration item.
/// </summary>
public partial class ColumnItem : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private bool _isVisible = true;

    [ObservableProperty]
    private double _width = 100;

    [ObservableProperty]
    private int _displayIndex;
}

/// <summary>
/// ViewModel for the Column Reorder dialog.
/// </summary>
public partial class ColumnReorderDialogViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the collection of column items.
    /// </summary>
    public ObservableCollection<ColumnItem> Columns { get; } = new();

    /// <summary>
    /// Gets or sets the currently selected column.
    /// </summary>
    [ObservableProperty]
    private ColumnItem? _selectedColumn;

    /// <summary>
    /// Gets the dialog result.
    /// </summary>
    public bool DialogResult { get; private set; }

    #region Commands

    public IRelayCommand MoveUpCommand { get; }
    public IRelayCommand MoveDownCommand { get; }
    public IRelayCommand ShowAllCommand { get; }
    public IRelayCommand HideAllCommand { get; }
    public IRelayCommand ResetCommand { get; }
    public IRelayCommand OkCommand { get; }
    public IRelayCommand CancelCommand { get; }

    #endregion

    public ColumnReorderDialogViewModel()
    {
        MoveUpCommand = new RelayCommand(OnMoveUp, CanMoveUp);
        MoveDownCommand = new RelayCommand(OnMoveDown, CanMoveDown);
        ShowAllCommand = new RelayCommand(OnShowAll);
        HideAllCommand = new RelayCommand(OnHideAll);
        ResetCommand = new RelayCommand(OnReset);
        OkCommand = new RelayCommand(OnOk);
        CancelCommand = new RelayCommand(OnCancel);

        // Initialize with default columns
        LoadDefaultColumns();
    }

    private void LoadDefaultColumns()
    {
        Columns.Clear();
        Columns.Add(new ColumnItem { Name = "Number", DisplayName = "Number", IsVisible = true, Width = 80, DisplayIndex = 0 });
        Columns.Add(new ColumnItem { Name = "Level", DisplayName = "Level", IsVisible = true, Width = 80, DisplayIndex = 1 });
        Columns.Add(new ColumnItem { Name = "Timestamp", DisplayName = "Timestamp", IsVisible = true, Width = 150, DisplayIndex = 2 });
        Columns.Add(new ColumnItem { Name = "Logger", DisplayName = "Logger", IsVisible = true, Width = 200, DisplayIndex = 3 });
        Columns.Add(new ColumnItem { Name = "Thread", DisplayName = "Thread", IsVisible = true, Width = 80, DisplayIndex = 4 });
        Columns.Add(new ColumnItem { Name = "Message", DisplayName = "Message", IsVisible = true, Width = 500, DisplayIndex = 5 });
        Columns.Add(new ColumnItem { Name = "Exception", DisplayName = "Exception", IsVisible = false, Width = 200, DisplayIndex = 6 });
        Columns.Add(new ColumnItem { Name = "Class", DisplayName = "Class", IsVisible = false, Width = 150, DisplayIndex = 7 });
        Columns.Add(new ColumnItem { Name = "Method", DisplayName = "Method", IsVisible = false, Width = 150, DisplayIndex = 8 });
        Columns.Add(new ColumnItem { Name = "File", DisplayName = "File", IsVisible = false, Width = 150, DisplayIndex = 9 });
        Columns.Add(new ColumnItem { Name = "Line", DisplayName = "Line", IsVisible = false, Width = 60, DisplayIndex = 10 });
    }

    partial void OnSelectedColumnChanged(ColumnItem? value)
    {
        ((RelayCommand)MoveUpCommand).NotifyCanExecuteChanged();
        ((RelayCommand)MoveDownCommand).NotifyCanExecuteChanged();
    }

    private bool CanMoveUp()
    {
        return SelectedColumn != null && Columns.IndexOf(SelectedColumn) > 0;
    }

    private void OnMoveUp()
    {
        if (SelectedColumn == null) return;

        var index = Columns.IndexOf(SelectedColumn);
        if (index > 0)
        {
            Columns.Move(index, index - 1);
            UpdateDisplayIndices();
            ((RelayCommand)MoveUpCommand).NotifyCanExecuteChanged();
            ((RelayCommand)MoveDownCommand).NotifyCanExecuteChanged();
        }
    }

    private bool CanMoveDown()
    {
        return SelectedColumn != null && Columns.IndexOf(SelectedColumn) < Columns.Count - 1;
    }

    private void OnMoveDown()
    {
        if (SelectedColumn == null) return;

        var index = Columns.IndexOf(SelectedColumn);
        if (index < Columns.Count - 1)
        {
            Columns.Move(index, index + 1);
            UpdateDisplayIndices();
            ((RelayCommand)MoveUpCommand).NotifyCanExecuteChanged();
            ((RelayCommand)MoveDownCommand).NotifyCanExecuteChanged();
        }
    }

    private void UpdateDisplayIndices()
    {
        for (int i = 0; i < Columns.Count; i++)
        {
            Columns[i].DisplayIndex = i;
        }
    }

    private void OnShowAll()
    {
        foreach (var column in Columns)
        {
            column.IsVisible = true;
        }
    }

    private void OnHideAll()
    {
        // Keep at least Number and Message visible
        foreach (var column in Columns)
        {
            column.IsVisible = column.Name == "Number" || column.Name == "Message";
        }
    }

    private void OnReset()
    {
        LoadDefaultColumns();
    }

    private void OnOk()
    {
        DialogResult = true;
    }

    private void OnCancel()
    {
        DialogResult = false;
    }

    /// <summary>
    /// Gets the column configuration for saving.
    /// </summary>
    public ColumnItem[] GetColumnConfiguration()
    {
        return Columns.ToArray();
    }

    /// <summary>
    /// Loads column configuration from saved settings.
    /// </summary>
    public void LoadColumnConfiguration(ColumnItem[] columns)
    {
        Columns.Clear();
        foreach (var column in columns.OrderBy(c => c.DisplayIndex))
        {
            Columns.Add(column);
        }
    }
}
