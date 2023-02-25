using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyDataConfig
{
    public string type;
    public int timeout;
    public int duration;
    public float factor;
}
public class EmergencyConfig
{
    public int star;
    public float interval;
    public List<EmergencyDataConfig> emergencies;
}
