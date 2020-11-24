namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <inheritdoc/>
    public abstract class DefaultUnaryDimension : UnaryDimension<Context>
    {
        /// <inheritdoc/>
        protected DefaultUnaryDimension(Context context, int coefficient, bool shouldRegister = true)
            : base(context, coefficient, shouldRegister)
        {
        }
    }
}
