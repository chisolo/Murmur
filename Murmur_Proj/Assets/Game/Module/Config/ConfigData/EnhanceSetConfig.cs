using System.Collections.Generic;

public struct Bonus
{
    public string item;
    public int amount;
}
public class EnhanceSetConfig
{
    public string id;
    public List<string> enhances;
    public List<Bonus> bonus;
}
