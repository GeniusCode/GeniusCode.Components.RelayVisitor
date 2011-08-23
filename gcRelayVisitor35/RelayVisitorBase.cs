using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeniusCode.Components.RelayVisitor
{
    public abstract partial class RelayVisitorBase : IRelayVisitor
    {

        #region Virtual Members

        protected virtual void HandleNodeScoutException(Exception exception, out bool handled)
        {
            handled = false;
        }

        protected virtual void HandleNodeActionException(Exception exception, out bool handled)
        {
            handled = false;
        }
        protected virtual void HandleNodePredicateException(Exception exception, out bool handled)
        {
            handled = false;
        }
        #endregion

        #region Public Members

        public void AddScoutForType<I, O>(Func<I, O> scoutFunc, Func<I, bool> predicate)
            where I : class
            where O : class
        {
            var node = VisitRouteNodeFactory.CreateScoutNode<I, O>(scoutFunc, predicate);
            AddTracker(node);
        }

        public void AddActionForType<I>(Action<I> action, Func<I, bool> predicate) where I : class
        {
            var node = VisitRouteNodeFactory.CreateActionNode<I>(null, action);
            AddTracker(node);
        }
        public bool AllowRecursiveReferences { get; private set; }
        public int RecursionCount { get; private set; }
        public ILog Log { get; set; }
        #endregion

        #region Assets
        readonly List<IVisitRouteNode> VisitRouteNodes = new List<IVisitRouteNode>();
        #endregion

        #region Constuctors
        public RelayVisitorBase(bool allowRecursiveReferences)
        {
            AllowRecursiveReferences = allowRecursiveReferences;
        }
        #endregion

        #region Helpers
        protected void BeginStartVisiting(object startObject, Func<bool> goUntil)
        {
            // Clear recurse count
            RecursionCount = 0;
            bool breakNow = false;
            StartVisiting(startObject, goUntil, ref breakNow);
        }
        #endregion

        #region Private Members

        protected void AddTracker(IVisitRouteNode tracker)
        {
            VisitRouteNodes.Add(tracker);
        }


        private void IncrementCount()
        {
            RecursionCount++;
            Log.LogIfNotNull("Verbose", String.Format("NewCount={0}", RecursionCount), "StatusUpdate");
        }

        private List<IVisitRouteNode> GetMatchingNodesForStartType(Type startType)
        {
            var matchingNodes = (from t in VisitRouteNodes
                                 where t.StartType.IsAssignableFrom(startType)
                                 select t).ToList();
            return matchingNodes;
        }

        private bool ProxyVirtualErrorHandlers(Exception ex, VisitMode vm)
        {
            bool handled;
            switch (vm)
            {
                case VisitMode.Action:
                    HandleNodeActionException(ex, out handled);
                    break;
                case VisitMode.GetNextNode:
                    HandleNodeScoutException(ex, out handled);
                    break;
                case VisitMode.Predicate:
                    HandleNodePredicateException(ex, out handled);
                    break;
                default:
                    throw new Exception();
            }
            return handled;
        }

        private bool DetectPassGoUntilPredicate(Func<bool> goUntil, ILog log)
        {
            var passed = (goUntil == null || goUntil() == false);
            if (!passed)
                Log.LogIfNotNull("Info", "goUntil predicate has passed at RecursiveWalker.PerformObject(), recursivewalker stopping", "StatusUpdate");

            return passed;
        }

        private bool DetectPassStartObjectNullTest(object startObject, ILog log)
        {
            var passed = (startObject != null);
            if (!passed)
                Log.LogIfNotNull("Info", "Null startObject at RecursiveWalker.PerformObject()", "StatusUpdate");

            return passed;
        }


        private void StartVisiting(object startObject, Func<bool> goUntil, ref bool breakNow)
        {
            var passedGoUntilPredicate = DetectPassGoUntilPredicate(goUntil, Log);
            if (!passedGoUntilPredicate)
            {
                breakNow = true;
                return;
            }

            IncrementCount();

            var passedStartObjectNullTest = DetectPassStartObjectNullTest(startObject, Log);
            if (!passedStartObjectNullTest)
                return;


            Type startType = startObject.GetType();
            var matchingNodes = GetMatchingNodesForStartType(startType);

            Log.LogIfNotNull("Verbose", string.Format("{0} matching Trackers found for Type {1}", matchingNodes.Count, startType.Name), "StatusUpdate");

            for (int i = 0; i < matchingNodes.Count; i++)
            {
                Log.LogIfNotNull("Verbose", string.Format("Executing Tracker {0} of {1} for Type {1}", i, matchingNodes.Count() - 1, startType.Name), "StatusUpdate");

                // Get matching tracker
                IVisitRouteNode visitNode = matchingNodes[i];

                object nextNode = visitNode.PerformVisit(startObject, (ex, vm) => ProxyVirtualErrorHandlers(ex, vm), Log);

                if (nextNode != null)
                {
                    if (visitNode.NextItemTypeIsIEnumerable)
                    {
                        // either we recursive against one item, or a list
                        IEnumerable list = nextNode as IEnumerable;
                        foreach (object thing in list)
                        {
                            StartVisiting(thing, goUntil, ref breakNow);
                            if (breakNow) return;
                        }
                    }
                    else
                    {
                        StartVisiting(nextNode, goUntil, ref breakNow);
                        if (breakNow) return;
                    }
                }

            }
        }

        #endregion

        #region Protected Members
        protected void GoUntil(object startObject, Func<bool> goUntil)
        {
            Log.LogIfNotNull("Verbose", String.Format("RecursiveWalker.Go({0}) : {1} trackers in list", startObject.GetType().Name, VisitRouteNodes.Count()), "MethodEnter");
            BeginStartVisiting(startObject, goUntil);
        }
        #endregion
    }
}
