using System.Collections.Generic;

public class SeasonBonus
{
    public int exp;
    public int type;
    public int amount;
    public int param;
    public int pass_type;
    public int pass_amount;
    public int pass_param;
}
public class SeasonConfig
{
    public string id;
    public int exp;
    public int pass_exp;
    public string service;
    public float interval;
    public int duration;
    public string guest_id;
    public string button_icon;
    public SeasonBonus[] bonus;

}
