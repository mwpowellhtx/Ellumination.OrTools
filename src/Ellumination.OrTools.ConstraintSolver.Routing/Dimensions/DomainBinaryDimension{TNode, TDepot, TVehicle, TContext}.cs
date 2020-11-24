namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Represents a Domain specific <see cref="BinaryDimension{TContext}"/>
    /// specialized by <typeparamref name="TNode"/>, <typeparamref name="TDepot"/>,
    /// and <typeparamref name="TVehicle"/>.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TDepot"></typeparam>
    /// <typeparam name="TVehicle"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="DomainContext{TNode, TDepot, TVehicle}"/>
    public abstract class DomainBinaryDimension<TNode, TDepot, TVehicle, TContext> : BinaryDimension<TContext>
        where TDepot : TNode
        where TContext : DomainContext<TNode, TDepot, TVehicle>
    {
        /// <inheritdoc/>
        protected DomainBinaryDimension(TContext context, int coefficient, bool shouldRegister = true)
            : base(context, coefficient, shouldRegister)
        {
        }
    }
}
