using Godot;
using System;

public class Menu : Node
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
