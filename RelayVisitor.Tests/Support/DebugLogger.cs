using System;
using System.Diagnostics;

namespace GeniusCode.Components.RelayVisitor.Tests.Support
{
    public class DebugWriteLogger : ILog
    {
        public void Log(string level, string message, string title)
        {
            Debug.WriteLine(String.Format("{0} - {1}", title, message));
        }
    }
}
