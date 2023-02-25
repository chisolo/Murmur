using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyData
{
    public EmergencyDataConfig config;
    public EmergencyState state; 
    public int bonus;

    public EmergencyData(EmergencyDataConfig config)
    {
        this.config = config;
        state = EmergencyState.Ready;
        bonus = (int)(config.factor * BuildingModule.Instance.GetTotalProfit());
    }
}
