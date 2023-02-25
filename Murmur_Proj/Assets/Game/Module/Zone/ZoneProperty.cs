using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ZoneProperty
{
    public const string LvlUpDuration = "r_lvlup_duration"; // 升级时间
    public const string LvlUpCost = "r_lvlup_cost"; // 升级花费
    public const string MoneyLimit = "v_money_limit";
    public const string ServiceEffy = "r_service_effy"; // 服务时间
    public const string WaitingCapacity = "v_waiting_capacity"; //
    public const string ServiceCapacity = "v_service_capacity";
    public const string ProductCapacity = "v_product_capacity";
    public const string Revenue = "r_revenue"; // 收益
    public const string ItemLvlupDiscount = "v_item_lvlup_discount"; // enhance升级消耗
    public const string MoveSpeed = "r_move_speed"; // 员工移动速度
    public const string InstantService = "v_instance_service"; // 瞬间完成的几率, 此处务必注意，虽然叫几率，但是他实际是一个百分比的值，加成是加成这个百分比的值
    public const string Product1Sub = "v_product1_sub"; 
    public const string Product2Sub = "v_product2_sub"; 
    public const string Product3Sub = "v_product3_sub"; 
    public const string WaitingTimeout = "r_waiting_timeout"; // 等候超时
    public const string ExtraRevenueNail = "v_extra_revenue_nail";
    public const string ExtraRevenueHair = "v_extra_revenue_hair";
    public const string ExtraRevenueSpa = "v_extra_revenue_spa";
    public const string RevenueDouble = "v_revenue_double"; // 
    public const string RevenueTriple = "v_revenue_triple";
    public const string NoProductRequire = "v_no_product_require";
    public const string GuestRate = "v_guest_rate";
    public const string GuestInterval = "v_guest_interval";
}