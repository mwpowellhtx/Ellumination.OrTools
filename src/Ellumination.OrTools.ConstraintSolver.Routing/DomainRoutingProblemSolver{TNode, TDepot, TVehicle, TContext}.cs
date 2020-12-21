namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Represents a Domain specific
    /// <see cref="AssignableRoutingProblemSolver{TContext, TAssign}"/>. In which case, we happen
    /// to have knowledge are <typeparamref name="TNode"/>, <typeparamref name="TDepot"/>, and
    /// <typeparamref name="TVehicle"/>.
    /// </summary>
    /// <typeparam name="TNode">The Node type.</typeparam>
    /// <typeparam name="TDepot">The Depot type.</typeparam>
    /// <typeparam name="TVehicle">The Vehicle type.</typeparam>
    /// <typeparam name="TContext">The Context type.</typeparam>
    /// <see cref="DomainContext{TNode, TDepot, TVehicle}"/>
    /// <see cref="DomainRoutingAssignmentEventArgs{TNode, TDepot, TVehicle, TContext}"/>
    public abstract class DomainRoutingProblemSolver<TNode, TDepot, TVehicle, TContext>
        : AssignableRoutingProblemSolver<TContext
            , DomainRoutingAssignmentEventArgs<TNode, TDepot, TVehicle, TContext>>
            , IDomainRoutingProblemSolver<TNode, TDepot, TVehicle, TContext
                , DomainRoutingAssignmentEventArgs<TNode, TDepot, TVehicle, TContext>>
        where TDepot : TNode
        where TContext : DomainContext<TNode, TDepot, TVehicle>
    {
    }
}
