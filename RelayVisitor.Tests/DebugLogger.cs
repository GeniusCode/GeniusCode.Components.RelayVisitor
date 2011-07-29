using System;
using System.Diagnostics;
using GeniusCode.Framework.Support.Objects;

namespace GcCore.Tests
{
    public class DebugWriteLogger : ILog
    {
        public void Log(string level, string message, string title)
        {
            Debug.WriteLine(String.Format("{0} - {1}", title, message));
        }
    }
}
