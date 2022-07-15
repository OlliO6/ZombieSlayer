using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterFileSystem
{
    [Tool]
    public class BetterFileSystemPlugin : EditorPlugin
    {
        #region Variables 

        private bool hidingEmptyDirectories;

        internal static BetterFileSystemPlugin instance;
        internal Theme EditorTheme => GetEditorInterface().GetBaseControl().Theme;

        internal string searchFilter { get => filterBar.Text; set => filterBar.Text = value; }

        private PackedScene firstRowExtrasScene = GD.Load<PackedScene>("res://addons/BetterFileSystem/FirstRowExtras.tscn");
        private PackedScene secondRowExtrasScene = GD.Load<PackedScene>("res://addons/BetterFileSystem/SecondRowExtras.tscn");
        private PackedScene sideBarScene = GD.Load<PackedScene>("res://addons/BetterFileSystem/SideBar.tscn");

        private FileSystemDock fileSystemDock;
        internal EditorFileSystem fileSystem;

        private Tree fileTree;
        private VScrollBar treeScroll;
        private Node row;
        private Node firstRowExtras;
        private Node secondRowExtras;
        private Node sideBar;
        private HBoxContainer hBox;
        private LineEdit filterBar;
        internal ClearFilter clearButton;
        internal List<FileFilter> fileFilters = new();
        internal List<DirectoryFilter> directoryFilters = new();

        private Control buttonToHide;
        private Control rescanButton;

        private Dictionary<string, TreeItem> favorites = new();

        #endregion

        #region Setup 

        public override void _EnterTree()
        {
            fileSystemDock = GetEditorInterface().GetFileSystemDock();
            fileSystem = GetEditorInterface().GetResourceFilesystem();
            row = fileSystemDock.GetChild(3);
            fileTree = row.GetChild<Tree>(0);
            treeScroll = fileTree.GetChild<VScrollBar>(4);

            hBox = new() { };
            row.RemoveChild(fileTree);
            row.AddChild(hBox);
            row.MoveChild(hBox, 0);

            sideBar = sideBarScene.Instance();
            hBox.AddChild(sideBar);
            hBox.AddChild(fileTree);
            fileTree.SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill;

            Node vBox = fileSystemDock.GetChild(0);
            Node firstRow = vBox.GetChild(0);
            Node secondRow = vBox.GetChild(1);

            firstRowExtras = firstRowExtrasScene.Instance();
            firstRow.AddChild(firstRowExtras);
            firstRow.MoveChild(firstRowExtras, 0);

            secondRowExtras = secondRowExtrasScene.Instance();
            secondRow.AddChild(secondRowExtras);
            secondRow.MoveChild(secondRowExtras, 0);

            filterBar = secondRow.GetChild<LineEdit>(1);

            rescanButton = firstRow.GetChild<Control>(4); // Hidden feature
            rescanButton.Show();

            buttonToHide = firstRow.GetChild<Control>(5);
            buttonToHide.Hide();

            clearButton = secondRowExtras.GetChild<ClearFilter>(0);
            clearButton.filters = sideBar;

            rescanButton.Connect("pressed", this, nameof(Update));
            filterBar.Connect("text_changed", this, nameof(OnFilterBarChanged));
            fileTree.Connect("draw", this, nameof(Update));

            secondRowExtras.GetNode("HideEmpty").Connect("toggled", this, nameof(ToggleHideEmpty));
            secondRowExtras.GetNode("Collapse").Connect("pressed", this, nameof(CollapseAll));
            secondRowExtras.GetNode("Expand").Connect("pressed", this, nameof(ExpandAll));

            treeScroll.Connect("changed", this, nameof(OnScrollChange));
            treeScroll.Connect("value_changed", this, nameof(OnScrollValueChange));

            ManuelUpdate();
            instance = this;
        }

        public override void _ExitTree()
        {
            instance = null;

            firstRowExtras.QueueFree();
            secondRowExtras.QueueFree();
            sideBar.QueueFree();

            buttonToHide.Show();
            rescanButton.Hide();

            hBox.RemoveChild(fileTree);
            row.AddChild(fileTree);
            row.MoveChild(fileTree, 0);
            hBox.QueueFree();
        }

        public override void _Process(float delta)
        {
            if (instance is null) instance = this;
            ScrollFixProcess();
        }

        #endregion

        #region Scroll Fix

        int scrollChangeCount, scrollValueChangeCount;
        float? scrollHolder; // Bugfix but i am to lazy to make it readable :)

        private void ScrollFixProcess()
        {
            if (scrollChangeCount != 0)
                Update();

            if (scrollChangeCount >= 4 && scrollValueChangeCount > 0 && scrollHolder.HasValue)
            {
                CallDeferred(nameof(ChangeScroll), scrollHolder.Value);
            }
            scrollChangeCount = 0;
            scrollValueChangeCount = 0;

            scrollHolder = null;
        }
        private void ChangeScroll(float value)
        {
            treeScroll.Value = value;
        }

        private void OnScrollChange()
        {
            scrollChangeCount++;

            if (scrollHolder is null)
                scrollHolder = (float)treeScroll.Value;
        }
        private void OnScrollValueChange(float value)
        {
            scrollValueChangeCount++;
        }

        #endregion

        #region Updating 
        private void OnFilterBarChanged(string _) => Update();

        internal void ManuelUpdate()
        {
            float prevScroll = (float)treeScroll.Ratio;
            filterBar.EmitSignal("text_changed", filterBar.Text);
            treeScroll.Ratio = prevScroll;
        }
        public void Update()
        {
            TreeItem favDir = fileTree.GetRoot().GetChildren();
            TreeItem resDir = favDir.GetNext();
            TreeItem currentItem = resDir.GetChildren();

            Stack<TreeItem> folders = new();

            FilterFiles(currentItem, "res:/", ref folders);

            SetFavotites();

            FilterDirectories(folders);

            void FilterFiles(TreeItem item, string path, ref Stack<TreeItem> folders)
            {
                if (item is null)
                    return;

                string parentText = item.GetParent().GetText(0);

                while (item is not null)
                {
                    string pathToItem = $"{path}/{item.GetText(0)}";

                    string itemType = fileSystem.GetFileType(pathToItem);

                    TreeItem next = item.GetNext();

                    bool isFolder = itemType is "";

                    if (isFolder)
                        folders.Push(item);

                    FilterFiles(item.GetChildren(), pathToItem, ref folders);

                    if (!isFolder && !FileFiltered(pathToItem, item.GetText(0), itemType))
                        item.Free();

                    item = next;
                }
            }

            void SetFavotites()
            {
                favorites.Clear();
                TreeItem fav = favDir.GetChildren();

                while (fav is not null)
                {
                    favorites.Add(fav.GetTooltip(0), fav);

                    fav = fav.GetNext();
                }
            }
        }

        #endregion

        #region Filter Logic 
        private bool FileFiltered(string path, string name, string itemType)
        {
            bool noIncluders = true;
            bool included = false;

            foreach (FileFilter filter in fileFilters)
            {
                if (noIncluders && filter.includeType is IncludeType.Include) noIncluders = false;

                bool isMathing = false;

                switch (filter.filterType)
                {
                    case FileFilterType.DerivedType:
                        isMathing = IsClassDerivedFromType(itemType, filter.filterString);
                        break;

                    case FileFilterType.MatchType:
                        isMathing = itemType == filter.filterString;
                        break;

                    case FileFilterType.PathContains:
                        isMathing = path.Contains(filter.filterString);
                        break;

                    case FileFilterType.PathMatch:
                        isMathing = path == filter.filterString;
                        break;

                    case FileFilterType.NameContains:
                        isMathing = name.Contains(filter.filterString);
                        break;

                    case FileFilterType.NameMatch:
                        isMathing = name == filter.filterString;
                        break;
                }


                if (!isMathing) continue;

                switch (filter.includeType)
                {
                    case IncludeType.Include:
                        included = true;
                        continue;

                    case IncludeType.Exclude:
                        return false;
                }
            }

            return noIncluders || included;
        }

        private bool IsClassDerivedFromType(string @class, string type)
        {
            if (@class is "")
                return false;
            if (@class == type)
                return true;

            string parentClass = ClassDB.GetParentClass(@class);

            return IsClassDerivedFromType(parentClass, type);
        }

        private void FilterDirectories(Stack<TreeItem> folders)
        {
            IEnumerable<DirectoryFilter> includers =
            directoryFilters.Where((DirectoryFilter filter) =>
            {
                return filter.includeType is IncludeType.Include;
            });
            IEnumerable<DirectoryFilter> excluders =
            directoryFilters.Where((DirectoryFilter filter) =>
            {
                return filter.includeType is IncludeType.Exclude;
            });

            bool noInluders = includers.Count() == 0;


            foreach (TreeItem folder in folders)
            {
                string path = GetPath(folder);

                foreach (DirectoryFilter filter in excluders)
                {
                    if (IsMatching(folder, path, filter))
                    {
                        RemoveFolder(folder, path);
                        continue;
                    }
                }

                if (noInluders) continue;

                foreach (DirectoryFilter filter in includers)
                {
                    if (IsMatching(folder, path, filter)) continue;
                }

                RemoveFolder(folder, path);
            }

            if (hidingEmptyDirectories) RemoveEmptyFolders(folders);


            void RemoveFolder(TreeItem folder, string path)
            {
                if (favorites.ContainsKey(path)) favorites[path].Free();
                folder.CallDeferred("free");
            }

            static bool IsMatching(TreeItem folder, string path, DirectoryFilter filter)
            {
                switch (filter.filterType)
                {
                    case DirectoryFilterType.PathMatch:
                        return path == filter.filterString;

                    case DirectoryFilterType.PathContains:
                        return path.Contains(filter.filterString);

                    case DirectoryFilterType.NameMatch:
                        return folder.GetText(0) == filter.filterString;

                    case DirectoryFilterType.NameContains:
                        return folder.GetText(0).Contains(filter.filterString);
                }

                return false;
            }

            static string GetPath(TreeItem folder)
            {
                string path = "";
                while (folder is not null)
                {
                    string name = folder.GetText(0);
                    folder = folder.GetParent();

                    if (name is "res://")
                    {
                        path = $"res://{path}";
                        break;
                    }
                    path = $"{name}/{path}";
                }
                return path;
            }

            static void RemoveEmptyFolders(Stack<TreeItem> folders)
            {
                while (folders.Count > 0)
                {
                    TreeItem folder = folders.Pop();

                    if (folder is null) continue;

                    if (folder.GetChildren() is null)
                        folder.Free();
                }
            }
        }

        #endregion

        #region Interactions 

        public void ToggleHideEmpty(bool toggled)
        {
            hidingEmptyDirectories = toggled;
            ManuelUpdate();
        }

        public void ExpandAll()
        {
            ManuelUpdate();
            TreeItem favorites = fileTree.GetRoot().GetChildren();
            TreeItem resDir = favorites.GetNext();

            favorites.Collapsed = false;

            SetCollapseRecursive(resDir, false);
        }

        public void CollapseAll()
        {
            ManuelUpdate();
            TreeItem favorites = fileTree.GetRoot().GetChildren();
            TreeItem resDir = favorites.GetNext();

            favorites.Collapsed = true;

            SetCollapseRecursive(resDir.GetChildren(), true);
        }

        private void SetCollapseRecursive(TreeItem item, bool collapse)
        {
            if (item is null)
                return;

            item.Collapsed = collapse;

            SetCollapseRecursive(item.GetNext(), collapse);
            SetCollapseRecursive(item.GetChildren(), collapse);
        }

        public void SetSearchFilter(string filter)
        {
            filterBar.Text = filter;
            // ManuelUpdate();
        }

        internal void GoToSelected()
        {
            string fullSelectedPath = GetEditorInterface().GetCurrentPath();
            string selectedType = fileSystem.GetFileType(fullSelectedPath);

            string selectedPath = fullSelectedPath;
            try
            {
                selectedPath = selectedPath.Remove(0, 6);

                if (selectedPath[selectedPath.Length - 1] is '/')
                {
                    selectedPath = selectedPath.Remove(selectedPath.Length - 1);
                }
            }
            catch
            {
                return;
            }

            TreeItem resDir = fileTree.GetRoot().GetChildren().GetNext();

            string[] splittedPath = selectedPath.Split('/');
            string name = splittedPath[splittedPath.Length - 1];

            TreeItem currentDir = resDir;
            for (int i = 0; i < splittedPath.Length; i++)
            {
                currentDir.Collapsed = false;
                currentDir = GetChild(currentDir, splittedPath[i]);

                if (currentDir is null) break;
            }

            if (currentDir is null)
            {
                clearButton.Clear(null);

                After3Frames(() =>
                {
                    GoToSelected();
                });

                return;
            }

            TreeItem item = GetChild(currentDir, name);

            if (item is null) item = currentDir;


            List<TreeItem> allVisible = GetAllVisibleItemsSorted();

            int positionOfItem = 0;

            for (int i = 0; i < allVisible.Count; i++)
            {
                if (allVisible[i] == item)
                {
                    positionOfItem = i;
                    break;
                }
            }

            float scrollRatio = ((float)positionOfItem / (float)allVisible.Count) - (((float)treeScroll.Page / 3) / (float)treeScroll.MaxValue);

            treeScroll.Ratio = scrollRatio;

            After3Frames(() =>
                {
                    treeScroll.Ratio = scrollRatio;
                });


            TreeItem GetChild(TreeItem item, string name)
            {
                if (item is null) return null;

                item = item.GetChildren();

                while (item is not null)
                {
                    if (item.GetText(0) == name) return item;

                    item = item.GetNext();
                }

                return null;
            }

            List<TreeItem> GetAllVisibleItemsSorted()
            {
                List<TreeItem> result = new();

                TreeItem current = fileTree.GetRoot();

                while (current is not null)
                {
                    current = current.GetNextVisible(false);

                    if (current is null) break;

                    result.Add(current);
                }

                return result;
            }

            void After3Frames(Action action)
            {
                ToSignal(GetTree(), "idle_frame").OnCompleted(() =>
                {
                    ToSignal(GetTree(), "idle_frame").OnCompleted(() =>
                    {
                        ToSignal(GetTree(), "idle_frame").OnCompleted(() =>
                        {
                            action();
                        });
                    });
                });
            }
        }
        
        #endregion
    }
}
