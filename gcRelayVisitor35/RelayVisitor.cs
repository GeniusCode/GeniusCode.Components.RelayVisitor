using System;

namespace GeniusCode.Components.RelayVisitor
{



    public sealed class RelayVisitor<TRoot> : RelayVisitorBase, IRelayVisitor<TRoot>
            where TRoot : class
    {

        public static void PerformSimpleRecursion(Func<TRoot, TRoot> scout, Action<TRoot> action, TRoot root)
        {
            var walker = new RelayVisitor<TRoot>(false);
            walker.AddScoutForType<TRoot,TRoot>(scout, null);
            walker.AddActionForType<TRoot>(action,null);
            walker.Go(root);
        }

        public RelayVisitor(bool allowRecursiveReferences)
            : base(allowRecursiveReferences)
        { }

        public void Go(TRoot startObject)
        {
            GoUntil(startObject, null);
        }

        public void GoUntil(TRoot startObject, Func<bool> goUntil)
        {
            base.GoUntil(startObject, goUntil);
        }

        public void AddActionForRootType(Action<TRoot> action, Func<TRoot, bool> predicate = null)
        {
            AddActionForType(action, predicate);
        }

        public void AddScoutForRootType<O>(Func<TRoot, O> scoutFunc, Func<TRoot, bool> predicate = null)
            where O: class
        {
            AddScoutForType(scoutFunc, predicate);
        }
    }
}
