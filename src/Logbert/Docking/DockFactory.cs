using Dock.Model.Controls;
using Dock.Model.Core;
using Logbert.ViewModels;
using Logbert.ViewModels.Docking;

namespace Logbert.Docking;

/// <summary>
/// Factory for creating dock elements.
/// </summary>
public class DockFactory : Factory
{
    private readonly MainWindowViewModel _mainViewModel;

    public DockFactory(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public override IRootDock CreateLayout()
    {
        // Create tool documents (panels)
        var filterPanel = new FilterPanelViewModel
        {
            Id = "FilterPanel",
            Title = "Filter"
        };

        var loggerTree = new LoggerTreeViewModel
        {
            Id = "LoggerTree",
            Title = "Logger Tree"
        };

        var bookmarksPanel = new BookmarksPanelViewModel
        {
            Id = "Bookmarks",
            Title = "Bookmarks"
        };

        // Create tool dock for left side
        var leftToolDock = new ProportionalDock
        {
            Id = "LeftTools",
            Title = "LeftTools",
            Proportion = 0.25,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    Id = "LeftToolDock",
                    Title = "LeftToolDock",
                    Proportion = double.NaN,
                    ActiveDockable = filterPanel,
                    VisibleDockables = CreateList<IDockable>(filterPanel, loggerTree)
                },
                new SplitterDock()
                {
                    Id = "LeftSplitter",
                    Title = "LeftSplitter"
                },
                new ToolDock
                {
                    Id = "LeftBottomToolDock",
                    Title = "LeftBottomToolDock",
                    Proportion = double.NaN,
                    ActiveDockable = bookmarksPanel,
                    VisibleDockables = CreateList<IDockable>(bookmarksPanel)
                }
            )
        };

        // Create document dock for log tabs
        var documentDock = new DocumentDock
        {
            Id = "Documents",
            Title = "Documents",
            Proportion = double.NaN,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>(),
            CanCreateDocument = false
        };

        // Create main layout with proportional dock
        var mainLayout = new ProportionalDock
        {
            Id = "MainLayout",
            Title = "MainLayout",
            Proportion = double.NaN,
            Orientation = Orientation.Horizontal,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>
            (
                leftToolDock,
                new SplitterDock()
                {
                    Id = "MainSplitter",
                    Title = "MainSplitter"
                },
                documentDock
            )
        };

        // Create root dock
        var rootDock = CreateRootDock();
        rootDock.Id = "Root";
        rootDock.Title = "Root";
        rootDock.ActiveDockable = mainLayout;
        rootDock.DefaultDockable = mainLayout;
        rootDock.VisibleDockables = CreateList<IDockable>(mainLayout);

        return rootDock;
    }

    public override void InitLayout(IDockable layout)
    {
        ContextLocator = new Dictionary<string, Func<object?>>
        {
            ["FilterPanel"] = () => _mainViewModel.ActiveDocument,
            ["LoggerTree"] = () => _mainViewModel.ActiveDocument,
            ["Bookmarks"] = () => _mainViewModel.ActiveDocument,
        };

        DockableLocator = new Dictionary<string, Func<IDockable?>>
        {
        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
        {
            [nameof(IDockWindow)] = () => new HostWindow()
        };

        base.InitLayout(layout);
    }

    /// <summary>
    /// Adds a document to the document dock.
    /// </summary>
    public void AddDocument(LogDocumentViewModel document)
    {
        if (Layout is not IRootDock rootDock)
            return;

        // Find the document dock
        var documentDock = FindDockable(rootDock, "Documents") as DocumentDock;
        if (documentDock == null)
            return;

        // Create a document wrapper
        var dockDocument = new Document
        {
            Id = $"Doc_{document.GetHashCode()}",
            Title = document.Title,
            Context = document
        };

        // Add to visible dockables
        if (documentDock.VisibleDockables == null)
            documentDock.VisibleDockables = CreateList<IDockable>();

        documentDock.VisibleDockables.Add(dockDocument);
        documentDock.ActiveDockable = dockDocument;

        // Update to notify
        this.SetActiveDockable(dockDocument);
        this.SetFocusedDockable(documentDock, dockDocument);
    }

    /// <summary>
    /// Removes a document from the document dock.
    /// </summary>
    public void RemoveDocument(LogDocumentViewModel document)
    {
        if (Layout is not IRootDock rootDock)
            return;

        var documentDock = FindDockable(rootDock, "Documents") as DocumentDock;
        if (documentDock?.VisibleDockables == null)
            return;

        var dockDocument = documentDock.VisibleDockables
            .FirstOrDefault(d => d.Context == document);

        if (dockDocument != null)
        {
            documentDock.VisibleDockables.Remove(dockDocument);

            if (documentDock.VisibleDockables.Count > 0)
            {
                documentDock.ActiveDockable = documentDock.VisibleDockables[^1];
            }
        }
    }

    /// <summary>
    /// Sets the active document.
    /// </summary>
    public void SetActiveDocument(LogDocumentViewModel? document)
    {
        if (Layout is not IRootDock rootDock)
            return;

        var documentDock = FindDockable(rootDock, "Documents") as DocumentDock;
        if (documentDock?.VisibleDockables == null)
            return;

        if (document == null)
        {
            documentDock.ActiveDockable = null;
            return;
        }

        var dockDocument = documentDock.VisibleDockables
            .FirstOrDefault(d => d.Context == document);

        if (dockDocument != null)
        {
            documentDock.ActiveDockable = dockDocument;
            this.SetActiveDockable(dockDocument);
            this.SetFocusedDockable(documentDock, dockDocument);
        }
    }

    /// <summary>
    /// Finds a dockable by ID recursively.
    /// </summary>
    private IDockable? FindDockable(IDockable root, string id)
    {
        if (root.Id == id)
            return root;

        if (root is IDock dock && dock.VisibleDockables != null)
        {
            foreach (var child in dock.VisibleDockables)
            {
                var found = FindDockable(child, id);
                if (found != null)
                    return found;
            }
        }

        return null;
    }
}
