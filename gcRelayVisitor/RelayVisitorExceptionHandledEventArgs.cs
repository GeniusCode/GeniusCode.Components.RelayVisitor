using System;

namespace GeniusCode.Components.RelayVisitor
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
