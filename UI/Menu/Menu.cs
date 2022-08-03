using System;
using Godot;

public class Menu : CanvasLayer
{
    [Export] private PackedScene gameScene;

    [TroughtSignal]
    private void OnPlayPressed()
    {
        SceneManager.ChangeScence(gameScene);
    }

    [TroughtSignal]
    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
