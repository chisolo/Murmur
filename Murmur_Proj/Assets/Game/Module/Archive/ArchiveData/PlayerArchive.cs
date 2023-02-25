using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArchive : IArchive
{
    public int moneyLimit;
    public int money;

    public int coin;
    public int coupon;
    public int star;

    public long lastLoginTime;
    public int loginDay;

    public bool bgm;
    public bool sfx;

    public long lastPlayTime;
    public int lastReviewedStar;
    public void Default()
    {
        // TODO
        moneyLimit = 1000;
        money = 1000;
        money = moneyLimit;

        coin = 0;
        coin = 0;
        coupon = 0;
        loginDay = 0;

        bgm = true;
        sfx = true;

        lastReviewedStar = 0;
    }
}
