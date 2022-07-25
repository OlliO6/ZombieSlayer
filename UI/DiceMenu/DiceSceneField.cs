using Godot;
using System;
using Additions;

[Tool]
public class DiceSceneField : NinePatchRect
{
    [Export]
    public PackedScene Scene
    {
        get => _scene;
        set
        {
            _scene = value;

            if (value is null)
            {
                CallDeferred(nameof(Deactivate));
                return;
            }

            if (Icons.IconPickupMatrix.ContainsKey(value)) CallDeferred(nameof(SetIcon), Icons.IconPickupMatrix[value]);
        }
    }

    private PackedScene _scene;

    private void SetIcon(Texture icon)
    {
        IconRect.Texture = icon;
    }

    private void Deactivate()
    {
        IconRect.Visible = false;

        Modulate = new Color(Colors.White, 0.5f);
    }

    #region IconRect Reference

    private TextureRect storerForIconRect;

    public TextureRect IconRect => this.LazyGetNode(ref storerForIconRect, "Icon");

    #endregion

    public override void _Ready()
    {
        HintTooltip = GetDescription();
    }

    private string GetDescription()
    {
        if (Scene is null) return "";

        SceneState sceneState = Scene.GetState();

        int propCount = sceneState.GetNodePropertyCount(0);

        if (propCount > 1 && sceneState.GetNodePropertyValue(0, 1) is CSharpScript script)
        {
            return script.Call("GetDescription") as string;
        }

        return "";
    }
}
