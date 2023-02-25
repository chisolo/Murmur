using System;
using UnityEngine;

namespace Lemegeton
{
    public static class FormatUtil
    {
        public static string Currency(int currency, bool outcome = true)
        {
            if(currency <= 0 && outcome) return "FREE".Locale();
            return string.Format("{0:#,###0}", currency);
        }
        public static string Revenue(int currency, bool noclr = true)
        {
            if(noclr) string.Format("+{0:#,###0}", currency);
            return string.Format("<color=#16C771>+{0:#,###0}</color>", currency);
        }
        public static string Salary(int currency, bool noclr = true)
        {
            if(noclr) return string.Format("-{0:#,###0}", currency);;
            return string.Format("<color=#F1741E>-{0:#,###0}</color>", currency);
        }
        public static string FormatTimeLong(long time)
        {
            var remain = TimeSpan.FromSeconds(time);
            if(time >= 84000) return string.Format("{0}d {1}h {2}m", remain.Days, remain.Hours, remain.Minutes);
            else if(time >= 3600) return string.Format("{0}h {1}m {2}s", remain.Hours, remain.Minutes, remain.Seconds);
            else if(time >= 60) return string.Format("{0}m {1}s", remain.Minutes, remain.Seconds);
            else if (time <= 0) return "0s";
            else return string.Format("{0}s", remain.Seconds);
        }
        public static string FormatTimeShort(long time)
        {
            var remain = TimeSpan.FromSeconds(time);
            if(time >= 84000) return string.Format("{0}d {1}h", remain.Days, remain.Hours);
            if(time >= 3600) return string.Format("{0}h {1}m", remain.Hours, remain.Minutes);
            else if(time >= 60) return string.Format("{0}m {1}s", remain.Minutes, remain.Seconds);
            else return string.Format("{0}s", remain.Seconds);
        }
        public static string FormatTimeAuto(long time)
        {
            var remain = TimeSpan.FromSeconds(time);
            if(remain.Days > 0) {
                if(remain.Hours > 0) {
                    if(remain.Minutes > 0) return string.Format("{0}d {1}h {2}m", remain.Days, remain.Hours, remain.Minutes);
                    return string.Format("{0}d {0}h", remain.Days, remain.Hours);
                } else {
                    if(remain.Minutes > 0) return string.Format("{0}d {1}h {2}m", remain.Days, remain.Hours, remain.Minutes);
                    return string.Format("{0}d", remain.Days);
                }
            } else if(remain.Hours > 0) {
                if(remain.Minutes > 0) {
                    if(remain.Seconds > 0) return string.Format("{0}h {1}m {2}s", remain.Hours, remain.Minutes, remain.Seconds);
                    else return string.Format("{0}h {0}m", remain.Hours, remain.Minutes);
                } else {
                    if(remain.Seconds > 0) return string.Format("{0}h {1}m {2}s", remain.Hours, remain.Minutes, remain.Seconds);
                    else return string.Format("{0}h", remain.Hours);
                }
            } else if(remain.Minutes > 0) {
                if(remain.Seconds > 0) return string.Format("{0}m {1}s", remain.Minutes, remain.Seconds);
                else return string.Format("{0}m", remain.Minutes);
            } else {
                return string.Format("{0}s", remain.Seconds);
            }
        }
    }
}

