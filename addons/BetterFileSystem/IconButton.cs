using Godot;

namespace BetterFileSystem
{
    [Tool]
    public class IconButton : Button
    {
        private string __icon;
        private bool firstFrame = true;

        [Export]
        private string __EditorIcon
        {
            get => __icon;
            set
            {
                __icon = value;

                if (value is not "")
                {
                    if (BetterFileSystemPlugin.instance is null)
                        Icon = GetIcon(value, "EditorIcons");
                    else
                        Icon = BetterFileSystemPlugin.instance.EditorTheme.GetIcon(value, "EditorIcons");
                }
            }
        }

        [Export] protected bool defaultPressed;

        public override void _Process(float delta)
        {
            if (!firstFrame) return;

            __EditorIcon = __icon;

            firstFrame = false;

            if (ToggleMode)
            {
                SetPressedNoSignal(defaultPressed);
                EmitSignal("toggled", defaultPressed);
                return;
            }

            if (defaultPressed) EmitSignal("pressed");
        }
    }
}
