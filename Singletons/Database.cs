using System;
using System.Collections;
using Additions;
using Godot;

public class Database : Node
{
    private const string WeaponDataJsonPath = "res://Data/WeaponData.json";

    public static Godot.Collections.Dictionary weaponData;

    public override void _Ready()
    {
        LoadData();
    }

    public static void LoadData()
    {
        LoadWeaponData();
    }

    private static void LoadWeaponData()
    {
        File weaponDataFile = new();
        weaponDataFile.Open(WeaponDataJsonPath, File.ModeFlags.Read);
        weaponData = JSON.Parse(weaponDataFile.GetAsText()).Result as Godot.Collections.Dictionary;

        ForeachContainerInData(weaponData.Values, (IEnumerable container) =>
        {
            if (container is not Godot.Collections.Dictionary dict) return;

            foreach (string key in dict.Keys)
            {
                if (key.EndsWith("Curve"))
                {
                    dict[key] = ConvertCurveData(dict[key] as Godot.Collections.Array);
                    continue;
                }

                if (dict[key] is string textData)
                {
                    if (textData.IsAbsPath())
                    {
                        dict[key] = GD.Load(textData);
                    }
                }
            }
        });
    }


    private static void ForeachValueInData(IEnumerable data, Action<object> action, bool ignoreContainers = true)
    {
        foreach (object item in data is IDictionary dict ? dict.Values : data)
        {
            if (item is IEnumerable container)
            {
                if (!ignoreContainers) action(item);
                ForeachValueInData(container, action, ignoreContainers);
                continue;
            }
            action(item);
        }
    }
    private static void ForeachContainerInData(IEnumerable data, Action<IEnumerable> action)
    {
        foreach (object item in data is IDictionary dict ? dict.Values : data)
        {
            if (item is IEnumerable container)
            {
                action(container);
                ForeachContainerInData(container, action);
            }
        }
    }
    private static Curve ConvertCurveData(Godot.Collections.Array data)
    {
        Curve curve = new();

        foreach (Godot.Collections.Dictionary pointData in data)
        {
            curve.AddPoint(
                    position: new Vector2(pointData.Get<float>("Pos"), pointData.Get<float>("Val")),
                    leftMode: Curve.TangentMode.Linear,
                    rightMode: Curve.TangentMode.Linear);
        }

        return curve;
    }
}