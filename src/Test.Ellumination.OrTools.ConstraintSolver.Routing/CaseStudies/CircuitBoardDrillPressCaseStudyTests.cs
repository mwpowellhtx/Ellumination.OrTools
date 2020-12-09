using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using RoutingProblemSolverType = AssignableRoutingProblemSolver<RoutingContext, DefaultRoutingAssignmentEventArgs>;

    /// <summary>
    /// This example involves drilling holes in a circuit board with an automated drill.
    /// The problem is to find the shortest route for the drill to take on the board in
    /// order to drill all of the required holes. The example is taken from TSPLIB, a
    /// library of TSP problems.
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/tsp#circuit_board"/>
    public class CircuitBoardDrillPressCaseStudyTests : OrToolsRoutingCaseStudyTests<Distances.DistanceMatrix
        , RoutingContext, CircuitBoardDrillPressCaseStudyTests.CircuitBoardDrillPressCaseStudyScope>
    {
        public CircuitBoardDrillPressCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <inheritdoc/>
        public class CircuitBoardDrillPressCaseStudyScope : CaseStudyScope
        {
            /// <summary>
            /// Gets the Circuit Board Drill Press DistanceUnit, &quot;m&quot;.
            /// </summary>
            protected override string DistanceUnit { get; } = "m";

            public CircuitBoardDrillPressCaseStudyScope(ITestOutputHelper outputHelper)
                : base(outputHelper)
            {
            }

            /// <summary>
            /// Gets or Sets the TravelDistance.
            /// </summary>
            internal int TravelDistance { get; set; }

            /// <summary>
            /// The data for the problem consist of 280 points in the plane, shown in
            /// the scatter chart above. The program creates the data in an array of
            /// ordered pairs corresponding to the points in the plane, as shown below.
            /// </summary>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#data3"/>
            internal int[,] CircuitBoardCoordinates { get; } = new int[,]
            {
                {288, 149}, {288, 129}, {270, 133}, {256, 141}, {256, 157}, {246, 157}
                , {236, 169}, {228, 169}, {228, 161}, {220, 169}, {212, 169}, {204, 169}
                , {196, 169}, {188, 169}, {196, 161}, {188, 145}, {172, 145}, {164, 145}
                , {156, 145}, {148, 145}, {140, 145}, {148, 169}, {164, 169}, {172, 169}
                , {156, 169}, {140, 169}, {132, 169}, {124, 169}, {116, 161}, {104, 153}
                , {104, 161}, {104, 169}, {90, 165}, {80, 157}, {64, 157}, {64, 165}
                , {56, 169}, {56, 161}, {56, 153}, {56, 145}, {56, 137}, {56, 129}
                , {56, 121}, {40, 121}, {40, 129}, {40, 137}, {40, 145}, {40, 153}
                , {40, 161}, {40, 169}, {32, 169}, {32, 161}, {32, 153}, {32, 145}
                , {32, 137}, {32, 129}, {32, 121}, {32, 113}, {40, 113}, {56, 113}
                , {56, 105}, {48, 99}, {40, 99}, {32, 97}, {32, 89}, {24, 89}, {16, 97}
                , {16, 109}, {8, 109}, {8, 97}, {8, 89}, {8, 81}, {8, 73}, {8, 65}
                , {8, 57}, {16, 57}, {8, 49}, {8, 41}, {24, 45}, {32, 41}, {32, 49}
                , {32, 57}, {32, 65}, {32, 73}, {32, 81}, {40, 83}, {40, 73}, {40, 63}
                , {40, 51}, {44, 43}, {44, 35}, {44, 27}, {32, 25}, {24, 25}, {16, 25}
                , {16, 17}, {24, 17}, {32, 17}, {44, 11}, {56, 9}, {56, 17}, {56, 25}
                , {56, 33}, {56, 41}, {64, 41}, {72, 41}, {72, 49}, {56, 49}, {48, 51}
                , {56, 57}, {56, 65}, {48, 63}, {48, 73}, {56, 73}, {56, 81}, {48, 83}
                , {56, 89}, {56, 97}, {104, 97}, {104, 105}, {104, 113}, {104, 121}
                , {104, 129}, {104, 137}, {104, 145}, {116, 145}, {124, 145}, {132, 145}
                , {132, 137}, {140, 137}, {148, 137}, {156, 137}, {164, 137}, {172, 125}
                , {172, 117}, {172, 109}, {172, 101}, {172, 93}, {172, 85}, {180, 85}
                , {180, 77}, {180, 69}, {180, 61}, {180, 53}, {172, 53}, {172, 61}
                , {172, 69}, {172, 77}, {164, 81}, {148, 85}, {124, 85}, {124, 93}
                , {124, 109}, {124, 125}, {124, 117}, {124, 101}, {104, 89}, {104, 81}
                , {104, 73}, {104, 65}, {104, 49}, {104, 41}, {104, 33}, {104, 25}
                , {104, 17}, {92, 9}, {80, 9}, {72, 9}, {64, 21}, {72, 25}, {80, 25}
                , {80, 25}, {80, 41}, {88, 49}, {104, 57}, {124, 69}, {124, 77}, {132, 81}
                , {140, 65}, {132, 61}, {124, 61}, {124, 53}, {124, 45}, {124, 37}
                , {124, 29}, {132, 21}, {124, 21}, {120, 9}, {128, 9}, {136, 9}, {148, 9}
                , {162, 9}, {156, 25}, {172, 21}, {180, 21}, {180, 29}, {172, 29}
                , {172, 37}, {172, 45}, {180, 45}, {180, 37}, {188, 41}, {196, 49}
                , {204, 57}, {212, 65}, {220, 73}, {228, 69}, {228, 77}, {236, 77}
                , {236, 69}, {236, 61}, {228, 61}, {228, 53}, {236, 53}, {236, 45}
                , {228, 45}, {228, 37}, {236, 37}, {236, 29}, {228, 29}, {228, 21}
                , {236, 21}, {252, 21}, {260, 29}, {260, 37}, {260, 45}, {260, 53}
                , {260, 61}, {260, 69}, {260, 77}, {276, 77}, {276, 69}, {276, 61}
                , {276, 53}, {284, 53}, {284, 61}, {284, 69}, {284, 77}, {284, 85}
                , {284, 93}, {284, 101}, {288, 109}, {280, 109}, {276, 101}, {276, 93}
                , {276, 85}, {268, 97}, {260, 109}, {252, 101}, {260, 93}, {260, 85}
                , {236, 85}, {228, 85}, {228, 93}, {236, 93}, {236, 101}, {228, 101}
                , {228, 109}, {228, 117}, {228, 125}, {220, 125}, {212, 117}, {204, 109}
                , {196, 101}, {188, 93}, {180, 93}, {180, 101}, {180, 109}, {180, 117}
                , {180, 125}, {196, 145}, {204, 145}, {212, 145}, {220, 145}, {228, 145}
                , {236, 145}, {246, 141}, {252, 125}, {260, 129}, {280, 133}
            };

            /// <summary>
            /// Returns the Calculated Euclidian <see cref="Distances.Matrix.Values"/>
            /// of the <see cref="Distances.DistanceMatrix"/>.
            /// </summary>
            /// <param name="coords"></param>
            /// <returns></returns>
            /// <see cref="!:https://en.wikipedia.org/wiki/Euclidean_distance"/>
            private static int?[,] CalculateEuclidianDistanceMatrix(int[,] coords)
            {
                // Calculate the distance matrix using Euclidean distance.
                var length = coords.GetLength(0);
                var matrixValues = new int?[length, length];

                int Sqrt(double d) => (int)Math.Sqrt(d);
                double Pow(double x, double y) => Math.Pow(x, y);

                void OnCalculateDistance((int fromNode, int toNode) coord)
                {
                    matrixValues[coord.fromNode, coord.toNode] = Sqrt(
                        Pow(coords[coord.toNode, 0] - coords[coord.fromNode, 0], 2)
                        + Pow(coords[coord.toNode, 1] - coords[coord.fromNode, 1], 2)
                    );
                }

                var dim = Enumerable.Range(0, length).ToArray();

                // Zero the diagonal, which is handled by the matrix itself.
                dim.Zip(dim, (x, y) => (x, y)).Where(coord => coord.x != coord.y)
                    .ToList().ForEach(OnCalculateDistance);

                return matrixValues;
            }

            private int?[,] _matrixValues;

            private Distances.DistanceMatrix _matrix;

            private RoutingContext _context;

            /// <inheritdoc/>
            protected override int?[,] MatrixValues => this._matrixValues ?? (
                this._matrixValues = CalculateEuclidianDistanceMatrix(this.CircuitBoardCoordinates)
            );

            /// <inheritdoc/>
            internal override Distances.DistanceMatrix Matrix => this._matrix ?? (
                this._matrix = new Distances.DistanceMatrix(this.MatrixValues)
            );

            /// <summary>
            /// A single vehicle, the drill press.
            /// </summary>
            private const int OneVehicle = 1;

            /// <summary>
            /// Zed, <c>0</c>, is the home Depot.
            /// </summary>
            private const int Depot = 0;

            /// <inheritdoc/>
            internal override RoutingContext Context => this._context ?? (
                this._context = new RoutingContext(this.Matrix.Length, OneVehicle, Depot)
            );

            /// <inheritdoc/>
            protected override void OnDisposing()
            {
                /* https://developers.google.com/optimization/routing/tsp#print_solution2 */

                void OnReportTotalDistance(int totalDistance) =>
                    this.OutputHelper.WriteLine($"Objective: {totalDistance} {this.DistanceUnit}");

                void OnReportEachVehiclePath((int vehicle, IEnumerable<int> path) item) =>
                    this.OutputHelper.WriteLine(
                        $"Route for vehicle {item.vehicle}: {string.Join(" -> ", item.path)}"
                    );

                OnReportTotalDistance(this.TotalDistance);

                this.SolutionPaths.Keys.OrderBy(key => key)
                    .Select(key => (key, (IEnumerable<int>)this.SolutionPaths[key]))
                        .ToList().ForEach(OnReportEachVehiclePath);

                base.OnDisposing();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class TestDimension : Dimension
        {
            private Distances.DistanceMatrix Matrix { get; }

            /// <summary>
            /// <c>100</c>
            /// </summary>
            private const int DefaultCoefficient = 100;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="context"></param>
            /// <param name="matrix"></param>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#distance_callback"/>
            public TestDimension(RoutingContext context, Distances.DistanceMatrix matrix)
                : base(context, DefaultCoefficient)
            {
                this.Matrix = matrix;

                var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                this.SetArcCostEvaluators(evaluatorIndex);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromIndex"></param>
            /// <param name="toIndex"></param>
            /// <returns></returns>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#distance_callback"/>
            private long OnEvaluateTransit(long fromIndex, long toIndex) => this.OnEvaluateTransit(
                this.Context.IndexToNode(fromIndex), this.Context.IndexToNode(toIndex)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        [Background]
        public void CircuitBoardDrillPressBackground()
        {
            // TODO: TBD: which even this is a bit boilerplate, for academic case study...
            $"Add {nameof(TestDimension)} to this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.Context.AddDefaultDimension<TestDimension>(this.Scope.Matrix)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        [Scenario]
        public void Verify_ProblemSolver_Default_Solution()
        {
            /* See: https://developers.google.com/optimization/routing/tsp#main, through which
             * we abstract these are the default solver options, i.e. PATH_CHEAPEST_ARC, etc. */

            // TODO: TBD: this is starting to look like really boilerplate stuff as well...
            $"Solve routing problem given this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.ProblemSolver.Solve(this.Scope.Context)
            );

            void OnVerifyDefaultSolution() => this.OnVerifySolution(this.Scope);

            $"Verify the default solution using nominal search params".x(OnVerifyDefaultSolution);
        }
    }
}
