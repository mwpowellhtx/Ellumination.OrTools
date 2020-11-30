namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Represents a <see cref="IDimension{TContext}"/> specialized for
    /// <typeparamref name="TNode"/>, <typeparamref name="TDepot"/> and
    /// <typeparamref name="TVehicle"/>.
    /// </summary>
    public interface IDomainDimension<TNode, TDepot, TVehicle, TContext> : IDimension<TContext>
        where TDepot : TNode
        where TContext : DomainContext<TNode, TDepot, TVehicle>
    {
    }
}
