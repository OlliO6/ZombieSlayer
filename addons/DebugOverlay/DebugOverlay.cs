#if DEBUG
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

namespace Additions.Debugging
{
    public class DebugOverlay : CanvasLayer
    {
        public static DebugOverlay instance;

        [Export] public bool turnOffCommonLogs;
        [Export] public bool removeNameUniqeness;

        private bool fpsShown = true;
        private bool overlayIsOpen;
        private bool gameWasPaused;

        #region Labels Reference

        private Container storerForLabels;
        public Container Labels => this.LazyGetNode(ref storerForLabels, _Labels);
        [Export] private NodePath _Labels = "Labels";

        #endregion

        #region Logs Reference

        private Container storerForLogs;
        public Container Logs => this.LazyGetNode(ref storerForLogs, _Logs);
        [Export] private NodePath _Logs = "Logs";

        #endregion

        #region Menu Reference

        private DebugMenu storerForMenu;
        public DebugMenu Menu => this.LazyGetNode(ref storerForMenu, _Menu);
        [Export] private NodePath _Menu = "Menu";

        #endregion

        [Export]
        public bool IsShowingFps
        {
            get => fpsShown;
            set
            {
                fpsShown = value;

                if (Labels is null)
                {
                    ToSignal(this, "ready").OnCompleted(() =>
                    {
                        Labels.GetNode<CanvasItem>("FPSLabel").Visible = value;
                    });
                    return;
                }
                Labels.GetNode<CanvasItem>("FPSLabel").Visible = value;
            }
        }

        public override void _Ready()
        {
            if (OS.IsDebugBuild())
            {
                instance = this;
                return;
            }

            instance = null;
            QueueFree();
        }

        public override void _ExitTree()
        {
            if (!OS.IsDebugBuild()) return;

            PackedScene scene = new();
            scene.Pack(this);
            ResourceSaver.Save(Filename, scene);
        }

        public override void _Input(InputEvent @event)
        {
            if (!@event.IsPressed()) return;

            if (@event is not InputEventKey keyInput) return;

            if (keyInput.Scancode is (uint)KeyList.D && keyInput.Alt)
            {
                ToggleOverlay();
                return;
            }
        }

        private void ToggleOverlay()
        {
            if (overlayIsOpen)
                HideMenu();
            else
                ShowMenu();
        }
        public void ShowMenu()
        {
            if (overlayIsOpen) return;

            overlayIsOpen = true;
            gameWasPaused = GetTree().Paused;
            GetTree().Paused = true;
            Menu.Show();
            Menu.LoadState();
        }
        public void HideMenu()
        {
            if (!overlayIsOpen) return;

            overlayIsOpen = false;
            if (!gameWasPaused) GetTree().Paused = false;
            Menu.stepCancellation.Cancel();
            Menu.Hide();
        }

        internal void AddWatcher(Godot.Object target, string property, bool autoRemove, bool showTargetName, Color? color, string optionalName)
        {
            Labels.AddChild(new Watcher(target, property, autoRemove, showTargetName, color, optionalName));
        }
        internal void RemoveWatcher(Godot.Object target, string property)
        {
            IEnumerable<Watcher> matchingWatchers = Labels.GetChildren<Watcher>().Where((Watcher watcher) => watcher.target == target && watcher.property == property);

            foreach (Watcher watcher in matchingWatchers)
            {
                watcher.QueueFree();
            }
        }
        internal void RemoveWatchersWithTarget(Godot.Object target)
        {
            IEnumerable<Watcher> matchingWatchers = Labels.GetChildren<Watcher>().Where((Watcher watcher) => watcher.target == target);

            foreach (Watcher watcher in matchingWatchers)
            {
                watcher.QueueFree();
            }
        }

        internal async void LogT(Godot.Object target, string message, float time, bool uncommon, Color? color, string optionalName, bool alsoPrint)
        {
            if (turnOffCommonLogs && !uncommon) return;

            string name = optionalName is "" ? GetTargetName(target) : optionalName;

            Label label = new Label()
            {
                Text = $"{(target is null ? "" : ($"{(name is "" or null ? "Thing" : removeNameUniqeness ? RemoveNameUniqeness(name) : name)}: "))}{message}"
            };

            if (alsoPrint) GD.Print(label.Text);

            Logs.AddChild(label);
            label.Modulate = GetTargetColor(target, color, uncommon);
            Tween tween = Logs.GetChild<Tween>(0);
            tween.InterpolateProperty(label, "modulate:a", 1, 0, time, Tween.TransitionType.Expo, Tween.EaseType.In);
            tween.Start();

            await new TimeAwaiter(this, time);

            label.QueueFree();
        }

        internal async void LogFrame(Godot.Object target, string message, bool physicsFrame, int frames, Color? color, string optionalName, bool bottomLetf, bool alsoPrint)
        {
            string name = optionalName is "" ? GetTargetName(target) : optionalName;

            Label label = new Label()
            {
                Text = $"{(target is null ? "" : ($"{(name is "" or null ? "Thing" : removeNameUniqeness ? RemoveNameUniqeness(name) : name)}: "))}{message}"
            };

            if (alsoPrint) GD.Print(label.Text);

            (bottomLetf ? Logs : Labels).AddChild(label);
            label.Align = bottomLetf ? Label.AlignEnum.Left : Label.AlignEnum.Center;
            label.Modulate = GetTargetColor(target, color, false);

            for (int i = 0; i < frames; i++) await ToSignal(GetTree(), physicsFrame ? "physics_frame" : "idle_frame");

            label.QueueFree();
        }

        internal static string GetTargetName(Godot.Object target)
        {
            if (target is Node node) return node.Name;
            if (target is Resource resource) return resource.ResourceName;
            return "";
        }
        internal static string RemoveNameUniqeness(string name)
        {
            if (!name.StartsWith("@")) return name;

            string result;

            result = name.Remove(name.FindLast("@"));
            result = result.Remove(0, 1);

            return result;
        }
        internal static Color GetTargetColor(Godot.Object target, Color? color, bool uncommon)
        {
            if (color is null)
            {
                if (target is not null)
                {
                    foreach (var attribute in target.GetType().GetCustomAttributes(true))
                    {
                        if (attribute is DefaultColorAttribute colorAttribute)
                            return uncommon ? colorAttribute.unommonColor : colorAttribute.commonColor;
                    }
                }
                return uncommon ? Colors.LightCyan : Colors.White;
            }
            return color.Value;
        }
    }
}
#endif