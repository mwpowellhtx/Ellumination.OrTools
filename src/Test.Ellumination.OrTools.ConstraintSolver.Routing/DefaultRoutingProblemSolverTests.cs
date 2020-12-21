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
    public class DefaultRoutingProblemSolverTests
        : DefaultRoutingProblemSolverTests<DefaultRoutingProblemSolverFixture>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DefaultRoutingProblemSolverTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Returns a Created <see cref="DefaultUnaryRoutingProblemSolver"/>.
        /// </summary>
        /// <returns></returns>
        protected override DefaultRoutingProblemSolverFixture CreateProblemSolver() =>
            new DefaultRoutingProblemSolverFixture();

        /// <inheritdoc/>
        protected override RoutingContext VerifyContext(RoutingContext context)
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
        protected override void OnVerifyAssignments(params (int vehicle, int node, int? previousNode)[] assignments)
        {
            base.OnVerifyAssignments(assignments);

            var validate = assignments.Select(pair => (
                pair.vehicle
                , pair.node
                , depot: pair.node == 0
                , even: pair.node % 2 == 0
                , odd: pair.node % 2 == 1)).ToArray();

            void ReportValidateAssignment()
            {
                const string squareBrackets = "[]";

                this.OutputHelper.WriteLine($"{nameof(validate)}: {squareBrackets[0]}");

                var i = 0;

                foreach (var (vehicle, node, depot, even, odd) in validate)
                {
                    var rendered = RenderTupleAssociates(
                        (nameof(vehicle), Render(vehicle))
                        , (nameof(node), Render(node))
                        , (nameof(depot), Render(depot))
                        , (nameof(even), Render(even))
                        , (nameof(odd), Render(odd))
                    );

                    const char comma = ',';

                    var prefix = $" /* [{i}] */ ";

                    this.OutputHelper.WriteLine($"    {(i++ == 0 ? "" : $"{comma} ")}{prefix}{rendered}");
                }

                this.OutputHelper.WriteLine($"{squareBrackets[1]}");
            }

            ReportValidateAssignment();

            var depotOrEven = validate.Select(x => x.depot || x.even).ToArray();

            var areAllDepotOrEven = depotOrEven.All(x => x);

            //// TODO: TBD: or at least this was the intention...
            //// TODO: TBD: for now we are backing off of this notion and simply verifying against the known examples on the ortools website.
            //areAllDepotOrEven.AssertTrue();
        }
    }
}
