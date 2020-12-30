using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// An <see cref="IAssignableRoutingProblemSolver{TContext, TAssign}"/> interface
    /// with domain model specific knowledge given <typeparamref name="TNode"/>,
    /// <typeparamref name="TVehicle"/>, <typeparamref name="TNode"/>, etc.
    /// </summary>
    /// <typeparam name="TNode">The Node type.</typeparam>
    /// <typeparam name="TDepot">The Depot type.</typeparam>
    /// <typeparam name="TVehicle">The Vehicle type.</typeparam>
    /// <typeparam name="TContext">The Context type.</typeparam>
    /// <typeparam name="TAssign">The
    /// <see cref="DomainRoutingAssignmentEventArgs{TNode, TDepot, TVehicle, TContext}"/>
    /// type.</typeparam>
    /// <see cref="DomainContext{TNode, TDepot, TVehicle}"/>
    /// <see cref="DomainRoutingAssignmentEventArgs{TNode, TDepot, TVehicle, TContext}"/>
    public interface IDomainRoutingProblemSolver<TNode, TDepot, TVehicle, TContext, TAssign>
        : IAssignableRoutingProblemSolver<TContext, TAssign>
        where TDepot : TNode
        where TContext : DomainContext<TNode, TDepot, TVehicle>
        where TAssign : DomainRoutingAssignmentEventArgs<TNode, TDepot, TVehicle, TContext>
    {
    }
}
