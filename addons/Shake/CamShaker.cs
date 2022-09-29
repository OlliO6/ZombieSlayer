namespace Shaking;

using System.Linq;
using Additions;
using Godot;

public class CamShaker : Shaker
{
    #region Camera Reference

    private Camera2D storerForCamera;
    public Camera2D Camera => this.LazyGetNode(ref storerForCamera, _Camera);
    [Export] private NodePath _Camera = "../Camera2D";

    #endregion

    public override void _Process(float delta)
    {
        amp = GetAmp();
        freqSummand = GetFreqSummand();
        ProcessNoise(delta);
        Camera.Rotation = GetShakedRotation();
        Camera.Offset = GetShakedPosition();
    }
}
