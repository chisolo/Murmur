using System.Collections.Generic;

public class PurchaseOrderInfo
{
    public string productId;
    public string orderSn;
}

public class PlayerRight
{
    public string productId;
}

public class PurchaseArchice : IArchive
{
    public Dictionary<string, PurchaseOrderInfo> purchaseOrderData;
    public Dictionary<string, PlayerRight> playerOwnedRights;

    public virtual void Default()
    {
        purchaseOrderData = new Dictionary<string, PurchaseOrderInfo>();
        playerOwnedRights = new Dictionary<string, PlayerRight>();
    }
}