using System.Collections.Generic;
public static class EmojiType
{
    public const string Smile = "smile";
    public const string Angry = "angry";
    public const string Timeout = "timeout";
    public const string Hotdog = "hotdog";
    public const string Hamberg = "hamberg";
    public const string Pizza = "pizza";

    public static readonly Dictionary<string, string> EmojiIcon = new Dictionary<string, string>()
    {
        {EmojiType.Smile, "Assets/Res/UI/Atlas/BuildingIconAtlas/icon_bubble_smile.png"},
        {EmojiType.Angry, "Assets/Res/UI/Atlas/BuildingIconAtlas/icon_bubble_angry.png"},
        {EmojiType.Timeout, "Assets/Res/UI/Atlas/BuildingIconAtlas/icon_bubble_timeout.png"},
        {EmojiType.Hotdog, "Assets/Res/UI/Atlas/BuildingIconAtlas/build_small_hotdog.png"},
        {EmojiType.Hamberg, "Assets/Res/UI/Atlas/BuildingIconAtlas/build_small_hamberg.png"},
        {EmojiType.Pizza, "Assets/Res/UI/Atlas/BuildingIconAtlas/build_small_pizza.png"},
    };
}