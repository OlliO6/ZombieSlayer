using Godot;
using System.Collections.Generic;
using System.Linq;

namespace BetterFileSystem
{
    [Tool]
    public class ClearFilter : IconButton
    {
        internal Node filters;

        private void OnPressed()
        {
            Clear(null);
        }
        public void Clear(FilterButton dont)
        {
            foreach (FilterButton filterButton in GetAllFilterButtons())
            {
                if (filterButton != dont && filterButton.autoDisable)
                {
                    filterButton.Deactivate();
                    filterButton.SetPressedNoSignal(false);
                }
            }

            BetterFileSystemPlugin.instance.searchFilter = "";

            BetterFileSystemPlugin.instance.ManuelUpdate();
        }

        private IEnumerable<FilterButton> GetAllFilterButtons()
        {
            return GetAllChildren(filters).OfType<FilterButton>();
        }

        private List<Node> GetAllChildren(Node parent)
        {
            List<Node> children = new();

            for (int i = 0; i < parent.GetChildCount(); i++)
            {
                Node child = parent.GetChild(i);
                children.Add(child);
                children.AddRange(GetAllChildren(child));
            }

            return children;
        }
    }
}