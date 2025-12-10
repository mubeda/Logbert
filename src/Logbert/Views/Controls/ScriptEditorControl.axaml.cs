using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using Logbert.ViewModels.Controls;
using System.Xml;

namespace Logbert.Views.Controls;

public partial class ScriptEditorControl : UserControl
{
    private ScriptEditorViewModel? _viewModel;

    public ScriptEditorControl()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;

        // Configure Lua syntax highlighting
        SetupLuaSyntaxHighlighting();
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is ScriptEditorViewModel viewModel)
        {
            _viewModel = viewModel;

            // Bind text editor to view model
            if (this.FindControl<TextEditor>("textEditor") is TextEditor editor)
            {
                editor.Text = viewModel.ScriptText;

                editor.TextChanged += (s, e) =>
                {
                    if (_viewModel != null)
                    {
                        _viewModel.ScriptText = editor.Text;
                    }
                };

                editor.TextArea.Caret.PositionChanged += (s, e) =>
                {
                    if (_viewModel != null)
                    {
                        _viewModel.UpdateCursorPosition(
                            editor.TextArea.Caret.Line,
                            editor.TextArea.Caret.Column
                        );
                    }
                };
            }
        }
    }

    private void SetupLuaSyntaxHighlighting()
    {
        var editor = this.FindControl<TextEditor>("textEditor");
        if (editor == null)
            return;

        // Create a simple Lua syntax highlighting definition
        var luaHighlighting = HighlightingManager.Instance.GetDefinition("Lua");

        // If Lua highlighting is not available, we'll use a basic one
        if (luaHighlighting == null)
        {
            // Use JavaScript as a fallback - it's similar enough for basic highlighting
            luaHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
        }

        editor.SyntaxHighlighting = luaHighlighting;
    }
}
