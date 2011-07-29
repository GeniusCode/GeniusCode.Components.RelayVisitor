using System;

namespace GeniusCode.Framework.Support.Objects
{
    public interface IVisitRouteNode
    {
        bool NextItemTypeIsIEnumerable { get; }
        Type StartType { get; }
        Type NextItemType { get; }
        object PerformVisit(object input, Func<Exception, VisitMode, bool> exceptionHandler, ILog log);
    }
}
