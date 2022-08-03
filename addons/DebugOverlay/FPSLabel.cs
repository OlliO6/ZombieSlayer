#if DEBUG
using Godot;

namespace Additions.Debugging
{
    public class FPSLabel : Label
    {
        public override void _Process(float delta)
        {
            Text = $"FPS: {Engine.GetFramesPerSecond()}";
        }
    }
}
#endif