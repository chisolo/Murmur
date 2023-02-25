
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Lemegeton
{
    public class NtpModule : Singleton<NtpModule>
    {
        private int _daySeconds = 86400;
        private DateTimeOffset _Now
        {
            get {
                return DateTimeOffset.Now;
            }
        }
        private DateTimeOffset _UtcNow
        {
            get {
                return _Now.ToUniversalTime();
            }
        }
        public long UtcNowSeconds
        {
            get {
                return _UtcNow.ToUnixTimeSeconds();
            }
        }
        public long UtcNowMillSeconds
        {
            get {
                return _UtcNow.ToUnixTimeMilliseconds();
            }
        }
        public long Today(int hour = 0)
        {
            var local = _Now;
            var today = local - local.TimeOfDay;
            return today.ToUnixTimeSeconds() + hour * 3600;
        }
        public long Tomorrow(int hour = 0)
        {
            return Today(hour) + _daySeconds;
        }
    }
}

