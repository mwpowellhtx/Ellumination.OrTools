namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Visitor Applies potentially mutating changes to the
    /// <typeparamref name="TContext"/> instance. It is therefore critical that any
    /// Visitors that <see cref="Apply"/> must have done so prior to Dimensions being
    /// Added, then Solution being discovered.
    /// </summary>
    public interface IVisitor<TContext>
        where TContext : Context
    {
        TContext Apply(TContext context);
    }
}
