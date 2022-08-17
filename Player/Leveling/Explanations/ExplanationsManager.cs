using System;
using System.Collections.Generic;
using Additions;
using Godot;

public class ExplanationsManager : MarginContainer
{
    public Explanation currentExplanation;

    public override void _Ready()
    {
        if (Player.currentPlayer is null) return;

        Player.currentPlayer.Connect(nameof(Player.LevelChanged), this, nameof(OnPlayerLevelChanged));
    }

    private void OnPlayerLevelChanged(int level)
    {
        var newExplanation = GetNodeOrNull<Explanation>($"Level{level}");
        if (newExplanation is null) return;

        currentExplanation?.Finish();
        currentExplanation = newExplanation;
        currentExplanation.Beginn();
    }
}
