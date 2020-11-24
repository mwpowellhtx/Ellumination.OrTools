namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <inheritdoc/>
    public abstract class DefaultBinaryDimension : BinaryDimension<Context>
    {
        /// <inheritdoc/>
        protected DefaultBinaryDimension(Context context, int coefficient, bool shouldRegister = true)
            : base(context, coefficient, shouldRegister)
        {
        }
    }
}
