using System;
using Godot;

public class Menu : CanvasLayer
{
    [Export] private PackedScene gameScene;

    [TroughtEditor]
    private void OnPlayPressed()
    {
        SceneManager.ChangeScence(gameScene);
    }

    [TroughtEditor]
    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
