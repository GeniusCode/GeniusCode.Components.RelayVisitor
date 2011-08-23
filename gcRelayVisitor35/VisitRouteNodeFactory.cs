using System;

namespace GeniusCode.Components.RelayVisitor
{
    public static class VisitRouteNodeFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisitRouteNode&lt;I, O&gt;"/> class that is able to switch visit types from one
        /// type of item to another
        /// </summary>
        /// <param name="getNextItemFunc">The get next item func.</param>
        /// <param name="nodePredicate">The node predicate.</param>
        public static IVisitRouteNode CreateScoutNode<I, O>(Func<I, O> getNextItemFunc, Func<I, bool> nodePredicate)
            where I : class
            where O : class
        {
            return new VisitRouteNode<I, O>(getNextItemFunc, nodePredicate, null);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="VisitRouteNode&lt;I, O&gt;"/> class that assigns actions to a type of item
        /// </summary>
        /// <param name="nodePredicate">The node predicate.</param>
        /// <param name="itemAction">The item action.</param>
        public static IVisitRouteNode CreateActionNode<I>(Func<I, bool> nodePredicate, Action<I> itemAction) where I : class
        {
            return new VisitRouteNode<I, object>(null, nodePredicate, itemAction);
        }
    }
}
