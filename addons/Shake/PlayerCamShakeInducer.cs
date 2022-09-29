using System.Collections.Generic;
using Additions;
using Godot;
using Shaking;

public class PlayerCamShakeInducer : Node
{
    [Export] public ShakeProfile shakeProfile;

    public void Shake() => Player.currentPlayer?.ShakeCam(shakeProfile);
    public void Shake(float ampAndTimeFactor) => Player.currentPlayer?.ShakeCam(shakeProfile, ampAndTimeFactor);
    public void Shake(float ampFactor, float timeFactor) => Player.currentPlayer?.ShakeCam(shakeProfile, ampFactor, timeFactor);
}
