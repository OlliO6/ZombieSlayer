using Godot;
using System;

public class ShopItem : Button
{
    [Export] public PackedScene sceneToBuy;
    [Export] private int startPrice = 10;
    [Export] private int maxBuyCount = 1;
    [Export] private int endPrice = 10;

    private int buyCount;

    public int GetPrice()
    {
        if (buyCount >= maxBuyCount) return -1;

        return Mathf.RoundToInt(Mathf.Lerp(startPrice, endPrice, (float)buyCount / (float)maxBuyCount));
    }



    [TroughtSignal]
    private void OnButtonDown()
    {
        
    }
}
