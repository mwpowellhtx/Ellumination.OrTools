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
    public abstract class RoutingProblemSolver<TNode, TDepot, TVehicle, TContext>
        : RoutingProblemSolver<TContext>
            , IRoutingProblemSolver<TNode, TDepot, TVehicle, TContext>
        where TContext : Context<TNode, TDepot, TVehicle>
        where TDepot : TNode
    {
        /// <inheritdoc/>
        public virtual event EventHandler<RoutingAssignmentEventArgs<TNode, TVehicle>> Assigning;

        /// <inheritdoc/>
        public virtual event EventHandler<RoutingAssignmentEventArgs<TNode, TVehicle>> Assign;

        /// <inheritdoc/>
        public virtual event EventHandler<RoutingAssignmentEventArgs<TNode, TVehicle>> Assigned;

        /// <summary>
        /// Delegates the <paramref name="e"/> payload to the <see cref="Assigning"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAssigning(RoutingAssignmentEventArgs<TNode, TVehicle> e) =>
            this.Assigning?.Invoke(this, e);

        /// <summary>
        /// Delegates the <paramref name="e"/> payload to the <see cref="Assigned"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAssigned(RoutingAssignmentEventArgs<TNode, TVehicle> e) =>
            this.Assigned?.Invoke(this, e);

        /// <summary>
        /// Delegates the <paramref name="node"/> and <paramref name="vehicle"/> to
        /// <see cref="Assigning"/>, <see cref="Assign"/> and <see cref="Assigned"/>
        /// events given <see cref="RoutingAssignmentEventArgs{TNode, TVehicle}"/> payload.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="vehicle"></param>
        protected virtual void OnAssign(TNode node, TVehicle vehicle)
        {
            var args = new RoutingAssignmentEventArgs<TNode, TVehicle>(node, vehicle);

            this.OnAssigning(args);

            this.Assign?.Invoke(this, args);

            this.OnAssigned(args);
        }
    }
}
