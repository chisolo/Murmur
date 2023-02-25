
using System;
using System.Collections.Generic;
using Max.ThirdParty;
using UnityEngine;

public class MaxChargeData
{
    public string OrderSn; //发行订单号，需要游戏判断重复，并且结束之后调用ChargeFinish(orderSn)告知sdk
    public string ConsumeExt; //消耗型商品的透传字段
    public string ProductId; //商品id
    public int ProductType; //商品类型: 1=消耗型;2=一次性;3=订阅型;
    public long Expires; //订阅商品的有效结束时间戳，单位毫秒
    public bool IsEffect; //订阅商品是否处于有效期（基于服务端时间戳判断）

    public static MaxChargeData Parse(string s)
    {
        if (s == null || s.Length == 0)
        {
            return null;
        }
        try
        {
            var dict = Json.Deserialize(s) as Dictionary<string, object>;
            var data = new MaxChargeData();
            data.OrderSn = dict["orderSn"].ToString();
            data.ConsumeExt = dict["consume_ext"].ToString();
            data.ProductId = dict["productId"].ToString();
            data.ProductType = (int)(long)dict["productType"];
            data.Expires = (long)dict["expires"];
            data.IsEffect = (bool)dict["is_effect"];
            return data;
        } catch(Exception ex)
        {
            Debug.Log("MaxChargeData exception - " + ex.Message);
        }
        return null;
    }
}