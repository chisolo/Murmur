
using System;
using System.Collections.Generic;
using Max.ThirdParty;
using UnityEngine;

public class MaxChargeProductData
{
    public string ProductId; //商品Id
    public string Money;//金额，带货币符号的格式化金额(进行了本地货币格式化，比如带有','等)建议直接使用
    public string Num;//金额纯数字
    public string Currency;//币种

    public static List<MaxChargeProductData> Parse(string s)
    {
        if (s == null || s.Length == 0)
        {
            return new List<MaxChargeProductData>();
        }
        var ret = new List<MaxChargeProductData>();
        try
        {
            var list = Json.Deserialize(s) as List<object>;
            foreach (var v in list)
            {
                var dict = v as Dictionary<string, object>;
                var data = new MaxChargeProductData();
                data.ProductId = dict["productId"].ToString();
                data.Money = dict["money"].ToString();
                data.Num = dict["num"].ToString();
                data.Currency = dict["currency"].ToString();
                ret.Add(data);
            }
        } catch(Exception ex)
        {
            Debug.Log("MaxChargeProductData exception - " + ex.Message + ", " + s);
        }
        return ret;
    }
}