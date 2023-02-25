using UnityEngine;

public enum ItemEffectType
{
    SmallMoney,
    BigMoney,
    BigCoin
    
}
public class ItemEffectEventArgs : BaseEventArgs<ItemEffectEventArgs>
{
   public ItemEffectType effect;
   public Vector3 pos;
}
