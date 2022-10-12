using System;
using System.Collections;
using System.Linq;
using Additions;
using Godot;

public class Database : Node
{
    private const string WeaponDataJsonPath = "res://Data/WeaponData.json";
    private const string LevelsDataJsonPath = "res://Data/Levels.json";

    public static Godot.Collections.Dictionary weaponData;
    public static Godot.Collections.Array levelsData;

    public override void _Ready()
    {
        LoadData();
    }

    public static void LoadData()
    {
        LoadWeaponData();
        LoadLevelsData();
    }

    private static void LoadWeaponData()
    {
        File weaponDataFile = new();
        weaponDataFile.Open(WeaponDataJsonPath, File.ModeFlags.Read);
        weaponData = JSON.Parse(weaponDataFile.GetAsText()).Result as Godot.Collections.Dictionary;

        ConvertCurvesAndPaths(weaponData);
    }

    private static void LoadLevelsData()
    {
        File levelsDataFile = new();
        levelsDataFile.Open(LevelsDataJsonPath, File.ModeFlags.Read);
        levelsData = JSON.Parse(levelsDataFile.GetAsText()).Result as Godot.Collections.Array;

        ConvertCurvesAndPaths(levelsData);
    }


    private static void ConvertCurvesAndPaths(ICollection data)
    {
        ForeachContainerInData(data,
                excludedDictKeys: new string[] { "CurveData" },
                action: ConvertDataInContainer);

        static void ConvertDataInContainer(ICollection container)
        {
            if (container is IDictionary dict)
            {
                foreach (object key in dict.Keys)
                {
                    dict[key] = ConvertedData(dict[key]);
                }
                return;
            }
            if (container is IList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = ConvertedData(list[i]);
                }
            }
        }
        static object ConvertedData(object data)
        {
            if (data is string textData)
            {
                string[] splittedText = textData.Split(' ');

                if (splittedText.Length < 2) return data;

                switch (splittedText[0])
                {
                    case "Load:": return GD.Load(splittedText[1]);
                }
                return data;
            }

            if (data is IDictionary dict)
            {
                if (dict.Count is 1 && dict.Contains("CurveData"))
                {
                    return ConvertCurveData(dict["CurveData"] as IList);
                }
                return data;
            }
            return data;
        }
    }
    private static void ForeachValueInData(ICollection data, Action<object> action, bool ignoreContainers = true, object[] excludedDictKeys = null)
    {
        foreach (object item in data is IDictionary dict ? dict.Values : data)
        {
            if (item is ICollection container)
            {
                if (!ignoreContainers) action(item);
                ForeachValueInData(container, action, ignoreContainers, excludedDictKeys);
                continue;
            }
            action(item);
        }
    }
    private static void ForeachContainerInData(ICollection data, Action<ICollection> action, IList excludedDictKeys = null)
    {
        if (data is IDictionary dict)
        {
            if (excludedDictKeys is null) excludedDictKeys = new Godot.Collections.Array();

            foreach (object key in dict.Keys)
            {
                if (excludedDictKeys.Contains(key)) continue;

                if (dict[key] is ICollection container)
                {
                    action(container);
                    ForeachContainerInData(container, action, excludedDictKeys);
                }
            }
            return;
        }

        foreach (object item in data)
        {
            if (item is ICollection container)
            {
                action(container);
                ForeachContainerInData(container, action, excludedDictKeys);
            }
        }
    }
    private static Curve ConvertCurveData(IList data)
    {
        Curve curve = new();

        foreach (IDictionary pointData in data)
        {
            curve.AddPoint(
                    position: new Vector2((float)pointData["Pos"], (float)pointData["Val"]),
                    leftMode: Curve.TangentMode.Linear,
                    rightMode: Curve.TangentMode.Linear);
        }

        return curve;
    }
}