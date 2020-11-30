namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Represents a <typeparamref name="TContext"/> specific <see cref="IDimension"/>.
    /// </summary>
    public interface IDimension<TContext> : IDimension
        where TContext : Context
    {
    }
}
