using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotConfig
{
    public string id;
    public Vector3 position;
    public Quaternion rotation;
    public bool strict;

    public static SpotConfig Default()
    {
        return new SpotConfig() { id = "not_found_spot", position = Vector3.zero, rotation = Quaternion.identity};
    }

    public void Dump()
    {
        Debug.Log("[Spot] Id = " + id + ", position = " + position);
    }
}