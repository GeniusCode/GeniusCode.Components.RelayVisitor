using System;

namespace GeniusCode.Framework.Support.Objects
{
    public static class LogExtensions
    {
        public static void LogIfNotNull(this ILog log, string level, string message, string title)
        {
            if (log != null)
            {
                log.Log(level, message, title);
            }
        }
    }
}
