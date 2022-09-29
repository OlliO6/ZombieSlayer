namespace Shaking;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Shaker : Node2D
{
    [Export] public float rotationMagnitude = 10, positionMagnitude = 10, speed = 10, amountThreshold = 0.1f;

    public List<ShakeProfile> currentShakeProfiles = new();

    protected float amp, freqSummand, noiseY;
    private OpenSimplexNoise noiseMap;

    public override void _Ready()
    {
        GD.Randomize();
        noiseMap = new OpenSimplexNoise()
        {
            Seed = (int)GD.Randi(),
            Period = 4,
            Octaves = 2
        };
    }

    public void Shake(ShakeProfile profile) => Shake(profile, 1f);
    public void Shake(ShakeProfile profile, float ampAndTimeFactor) => Shake(profile, ampAndTimeFactor, ampAndTimeFactor);
    public void Shake(ShakeProfile profile, float ampFactor, float timeFactor)
    {
        profile = GetUnusedProfile(profile);

        profile.Bound(this);
        currentShakeProfiles.Add(profile);
        profile.StartInterpolate(ampFactor, timeFactor);
        profile.Finished += (ShakeProfile shakeProfile) => currentShakeProfiles.Remove(shakeProfile);
    }

    private ShakeProfile GetUnusedProfile(ShakeProfile profile)
    {
        if (profile.inUse)
        {
            profile = profile.Duplicate() as ShakeProfile;
        }
        return profile;
    }

    public override void _Process(float delta)
    {
        amp = GetAmp();
        freqSummand = GetFreqSummand();
        ProcessNoise(delta);
        Rotation = GetShakedRotation();
        Position = GetShakedPosition();
    }

    protected void ProcessNoise(float delta) => noiseY += (Mathf.Max(speed + freqSummand, 1)) * delta;

    protected float GetFreqSummand() => currentShakeProfiles.Sum((ShakeProfile profile) => profile.FrequencySummand);

    protected float GetAmp() => currentShakeProfiles.Sum((ShakeProfile profile) => profile.ShakeAmount);

    protected Vector2 GetShakedPosition() => amp < amountThreshold ? Vector2.Zero : positionMagnitude * amp *
                    new Vector2(noiseMap.GetNoise2d(noiseMap.Seed * 2, noiseY), noiseMap.GetNoise2d(noiseMap.Seed * 3, noiseY));
    protected float GetShakedRotation() => rotationMagnitude is 0 ? 0 : (amp < amountThreshold ? 0 : rotationMagnitude * amp * noiseMap.GetNoise2d(noiseMap.Seed, noiseY));
}
