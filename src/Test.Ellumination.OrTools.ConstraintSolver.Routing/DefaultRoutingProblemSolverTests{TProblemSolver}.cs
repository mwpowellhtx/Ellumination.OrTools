using System;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    /// <summary>
    /// 
    /// </summary>
    public abstract class DefaultRoutingProblemSolverTests<TProblemSolver>
        : TestFixtureBase<Context, DefaultRoutingAssignmentEventArgs, TProblemSolver>
        where TProblemSolver : AssignableRoutingProblemSolver<Context, DefaultRoutingAssignmentEventArgs>
    {
        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        protected DefaultRoutingProblemSolverTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Returns a Created <see cref="Context"/> with <c>13</c> <em>Nodes</em>,
        /// <c>3</c> <em>Vehicles</em>, and a single <c>0</c> <em>Depot</em>.
        /// </summary>
        /// <returns></returns>
        protected override Context CreateContext() => new Context(13, 3);

        /// <inheritdoc/>
        protected override void OnVerifySolution(params (int vehicle, int node)[] solution)
        {
            base.OnVerifySolution(solution);

            // We are expecting a solution involving all three vehicles.
            var expectedVehicles = Range(0, 1, 2);

            solution.Select(x => x.vehicle).Distinct()
                .OrderBy(x => x).AssertCollectionEqual(expectedVehicles);
        }

        /// <summary>
        /// Performs some Background verification for the fixture.
        /// </summary>
        [Background]
        public void DefaultRoutingProblemSolverBackground()
        {
            $"this.{nameof(Context)}.{nameof(Context.NodeCount)} is correct".x(() => this.Context.NodeCount.AssertEqual(13));

            $"this.{nameof(Context)}.{nameof(Context.VehicleCount)} is correct".x(() => this.Context.VehicleCount.AssertEqual(3));
        }
    }
}
