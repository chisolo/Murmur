using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IngredientOrder
{
    public string buildingId;
    public string ingredient;
    public int amount;
    public IngredientOrder(string b, string i, int a) {
        buildingId = b;
        ingredient = i;
        amount = a;
    }
    public void Take(int count)
    {
        if(amount > count) amount -= count;
        else amount = 0;
    }
    public void Return(int count)
    {
        amount += count;
    }
}
