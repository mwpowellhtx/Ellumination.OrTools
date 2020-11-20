using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TDepot"></typeparam>
    /// <typeparam name="TVehicle"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public interface IRoutingProblemSolver<TNode, TDepot, TVehicle, TContext> : IRoutingProblemSolver<TContext>
        where TDepot : TNode
        where TContext : Context
    {
        /// <summary>
        /// Solution pairing event occurs Before Pair has been Assigned.
        /// </summary>
        event EventHandler<RoutingAssignmentEventArgs<TNode, TVehicle>> Assigning;

        /// <summary>
        /// Solution pairing event occurs at the moment of Pair Assignment.
        /// </summary>
        event EventHandler<RoutingAssignmentEventArgs<TNode, TVehicle>> Assign;

        /// <summary>
        /// Solution pairing event occurs After Pair has been Assigned.
        /// </summary>
        event EventHandler<RoutingAssignmentEventArgs<TNode, TVehicle>> Assigned;
    }
}
