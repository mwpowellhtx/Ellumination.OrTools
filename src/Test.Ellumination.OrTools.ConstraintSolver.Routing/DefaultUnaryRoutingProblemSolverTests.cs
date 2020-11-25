using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    /// <summary>
    /// 
    /// </summary>
    public class DefaultUnaryRoutingProblemSolverTests
        : DefaultRoutingProblemSolverTests<DefaultUnaryRoutingProblemSolver>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DefaultUnaryRoutingProblemSolverTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Returns a Created <see cref="DefaultUnaryRoutingProblemSolver"/>.
        /// </summary>
        /// <returns></returns>
        protected override DefaultUnaryRoutingProblemSolver CreateProblemSolver() =>
            new DefaultUnaryRoutingProblemSolver();

        /// <inheritdoc/>
        protected override Context VerifyContext(Context context)
        {
            context = base.VerifyContext(context);

            var context_Depot = context.Depot.AssertNotNull().AssertEqual(default(int));

            // Which across the Range of Coordinates must Each be Equal to the Default.
            bool OnVerifyEachEndpointCoord(int coord) => coord == default;

            context.Endpoints.SelectMany(OnDecoupleEndpoint)
                .Distinct().AssertTrue(x => x.All(OnVerifyEachEndpointCoord));

            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        [Background]
        public void DefaultUnaryRoutingProblemSolverBackground()
        {
            $"Verify this.{nameof(Context)}".x(() => this.VerifyContext(this.Context));
        }

        /// <inheritdoc/>
        protected override void OnVerifySolution(params (int vehicle, int node)[] solution)
        {
            base.OnVerifySolution(solution);

            var validate = solution.Select(pair => (
                pair.vehicle
                , pair.node
                , depot: pair.node == 0
                , even: pair.node % 2 == 0)).ToArray();

            var depotOrEven = validate.Select(x => x.depot || x.even).ToArray();

            var areAllDepotOrEven = depotOrEven.All(x => x);

            areAllDepotOrEven.AssertTrue();
        }
    }
}
