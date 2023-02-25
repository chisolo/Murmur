using System.Collections.Generic;

public class NotificationContent
{
    public string title;
    public string text;
}
public class NotificationConfig
{
    public int hour;
    public int minute;
    public List<NotificationContent> contents = new List<NotificationContent>();
}
