using Godot;
using System.Linq;
using System.Collections.Generic;

namespace BetterFileSystem
{
    [Tool]
    public class FilterButton : IconButton
    {
        [Export] public string searchFilter = "";
        [Export] public bool autoDisable = true, autoExpandAll, autoCollapseAll;
        [Export] private string autoSelectPath = "";

        private void OnToggled(bool toggled)
        {
            if (Owner is null || Owner.GetParent() is Viewport) return;
            if (BetterFileSystemPlugin.instance is null) return;

            if (toggled)
                Activate();
            else
                Deactivate();

            BetterFileSystemPlugin.instance.ManuelUpdate();
        }

        internal void Activate()
        {
            BetterFileSystemPlugin betterFileSystem = BetterFileSystemPlugin.instance;

            if (autoDisable) betterFileSystem.clearButton.Clear(this);

            if (searchFilter != "") betterFileSystem.SetSearchFilter(searchFilter);


            GetFilterNodes(out IEnumerable<FileFilter> fileFilters, out IEnumerable<DirectoryFilter> directoryFilters);

            AddAndRemoveFilters(fileFilters, directoryFilters, FilterState.OnEnabled);
        }

        internal void Deactivate()
        {
            BetterFileSystemPlugin betterFileSystem = BetterFileSystemPlugin.instance;

            GetFilterNodes(out IEnumerable<FileFilter> fileFilters, out IEnumerable<DirectoryFilter> directoryFilters);

            AddAndRemoveFilters(fileFilters, directoryFilters, FilterState.OnDisabled);
        }


        private void GetFilterNodes(out IEnumerable<FileFilter> fileFilters, out IEnumerable<DirectoryFilter> directoryFilters)
        {
            Godot.Collections.Array children = GetChildren();

            fileFilters = children.OfType<FileFilter>();
            directoryFilters = children.OfType<DirectoryFilter>();
        }

        private void AddAndRemoveFilters(IEnumerable<FileFilter> fileFilters, IEnumerable<DirectoryFilter> directoryFilters, FilterState whenToAdd)
        {
            BetterFileSystemPlugin betterFileSystem = BetterFileSystemPlugin.instance;

            foreach (var filter in fileFilters)
                betterFileSystem.fileFilters.Remove(filter);

            foreach (var filter in directoryFilters)
                betterFileSystem.directoryFilters.Remove(filter);

            var fileFiltersToAdd = fileFilters.Where((FileFilter filter) => { return filter.whenToFilter == whenToAdd; });
            var directoryFiltersToAdd = directoryFilters.Where((DirectoryFilter filter) => { return filter.whenToFilter == whenToAdd; });


            foreach (var filter in fileFiltersToAdd)
                betterFileSystem.fileFilters.Add(filter);

            foreach (var filter in directoryFiltersToAdd)
                betterFileSystem.directoryFilters.Add(filter);
        }
    }
}
