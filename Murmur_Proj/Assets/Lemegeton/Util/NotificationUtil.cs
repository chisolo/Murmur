#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
using System;

public static class NotificationUtil
{
    public static void Register(string channelId, string channelName, string channelDesc)
    {
    #if UNITY_ANDROID
        var channel = new AndroidNotificationChannel() {
            Id = channelId,
            Name = channelName,
            Importance = Importance.High,
            Description = channelDesc,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    #endif
    }

    public static void CancelAll()
    {
    #if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();
    #elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        iOSNotificationCenter.ApplicationBadge = 0;
    #endif
    }
    public static void CancelDelivered()
    {
    #if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
    #elif UNITY_IOS
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        iOSNotificationCenter.ApplicationBadge = 0;
    #endif
    }
    public static void Schedule(string title, string content, int hour, int minute, int badgeCount, string channelId = "", bool repeat = true)
    {
    #if UNITY_ANDROID
        DateTime localNow = DateTime.Now;
        DateTime fireTime = new DateTime(localNow.Year, localNow.Month, localNow.Day, hour, minute, 0);
        if(fireTime < localNow) fireTime = fireTime.AddDays(1);

        var notification = new AndroidNotification
        {
            Title = title,
            Text = content,
            Number = badgeCount,
            SmallIcon = "icon_notification_small",
            LargeIcon = "icon_notification_large",
            ShowInForeground = false,
            FireTime = fireTime,
        };
        if(repeat) notification.RepeatInterval = TimeSpan.FromDays(1);

        AndroidNotificationCenter.SendNotification(notification, channelId);
    #elif UNITY_IOS
        iOSNotificationCenter.ScheduleNotification(new iOSNotification()
        {
            Identifier = $"_notification_{badgeCount}",
            Title = title,
            Body = content,
            ShowInForeground = false,
            Badge = badgeCount,
            Trigger = new iOSNotificationCalendarTrigger()
            {
                Hour = hour,
                Minute = minute,
                Repeats = repeat
            }
        });
    #endif
    }

}
