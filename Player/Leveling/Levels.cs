namespace Leveling;

using System.Collections;
using System.Linq;
using Additions;
using Buffs;
using Godot;

public class Levels : Node
{
    public override void _Ready()
    {
        for (int i = 0; i < Database.levelsData.Count; i++)
        {
            var lvlData = Database.levelsData[i] as IDictionary;

            Level level = new()
            {
                xpToLevelUp = (int)lvlData.GetOrDefault<float>("XpToLevelUp", 0),
                explanation = lvlData.GetOrDefault<string>("Explanation", string.Empty),
                noMenu = lvlData.GetOrDefault<bool>("NoMenu", false),
                stopEnemySpawningTime = lvlData.GetOrDefault<float>("StopEnemySpawningTime", 15),
                Name = $"Level {i}"
            };

            AddChild(level);

            foreach (IDictionary buffData in lvlData.GetOrDefault<IList>("Buffs", new IDictionary[0]))
            {
                LevelBuff buff = NewBuffFromData(buffData);

                buff.dontShow = buffData.GetOrDefault<bool>("Hide", false);
                buff.overridingText = buffData.GetOrDefault<string>("Text", string.Empty);

                level.AddChild(buff);
            }
        }
    }

    private LevelBuff NewBuffFromData(IDictionary data)
    {
        switch (data.GetOrDefault<string>("Type", string.Empty))
        {
            case nameof(GameMethodCall):
                return new GameMethodCall()
                {
                    methodName = data.Get<string>("Method"),
                    args = data.GetOrDefault<Godot.Collections.Array>("Args", new())
                };

            case nameof(GamePropertySet):
                return new GamePropertySet()
                {
                    propertyName = data.Get<string>("property"),
                    value = data.Get<object>("Value")
                };

            case nameof(ShopItem):
                return new ShopItem()
                {
                    itemName = data.Get<string>("ItemName")
                };

            case nameof(Coins):
                return new Coins()
                {
                    coins = (int)data.Get<float>("Amount")
                };

            case nameof(AddDice):
                var diceData = data.Get<IDictionary>("Dice");
                PackedScene[] diceScenes = new PackedScene[diceData.Get<IList>("Scenes").Count];
                diceData.Get<IList>("Scenes").CopyTo(diceScenes, 0);
                return new AddDice()
                {
                    diceScene = diceData.Get<PackedScene>("Scene"),
                    diceScenes = diceScenes
                };

            case nameof(MusicChange):
                return new MusicChange()
                {

                };

            default:
                return new JustText();
        }
    }
}
