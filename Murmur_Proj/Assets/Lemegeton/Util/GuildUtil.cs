using System;
using System.Diagnostics;

namespace Lemegeton
{
    public static class GuildUtil {
        public static string NewUUID()
        {
            Guid myuuid = Guid.NewGuid();
            return myuuid.ToString();
        }
    }
}