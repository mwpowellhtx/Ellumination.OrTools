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
    using RoutingProblemSolverType = AssignableRoutingProblemSolver<RoutingContext, DefaultRoutingAssignmentEventArgs>;
    using FirstSolutionStrategyType = Google.OrTools.ConstraintSolver.FirstSolutionStrategy.Types.Value;
    using LocalSearchMetaheuristicType = Google.OrTools.ConstraintSolver.LocalSearchMetaheuristic.Types.Value;

    /// <summary>
    /// This example involves drilling holes in a circuit board with an automated drill.
    /// The problem is to find the shortest route for the drill to take on the board in
    /// order to drill all of the required holes. The example is taken from TSPLIB, a
    /// library of TSP problems.
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/tsp#circuit_board"/>
    public class CircuitBoardDrillPressCaseStudyTests : OrToolsRoutingCaseStudyTests<DistanceMatrix, RoutingContext
        , DefaultRoutingAssignmentEventArgs, DefaultRoutingProblemSolver, CircuitBoardDrillPressCaseStudyScope>
    {
        public CircuitBoardDrillPressCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        private class TestDimension : Dimension
        {
            private DistanceMatrix Matrix { get; }

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
            public TestDimension(RoutingContext context, DistanceMatrix matrix)
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
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#solution2"/>
        protected override void OnVerifySolution(CircuitBoardDrillPressCaseStudyScope scope)
        {
            base.OnVerifySolution(scope);

            scope.TotalDistance.AssertEqual(2790);

            // TODO: TBD: should refactor this into the scope...
            const int vehicle = default;

            var actualPath = scope.SolutionPaths[vehicle].AssertNotNull();

            // TODO: TBD: should refactor this into the scope...
            ICollection<int> expectedPath = Range(
                    0, 1, 279, 2, 278, 277, 247, 248, 249, 246, 244, 243, 242, 241, 240
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
                    , 135, 267, 266, 264, 263, 262, 261, 260, 258, 259, 276, 3, 4, 0
                ).ToList();

            actualPath.AssertEqual(expectedPath.Count, x => x.Count);

            void OnRenderPair((int a, int e, int i, bool z) pair)
            {
                var (a, e, i, z) = pair;

                var rendered = RenderTupleAssociates(
                    (nameof(a), Render(a))
                    , (nameof(e), Render(e))
                    , (nameof(i), Render(i))
                    , (nameof(z), Render(z))
                );

                this.OutputHelper.WriteLine($"  {rendered}");
            }

            this.OutputHelper.WriteLine($"{nameof(actualPath)}.Zip({nameof(expectedPath)}, ...) = new [] {{");

            actualPath.Zip(expectedPath, (a, e) => (a, e)).Select((z, i) => (z.a, z.e, i, z: z.a == z.e))
                .ToList().ForEach(OnRenderPair);

            this.OutputHelper.WriteLine($"}}");

            // TODO: TBD: see discussion concerning the failure: https://github.com/google/or-tools/discussions/2249
            actualPath.AssertCollectionEqual(expectedPath);
        }

        /// <summary>
        /// 
        /// </summary>
        [Scenario(Skip = "TODO: TBD: TotalDistance agrees however the path does not align with the web examples")]
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

        private void OnVerifyGuidedSolution(CircuitBoardDrillPressCaseStudyScope scope)
        {
            // actual: 2664 ...
            scope.AssertNotNull();

            const int originalArcCost = 2790;

            // "Better" should be cheaper in terms of overall arc cost.
            scope.AssertTrue(x => x.TotalDistance < originalArcCost);

            scope.AssertEqual(2672, x => x.TotalDistance);
        }

        /// <summary>
        /// 
        /// </summary>
        [Scenario(Skip = "TODO: TBD: TotalDisance is less than previously, but does not align with the web examples")]
        public void Verify_ProblemSolver_Guided_Solution()
        {
            void OnConfigureSearchParameters(object sender, RoutingSearchParametersEventArgs e)
            {
                var e_searchParams = e.SearchParameters;
                e_searchParams.FirstSolutionStrategy = FirstSolutionStrategyType.PathCheapestArc;
                e_searchParams.LocalSearchMetaheuristic = LocalSearchMetaheuristicType.GuidedLocalSearch;
                e_searchParams.TimeLimit = 30L.AsDuration();
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

            /* See: https://developers.google.com/optimization/routing/tsp#main, through which
             * we abstract these are the default solver options, i.e. PATH_CHEAPEST_ARC, etc. */

            $"Configure the search parameters callbacks".x(OnConfigureSearchParametersCallback)
                .Rollback(OnRollbackSearchParametersCallback);

            // TODO: TBD: this is starting to look like really boilerplate stuff as well...
            $"Solve routing problem given this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.ProblemSolver.Solve(this.Scope.Context)
            );

            void OnVerifyGuidedSolution() => this.OnVerifyGuidedSolution(this.Scope);

            $"Verify the default solution using nominal search params".x(OnVerifyGuidedSolution);
        }
    }
}
