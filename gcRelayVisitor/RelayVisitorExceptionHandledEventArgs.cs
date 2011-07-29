using System;

namespace GeniusCode.Framework.Support.Objects
{
    public class RelayVisitorExceptionHandledEventArgs : EventArgs
    {
        public RelayVisitorExceptionHandledEventArgs(Exception ex, VisitMode mode)
            : base()
        {
            WalkerMode = mode;
        }

        public VisitMode WalkerMode { get; private set; }
        public bool IsHandled { get; set; }
    }
}
