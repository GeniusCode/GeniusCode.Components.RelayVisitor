using System;

namespace GeniusCode.Components.RelayVisitor
{

    public interface IRelayVisitor<TRoot> : IRelayVisitor
            where TRoot : class
    {
        void Go(TRoot startObject);
        void GoUntil(TRoot startObject, Func<bool> goUntil);
    }

    public interface IRelayVisitor
    {
        int RecursionCount { get; }
        void AddScoutForType<I, O>(Func<I, O> scoutFunc, Func<I, bool> predicate)
            where I : class
            where O : class;
        void AddActionForType<I>(Action<I> action, Func<I, bool> predicate)
            where I : class;

        
    }

}