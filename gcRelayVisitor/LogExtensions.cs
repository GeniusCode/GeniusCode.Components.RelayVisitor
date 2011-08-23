using System;

namespace GeniusCode.Components.RelayVisitor
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
