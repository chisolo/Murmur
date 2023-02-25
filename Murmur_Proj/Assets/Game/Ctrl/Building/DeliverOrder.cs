using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DeliverOrder
{
    public string buildingId;
    public int sausage;
    public int cheese;
    public int flour;

    public void Add(int s, int c, int f)
    {
        sausage += s;
        cheese += c;
        flour += f;
    }
    public bool IsEmpty()
    {
        return sausage == 0 && cheese == 0 && flour == 0;
    }
    public static DeliverOrder Get(string b, int s, int c, int f)
    {
        DeliverOrder order = GenericPool<DeliverOrder>.Get();
        order.buildingId = b;
        order.sausage = s;
        order.cheese = c;
        order.flour = f;
        return order;
    }
    public void Return()
    {
        buildingId = string.Empty;
        sausage = 0;
        cheese = 0;
        flour = 0;
        GenericPool<DeliverOrder>.Release(this);
    }
}
