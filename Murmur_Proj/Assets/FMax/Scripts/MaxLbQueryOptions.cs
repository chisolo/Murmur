using System;
using System.Collections.Generic;
using Max.ThirdParty;

public class MaxLbQueryOptions
{
    public enum QueryType
    {
        Global = 0
    }

    public int Size;
    public QueryType Type = QueryType.Global;
    public int NumOfBeforeUser = -1;

    public MaxLbQueryOptions(int size, int numOfBeforeUser = -1)
    {
        this.Size = size;
        this.NumOfBeforeUser = numOfBeforeUser;
    }

    public override string ToString()
    {
        var dict = new Dictionary<string, object>();
        dict.Add("size", Size);
        dict.Add("type", Type.ToString().ToLower());
        dict.Add("numOfBeforeUser", NumOfBeforeUser);
        return Json.Serialize(dict);
    }
}

