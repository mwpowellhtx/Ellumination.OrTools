using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Google.Protobuf.WellKnownTypes;
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    // TODO: TBD: making an alias here because we think we may refactor to collections suite...
    using DistanceMatrix = Distances.DistanceMatrix;
    using FirstSolutionStrategyType = Google.OrTools.ConstraintSolver.FirstSolutionStrategy.Types.Value;
    using LocalSearchMetaheuristicType = Google.OrTools.ConstraintSolver.LocalSearchMetaheuristic.Types.Value;

    /// <summary>
    /// This example involves drilling holes in a circuit board with an automated drill.
    /// The problem is to find the shortest route for the drill to take on the board in
    /// order to drill all of the required holes. The example is taken from TSPLIB, a
    /// library of TSP problems.
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/tsp#circuit_board"/>
    public class CircuitBoardCaseStudyTests : OrToolsRoutingCaseStudyTests<DistanceMatrix, RoutingContext
        , DefaultRoutingAssignmentEventArgs, DefaultRoutingProblemSolver, CircuitBoardCaseStudyScope>
    {
        public CircuitBoardCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected class TestDimension : Dimension
        {
            /// <summary>
            /// Gets the SCope for Private use.
            /// </summary>
            private CircuitBoardCaseStudyScope Scope { get; }

            /// <summary>
            /// Gets the Matrix for Private use.
            /// </summary>
            private DistanceMatrix Matrix => this.Scope.Matrix;

            /// <summary>
            /// Constructs the dimension.
            /// </summary>
            /// <param name="context"></param>
            /// <param name="matrix"></param>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#distance_callback"/>
            public TestDimension(RoutingContext context, CircuitBoardCaseStudyScope scope)
                : base(context, scope.DimensionCoefficient)
            {
                if (this.Coefficient > 0)
                {
                    this.Scope = scope;

                    var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                    // TODO: TBD: I'm not sure we actually ever add the dimension here ...
                    // TODO: TBD: should review the example again to make sure that we actually should/are/do... (take your pick)
                    this.SetArcCostEvaluators(evaluatorIndex);
                }
            }

            /// <summary>
            /// Returns the result after Evaluating <see cref="Matrix"/> given
            /// <paramref name="fromNode"/> and <paramref name="toNode"/>.
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default;

            /// <summary>
            /// Returns the result after Evaluating <see cref="Matrix"/> given
            /// <paramref name="fromIndex"/> and <paramref name="toIndex"/>.
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
        /// Arranges the Circuit Board Case Study <paramref name="scope"/>.
        /// </summary>
        /// <param name="scope"></param>
        protected virtual void OnArrangeCircuitBoardCaseStudyScope(CircuitBoardCaseStudyScope scope)
        {
            scope.ExpectedTotalDistance = 2790;

            scope.ExpectedPaths = Range<int[]>(
                    Range(0, 1, 279, 2, 278, 277, 247, 248, 249, 246, 244, 243, 242, 241, 240
                        , 239, 238, 237, 236, 235, 234, 233, 232, 231, 230, 245, 250, 229, 228
                        , 227, 226, 225, 224, 223, 222, 221, 220, 219, 218, 217, 216, 215, 214
                        , 213, 212, 211, 210, 209, 208, 251, 254, 255, 257, 256, 253, 252, 207
                        , 206, 205, 204, 203, 202, 142, 141, 146, 147, 140, 139, 265, 136, 137
                        , 138, 148, 149, 177, 176, 175, 178, 179, 180, 181, 182, 183, 184, 186
                        , 185, 192, 196, 197, 198, 144, 145, 143, 199, 201, 200, 195, 194, 193
                        , 191, 190, 189, 188, 187, 163, 164, 165, 166, 167, 168, 169, 171, 170
                        , 172, 105, 106, 104, 103, 107, 109, 110, 113, 114, 116, 117, 61, 62
                        , 63, 65, 64, 84, 85, 115, 112, 86, 83, 82, 87, 111, 108, 89, 90, 91
                        , 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 92, 79, 88, 81, 80, 78
                        , 77, 76, 74, 75, 73, 72, 71, 70, 69, 66, 68, 67, 57, 56, 55, 54
                        , 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 58, 60, 59, 42, 41
                        , 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 124, 123, 122, 121
                        , 120, 119, 118, 156, 157, 158, 173, 162, 161, 160, 174, 159, 150, 151
                        , 155, 152, 154, 153, 128, 129, 130, 131, 18, 19, 20, 127, 126, 125, 28
                        , 27, 26, 25, 21, 24, 22, 23, 13, 12, 14, 11, 10, 9, 7, 8, 6, 5
                        , 275, 274, 273, 272, 271, 270, 15, 16, 17, 132, 133, 269, 268, 134
                        , 135, 267, 266, 264, 263, 262, 261, 260, 258, 259, 276, 3, 4, 0).ToArray()
                ).ToList();
        }

        /// <summary>
        /// Do some CircuitBoardBackground.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="matrix"></param>
        /// <param name="expectedPaths"></param>
        [Background]
        public void CircuitBoardBackground(CircuitBoardCaseStudyScope scope, DistanceMatrix matrix, IEnumerable<int[]> expectedPaths)
        {
            $"Verify {nameof(scope)} not null".x(() =>
                scope = this.Scope.AssertNotNull()
            );

            $"Verify {nameof(scope)}.{nameof(scope.Matrix)} not null".x(() =>
                matrix = scope.Matrix.AssertNotNull()
            );

            $"Arrange {nameof(scope)}.{nameof(scope.ExpectedTotalDistance)}".x(() =>
                expectedPaths = scope.ExpectedPaths.AssertEqual(scope.VehicleCount, x => x.Count)
            );

            // TODO: TBD: which even this is a bit boilerplate, for academic case study...
            $"Add {nameof(TestDimension)} to {nameof(scope)}.{nameof(scope.Context)}".x(
                () => scope.Context.AddDefaultDimension<TestDimension>(scope)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#main"/>
        /// <param name="scope"></param>
        [Scenario/*(Skip = "TODO: TBD: TotalDistance agrees however the path does not align with the web examples")*/]
        public void Verify_ProblemSolver_Solution(CircuitBoardCaseStudyScope scope)
        {
            $"Verify {nameof(scope)}".x(() => scope = this.Scope.AssertNotNull());

            void OnConfigureSearchParameters(object sender, RoutingSearchParametersEventArgs e)
            {
                var e_searchParams = e.SearchParameters;
                e_searchParams.FirstSolutionStrategy = FirstSolutionStrategyType.PathCheapestArc;
                //e_searchParams.LocalSearchMetaheuristic = LocalSearchMetaheuristicType.GuidedLocalSearch;
                //e_searchParams.TimeLimit = 30L.AsDuration();
                e_searchParams.LogSearch = true;
            }

            void OnConfigureSearchParametersCallback()
            {
                this.Scope.ProblemSolver.ConfigureSearchParameters += OnConfigureSearchParameters;
            }

            void OnRollbackSearchParametersCallback()
            {
                this.Scope.ProblemSolver.ConfigureSearchParameters -= OnConfigureSearchParameters;
            }

            $"Configure search parameters callbacks".x(OnConfigureSearchParametersCallback)
                .Rollback(OnRollbackSearchParametersCallback);

            // TODO: TBD: this is starting to look like really boilerplate stuff as well...
            $"Solve routing problem given {nameof(scope)}.{nameof(scope.Context)}".x(
                () => scope.ProblemSolver.Solve(scope.Context)
            );
        }

        /// <summary>
        /// Verifies the Circuit Board Case Study <paramref name="scope"/>.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#solution2"/>
        /// <see cref="!:https://github.com/google/or-tools/discussions/2249">See discussion concerning various aspects</see>
        protected virtual void OnVerifyCircuitBoardSolutionScope(CircuitBoardCaseStudyScope scope) =>
            scope.AssertNotNull().VerifyAndReportSolution();

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what
        /// we expected. We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        protected override void OnVerifySolutionScope(CircuitBoardCaseStudyScope scope)
        {
            base.OnVerifySolutionScope(scope);

            this.OnVerifyCircuitBoardSolutionScope(scope);
        }
    }
}
