using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Xunit;

    public class DefaultRoutingProblemSolverFixture : DefaultRoutingProblemSolver
    {
        /// <inheritdoc/>
        protected override void AddDimensions(RoutingContext context)
        {
            base.AddDimensions(context);

            const int coefficient = 1000;

            var count = context.InternalDimensions.AssertNotNull().Count;

            context.AddDefaultDimension<ThirdsDimension>(coefficient);

            context.InternalDimensions.AssertEqual(count + 1, x => x.Count);
        }
    }
}
