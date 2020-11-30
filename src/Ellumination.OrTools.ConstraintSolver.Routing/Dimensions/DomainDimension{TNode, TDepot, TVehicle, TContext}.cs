namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Represents a Domain specific <see cref="Dimension"/> specialized by
    /// <typeparamref name="TNode"/>, <typeparamref name="TDepot"/>, and
    /// <typeparamref name="TVehicle"/>.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TDepot"></typeparam>
    /// <typeparam name="TVehicle"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="DomainContext{TNode, TDepot, TVehicle}"/>
    public abstract class DomainDimension<TNode, TDepot, TVehicle, TContext>
        : Dimension
            , IDomainDimension<TNode, TDepot, TVehicle, TContext>
        where TDepot : TNode
        where TContext : DomainContext<TNode, TDepot, TVehicle>
    {
        /// <summary>
        /// Gets the Context associated with the Dimension.
        /// </summary>
        protected new virtual TContext Context => base.Context as TContext;

        /// <inheritdoc/>
        protected DomainDimension(TContext context, int coefficient)
            : base(context, coefficient)
        {
        }
    }
}
