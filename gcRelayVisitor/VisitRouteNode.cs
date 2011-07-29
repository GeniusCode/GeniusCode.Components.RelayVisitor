using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeniusCode.Framework.Support.Objects
{

    internal class VisitRouteNode<I, O> : IVisitRouteNode
        where I : class
        where O : class
    {

        internal VisitRouteNode(Func<I, O> scoutDelegate, Func<I, bool> nodePredicate, Action<I> itemAction)
        {
            ScoutDelegate = scoutDelegate;
            NodePredicate = nodePredicate;
            ItemAction = itemAction;

            ItemHashCodesVisited = new List<int>();
            NextItemTypeIsIEnumerable = (typeof(IEnumerable).IsAssignableFrom(typeof(O)));
        }

        #region Assets
        public Func<I, O> ScoutDelegate { get; set; }
        public Action<I> ItemAction { get; set; }
        public Func<I, bool> NodePredicate { get; set; }
        readonly List<int> ItemHashCodesVisited;

        
        public int ItemsVisitedCount
        {
            get
            {
                return ItemHashCodesVisited.Count;
            }
        }

        public bool WasItemAlreadyVisited(I item)
        {
           int hashcode = item.GetHashCode();
           var alreadyVisited = ItemHashCodesVisited.Contains(hashcode);

           if (!alreadyVisited)
               ItemHashCodesVisited.Add(hashcode);

           return alreadyVisited;
        }

        #endregion

        /// <summary>
        /// next item type is IEnumerable (a collection), or
        /// just one object
        /// </summary>
        public bool NextItemTypeIsIEnumerable { get; private set; }


        #region ITracker Members

        public Type StartType
        {
            get { return typeof(I); }
        }

        public Type NextItemType
        {
            get { return typeof(O); }
        }
        #endregion


        private bool DetectPassNullStartObjectTest(I startObject, ILog log)
        {
            bool passed = (startObject != null);

            if(!passed)
                log.LogIfNotNull("Info", string.Format("Null Object given to TrackerGo({0}) - recursive result type: {1}", StartType.Name, NextItemType.Name), "StatusUpdate");

            return passed;
        }

        private bool DetectItemWasNotAlreadyVisited(I startObject, ILog log)
        {
            bool passed = (!WasItemAlreadyVisited(startObject));

            if (!passed)
                log.LogIfNotNull("Info", "Start Object already processed, item skipped", "StatusUpdate");

            return passed;
        }


        private O Visit(I startObject, Func<Exception, VisitMode, bool> exceptionHandler, ILog Log)
        {
            var passedNullStartObjectTest = DetectPassNullStartObjectTest(startObject, Log);
            if (!passedNullStartObjectTest) 
                return null;

            Log.LogIfNotNull("Verbose", String.Format("RecursiveWalker.Go({0}) - recursive result type - {1}", StartType.Name, NextItemType.Name), "MethodEnter");
            O nextLayer = null;

            var passedItemAlreadyVisitedTest = DetectItemWasNotAlreadyVisited(startObject, Log);
            if (!passedItemAlreadyVisitedTest)
                return null;

            Log.LogIfNotNull("Verbose", String.Format("Object added to List of executed object for tracker.  Current count: {0}", ItemsVisitedCount), "StatusUpdate");


            //Test Predicate
            bool shouldPerformAction = DetectShouldPerformAction(startObject, exceptionHandler, Log);

            // Invoke actions if predicate passes
            if (shouldPerformAction)
            {
                try
                {
                    ItemAction(startObject);
                    Log.LogIfNotNull("Verbose", "Action {0} invoked against object", "StatusUpdate");
                }
                catch (Exception ex)
                {
                    bool handled = exceptionHandler(ex, VisitMode.Action);
                    if (!handled) throw ex;
                }
            }

            var shouldScoutNextItem = DetectShouldScoutNextItem(startObject,exceptionHandler);
            if (shouldScoutNextItem)
            {
                try
                {
                        Log.LogIfNotNull("Verbose", "Preparing To Get Next Recursive Layer Using Func {0}", "StatusUpdate");
                        nextLayer = ScoutDelegate(startObject);
                        Log.LogIfNotNull("Verbose", String.Format("Next layer returned - {0}", NextItemType.Name), "StatusUpdate");
                }
                catch (Exception ex)
                {
                    bool handled = exceptionHandler(ex, VisitMode.Action);
                    if (!handled) throw ex;
                }
            }

            Log.LogIfNotNull("Verbose", "RecursiveWalker.Go()", "MethodExit");
            return nextLayer;
        }

        private bool DetectShouldPerformAction(I startObject, Func<Exception, VisitMode, bool> exceptionHandler, ILog Log)
        {
            if (ItemAction == null)
                return false;

            bool shouldPerformAction = false;

            if (NodePredicate != null)
            {
                try
                {
                    shouldPerformAction = NodePredicate(startObject);
                }
                catch (Exception ex)
                {
                    bool handled = exceptionHandler(ex, VisitMode.Predicate);
                    if (!handled) throw ex;
                }
            }
            else
            {
                shouldPerformAction = true;
            }
            return shouldPerformAction;
        }


        private bool DetectShouldScoutNextItem(I startObject, Func<Exception, VisitMode, bool> exceptionHandler)
        {
            if (ScoutDelegate == null)
                return false;

            bool shouldScoutNextItem = false;

            if (NodePredicate != null)
            {
                try
                {
                    shouldScoutNextItem = NodePredicate(startObject);
                }
                catch (Exception ex)
                {
                    bool handled = exceptionHandler(ex, VisitMode.Predicate);
                    if (!handled) throw ex;
                }
            }
            else
            {
                shouldScoutNextItem = true;
            }
            return shouldScoutNextItem;
        }

        public object PerformVisit(object input, Func<Exception, VisitMode, bool> exceptionHandler, ILog log)
        {
            return Visit((I)input, exceptionHandler, log);
        }
    }
}
