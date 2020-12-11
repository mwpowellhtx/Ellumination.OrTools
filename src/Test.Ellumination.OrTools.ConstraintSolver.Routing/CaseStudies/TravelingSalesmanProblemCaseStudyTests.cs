using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    // TODO: TBD: ditto DistanceMatrix refactoring to collections suite...
    using DistanceMatrix = Distances.DistanceMatrix;

    /// <summary>
    /// Tests involving a <see cref="DefaultRoutingProblemSolver"/>. We do not care about <em>Location</em>, per se, apart from documentation purposes. In a real world scenario, we might promote that concern to an actual <see cref="Distances.LocationDistanceMatrix"/>, however.
    /// 
    /// <para>Our model represents the distances using the following indices, <c>0</c> being the <em>depot</em>.</para>
    /// 
    /// <para><list type="table">
    /// <listheader>
    /// <node>Node</node>
    /// <location>Location</location>
    /// <depot>Depot (default: <c>false</c>)</depot>
    /// </listheader>
    /// <item>
    /// <node>0</node>
    /// <location>New York</location>
    /// <depot>true</depot>
    /// </item>
    /// <item>
    /// <node>1</node>
    /// <location>Los Angeles</location>
    /// </item>
    /// <item>
    /// <node>2</node>
    /// <location>Chicago</location>
    /// </item>
    /// <item>
    /// <node>3</node>
    /// <location>Minneapolis</location>
    /// </item>
    /// <item>
    /// <node>4</node>
    /// <location>Denver</location>
    /// </item>
    /// <item>
    /// <node>5</node>
    /// <location>Dallas</location>
    /// </item>
    /// <item>
    /// <node>6</node>
    /// <location>Seattle</location>
    /// </item>
    /// <item>
    /// <node>7</node>
    /// <location>Boston</location>
    /// </item>
    /// <item>
    /// <node>8</node>
    /// <location>San Francisco</location>
    /// </item>
    /// <item>
    /// <node>9</node>
    /// <location>St. Louis</location>
    /// </item>
    /// <item>
    /// <node>10</node>
    /// <location>Houston</location>
    /// </item>
    /// <item>
    /// <node>11</node>
    /// <location>Phoenix</location>
    /// </item>
    /// <item>
    /// <node>12</node>
    /// <location>Salt Lake City</location>
    /// </item>
    /// </list></para>
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/tsp"/>
    public class TravelingSalesmanProblemCaseStudyTests : OrToolsRoutingCaseStudyTests<DistanceMatrix
        , RoutingContext, TravelingSalesmanProblemCaseStudyTests.TravelingSalesmanCaseStudyScope>
    {
        public TravelingSalesmanProblemCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <inheritdoc/>
        public class TravelingSalesmanCaseStudyScope : CaseStudyScope
        {
            /// <summary>
            /// Gets the Traveling Salesman Distance Unit, &quot;mi&quot;.
            /// </summary>
            protected override string DistanceUnit { get; } = "mi";

            public TravelingSalesmanCaseStudyScope(ITestOutputHelper outputHelper)
                : base(outputHelper)
            {
            }

            /// <summary>
            /// Gets the MatrixValues for use with the <see cref="Matrix"/> property.
            /// </summary>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#dist_matrix_data"/>
            protected override int?[,] MatrixValues { get; } = {
                {0, 2451, 713, 1018, 1631, 1374, 2408, 213, 2571, 875, 1420, 2145, 1972}
                , {2451, 0, 1745, 1524, 831, 1240, 959, 2596, 403, 1589, 1374, 357, 579}
                , {713, 1745, 0, 355, 920, 803, 1737, 851, 1858, 262, 940, 1453, 1260}
                , {1018, 1524, 355, 0, 700, 862, 1395, 1123, 1584, 466, 1056, 1280, 987}
                , {1631, 831, 920, 700, 0, 663, 1021, 1769, 949, 796, 879, 586, 371}
                , {1374, 1240, 803, 862, 663, 0, 1681, 1551, 1765, 547, 225, 887, 999}
                , {2408, 959, 1737, 1395, 1021, 1681, 0, 2493, 678, 1724, 1891, 1114, 701}
                , {213, 2596, 851, 1123, 1769, 1551, 2493, 0, 2699, 1038, 1605, 2300, 2099}
                , {2571, 403, 1858, 1584, 949, 1765, 678, 2699, 0, 1744, 1645, 653, 600}
                , {875, 1589, 262, 466, 796, 547, 1724, 1038, 1744, 0, 679, 1272, 1162}
                , {1420, 1374, 940, 1056, 879, 225, 1891, 1605, 1645, 679, 0, 1017, 1200}
                , {2145, 357, 1453, 1280, 586, 887, 1114, 2300, 653, 1272, 1017, 0, 504}
                , {1972, 579, 1260, 987, 371, 999, 701, 2099, 600, 1162, 1200, 504, 0}
            };

            private DistanceMatrix _matrix;

            private RoutingContext _context;

            /// <inheritdoc/>
            internal override DistanceMatrix Matrix => this._matrix ?? (this._matrix = new DistanceMatrix(this.MatrixValues));

            /// <inheritdoc/>
            internal override RoutingContext Context => this._context ?? (
                this._context = new RoutingContext(this.Matrix.Width, 1)
                );

            /// <summary>
            /// Event handler occurs On
            /// <see cref="AssignableRoutingProblemSolver{TContext, TAssign}.Assign"/> event.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected override void OnProblemSolverAssign(object sender, DefaultRoutingAssignmentEventArgs e)
            {
                var (vehicle, node, previousNode) = (e.AssertNotNull().VehicleIndex, e.NodeIndex, e.PreviousNodeIndex);

                // Vehicle should always be this.
                vehicle.AssertEqual(0);

                this.SolutionPaths[vehicle] = this.SolutionPaths.TryGetValue(vehicle, out var path)
                    ? path
                    : new List<int>();

                this.SolutionPaths[vehicle].AssertNotNull().Add(node);

                if (previousNode.HasValue)
                {
                    this.TotalDistance += this.Matrix[previousNode.Value, node] ?? default;
                }

                base.OnProblemSolverAssign(sender, e);
            }

            /// <summary>
            /// Verifies the Solution vis a vis the TSP solution.
            /// </summary>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#solution1"/>
            internal override void VerifySolution()
            {
                base.VerifySolution();

                this.SolutionPaths.AssertEqual(1, x => x.Count);

                if (this.SolutionPaths.TryGetValue(0, out var actualPath).AssertTrue())
                {
                    // Pretty much verbatim, https://developers.google.com/optimization/routing/tsp#solution1.
                    this.AssertEqual(7293, x => x.TotalDistance);

                    ICollection<int> expectedPath = Range(0, 7, 2, 3, 4, 12, 6, 8, 1, 11, 10, 5, 9, 0).ToList();

                    actualPath.AssertCollectionEqual(expectedPath);
                }

                /* See: https://developers.google.com/optimization/routing/tsp#printer, which
                 * we should have the solution in hand approaching test disposal. */

                void OnReportTotalDistance(int totalDistance) =>
                    this.OutputHelper.WriteLine($"Objective: {totalDistance} {this.DistanceUnit}");

                void OnReportEachVehiclePath((int vehicle, IEnumerable<int> path) item) =>
                    this.OutputHelper.WriteLine(
                        $"Route for vehicle {item.vehicle}: {string.Join(" , ", item.path)}"
                    );

                OnReportTotalDistance(this.TotalDistance);

                this.SolutionPaths.Keys.OrderBy(key => key)
                    .Select(key => (key, (IEnumerable<int>)this.SolutionPaths[key]))
                        .ToList().ForEach(OnReportEachVehiclePath);
            }
        }

        /// <inheritdoc/>
        protected override TravelingSalesmanCaseStudyScope VerifyScope(TravelingSalesmanCaseStudyScope scope)
        {
            scope = base.VerifyScope(scope);

            scope.TotalDistance.AssertEqual(default);

            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Width);
            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Height);

            scope.Context.AssertEqual(1, x => x.VehicleCount);

            scope.SolutionPaths.AssertNotNull().AssertCollectionEmpty();

            return scope;
        }

        /// <summary>
        /// Dimension for private usage.
        /// </summary>
        public class TestDimension : Dimension
        {
            /// <summary>
            /// Gets the Matrix for use throughout the Dimension.
            /// </summary>
            private DistanceMatrix Matrix { get; }

            /// <summary>
            /// Constructs a test dimension.
            /// </summary>
            /// <param name="context"></param>
            /// <param name="matrix"></param>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#arc-cost"/>
            public TestDimension(RoutingContext context, DistanceMatrix matrix)
                : base(context, 100)
            {
                this.Matrix = matrix;

                /* See: https://developers.google.com/optimization/routing/tsp#dist_callback1, the
                 * details of which are also abstracted behind Dimension, etc. */
                var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                /* See: https://developers.google.com/optimization/routing/tsp#arc-cost, the
                 * details of which are purposefully masked by the Dimension implementation. */

                this.SetArcCostEvaluators(evaluatorIndex);
            }

            /// <summary>
            /// Evaluates the Transit between <paramref name="fromNode"/> and
            /// <paramref name="toNode"/>.
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default(long);

            /// <summary>
            /// Evaluates the Transit between <paramref name="fromIndex"/> and
            /// <paramref name="toIndex"/>.
            /// </summary>
            /// <param name="fromIndex"></param>
            /// <param name="toIndex"></param>
            /// <returns></returns>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#dist_callback1"/>
            private long OnEvaluateTransit(long fromIndex, long toIndex) => this.OnEvaluateTransit(
                this.Context.IndexToNode(fromIndex)
                , this.Context.IndexToNode(toIndex)
            );
        }

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what we expected.
        /// We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#solution1"/>
        protected override void OnVerifySolution(TravelingSalesmanCaseStudyScope scope)
        {
            const int expectedVehicle = 0;
            scope.SolutionPaths.AssertEqual(1, x => x.Count);
            scope.SolutionPaths.AssertTrue(x => x.ContainsKey(expectedVehicle));
            var expectedPath = Range(0, 7, 2, 3, 4, 12, 6, 8, 1, 11, 10, 5, 9, 0);
            scope.SolutionPaths.AssertEqual(expectedPath, x => x[0]);
        }

        // TODO: TBD: okay, so test "works", output can be verified against expected outcomes...
        // TODO: TBD: the only question is in terms of appropriate factoring...
        // TODO: TBD: would like to be able to rinse and repeat with minimal additional effort...
        // TODO: TBD: bits that need repeating:
        // 1. matrix/values
        // 2. vehicle(s)
        // 3. depot(s)
        // 4. objective value(s)
        // 5. and/or solution accumulator(s)
        // 6. 
        // 7. 
        // 8. 
        /// <summary>
        /// Arranges for the Traveling Salesman Case Study scenarios to be performed.
        /// </summary>
        [Background]
        public void TravelingSalesmanBackground()
        {
            $"Add {nameof(TestDimension)} to this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.Context.AddDefaultDimension<TestDimension>(this.Scope.Matrix)
            );
        }

        // TODO: TBD: assuming that each of the case studies resolve in a sincle "verification" scenario...
        // TODO: TBD: then likely all we need to do is provide that single method...
        /// <summary>
        /// Demonstrates a scenario in which the Solution Agrees with the Optimization Routing
        /// Traveling Salesman Problem illustration.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp"/>
        [Scenario]
        public void Verify_ProblemSolver_Solution()
        {
            /* See: https://developers.google.com/optimization/routing/tsp#main, through which
             * we abstract these are the default solver options, i.e. PATH_CHEAPEST_ARC, etc. */

            // TODO: TBD: this is starting to look like really boilerplate stuff as well...
            $"Solve routing problem given this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.ProblemSolver.Solve(this.Scope.Context)
            );

            void OnVerifySolution() => this.OnVerifySolution(this.Scope);

            $"Verify solution".x(OnVerifySolution);
        }
    }
}
