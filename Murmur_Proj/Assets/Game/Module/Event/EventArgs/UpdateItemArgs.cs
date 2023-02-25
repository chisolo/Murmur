public class UpdateItemArgs : BaseEventArgs<UpdateItemArgs>
{
   public string item;
   public int old;
   public int current;
   public bool anim;
}
