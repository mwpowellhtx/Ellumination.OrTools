using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Xunit;

    public class DefaultUnaryRoutingProblemSolver : DefaultRoutingProblemSolver
    {
        /// <inheritdoc/>
        protected override void AddDimensions(Context context)
        {
            base.AddDimensions(context);

            const int coefficient = 1000;

            var count = context.InternalDimensions.AssertNotNull().Count;

            context.AddDefaultDimension<DefaultEvenDimension>(coefficient);

            context.InternalDimensions.AssertEqual(count + 1, x => x.Count);
        }
    }
}
