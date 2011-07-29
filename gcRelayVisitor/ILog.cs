using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace GeniusCode.Framework.Support.Objects
{
    public interface ILog
    {
        void Log(string level, string message, string title);
    }
}