using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using DistanceMatrix = Distances.DistanceMatrix;
    using FirstSolutionStrategyType = Google.OrTools.ConstraintSolver.FirstSolutionStrategy.Types.Value;
    using LocalSearchMetaheuristicType = Google.OrTools.ConstraintSolver.LocalSearchMetaheuristic.Types.Value;
    using static String;

    /// <summary>
    /// ... <see cref="!:https://developers.google.com/optimization/routing/penalties">web
    /// site example</see>.
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/penalties"/>
    public class PenaltyConstraintVehicleRoutingCaseStudyTests : VehicleRoutingCaseStudyTests<PenaltyConstraintVehicleRoutingCaseStudyScope>
    {
        public PenaltyConstraintVehicleRoutingCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Establishes a NodeDemandDimension for use during the routing problem solver.
        /// </summary>
        private class DisjunctionDimension : Dimension
        {
            /// <summary>
            /// Gets the Scope for Private use.
            /// </summary>
            private PenaltyConstraintVehicleRoutingCaseStudyScope Scope { get; }

            /// <summary>
            /// Constructs a Node Demand dimension.
            /// </summary>
            /// <param name="context"></param>
            /// <param name="scope"></param>
            /// <see cref="!:https://developers.google.com/optimization/routing/penalties#add-the-capacity-constraints-and-penalties"/>
            public DisjunctionDimension(RoutingContext context, PenaltyConstraintVehicleRoutingCaseStudyScope scope)
                : base(context, scope.DimensionCoefficient)
            {
                var scope_Penalty = scope.Penalty ?? default;

                if (this.Coefficient > 0 && scope_Penalty > 0)
                {
                    this.Scope = scope;

                    // Which we think is more accurate than assuming the Zero Node.
                    var context_DepotCoords = this.Context.DepotCoordinates.ToArray();

                    var nodes = Enumerable.Range(0, this.Context.NodeCount).Where(_ => !context_DepotCoords.Contains(_)).ToList();

                    void OnAddDisjunction(int node) => this.AddDisjunction(scope_Penalty, node);

                    nodes.ForEach(OnAddDisjunction);
                }
            }
        }

        /// <inheritdoc/>
        protected override PenaltyConstraintVehicleRoutingCaseStudyScope OnVerifyInitialScope(PenaltyConstraintVehicleRoutingCaseStudyScope scope)
        {
            scope = base.OnVerifyInitialScope(scope);

            scope.ActualTotalDistance.AssertEqual(default);

            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Width);
            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Height);

            scope.Context.AssertEqual(scope.VehicleCount, x => x.VehicleCount);

            return scope;
        }

        /// <summary>
        /// Performs the Background for the CVRP problem.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/penalties"/>
        [Background]
        public void PenaltyConstraintVehicleRoutingBackground(PenaltyConstraintVehicleRoutingCaseStudyScope scope)
        {
            $"Verify {nameof(scope)}".x(() => scope = this.Scope.AssertNotNull());

            $"Add {nameof(DisjunctionDimension)} dimension to {nameof(scope)}.{nameof(scope.Context)}".x(
                () => scope.Context.AddDefaultDimension<DisjunctionDimension>(scope)
            );
        }

        /// <summary>
        /// Demonstrates a scenario in which the Solution Agrees with the Optimization Routing
        /// Traveling Salesman Problem illustration.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrp"/>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrp#entire_program"/>
        [Scenario(Skip = IncongruentResults)]
        public void Verify_ProblemSolver_Constraint_Solution(PenaltyConstraintVehicleRoutingCaseStudyScope scope)
        {
            $"Verify {nameof(scope)}".x(() => scope = this.Scope.AssertNotNull());

            void OnScopeProblemSolverConfigureSearch(object sender, RoutingSearchParametersEventArgs e)
            {
                var e_params = e.Parameters.AssertNotNull();
                // Setting first solution heuristic.
                e_params.FirstSolutionStrategy = FirstSolutionStrategyType.PathCheapestArc;
                e_params.LocalSearchMetaheuristic = LocalSearchMetaheuristicType.GuidedLocalSearch;
                const long second = 1;
                e_params.TimeLimit = second;
            }

            void OnArrangeProblemSolverConfigSearch()
            {
                scope.ProblemSolver.ConfigureSearchParameters += OnScopeProblemSolverConfigureSearch;
            }

            void OnRollbackProblemSolverConfigSearch()
            {
                scope.ProblemSolver.ConfigureSearchParameters -= OnScopeProblemSolverConfigureSearch;
            }

            $"Configure {nameof(scope)}.{nameof(scope.ProblemSolver)}.{nameof(scope.ProblemSolver.ConfigureSearchParameters)}"
                .x(OnArrangeProblemSolverConfigSearch)
                .Rollback(OnRollbackProblemSolverConfigSearch)
                ;

            $"Solve routing problem given {nameof(scope)}.{nameof(scope.Context)}".x(
                () => scope.ProblemSolver.Solve(scope.Context)
            );
        }

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what
        /// we expected. We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/penalties#running-the-program"/>
        protected override void OnVerifySolutionScope(PenaltyConstraintVehicleRoutingCaseStudyScope scope)
        {
            // TODO: TBD: factor this one better...
            void Report()
            {
                var dropped = Range(0, scope.Context.NodeCount).Except(scope.SolutionPaths.SelectMany(_ => _).Distinct());

                this.OutputHelper.WriteLine($"Dropped nodes: {Render(dropped.ToArray())}");

                var scope_Context_DepotCoords = scope.Context.DepotCoordinates.ToArray();

                var loads = Repeat<long>(default, scope.Context.VehicleCount).ToArray();

                void OnReportEach((int vehicle, int[] path) tuple)
                {
                    this.OutputHelper.WriteLine($"{nameof(tuple.path)} for {nameof(tuple.vehicle)} {tuple.vehicle}:");

                    // Enumerate the path in terms of "X Load(Y) -> ..."

                    string RenderLoad(int node)
                    {
                        long load = scope_Context_DepotCoords.Contains(node)
                            ? default
                            : scope.Demands.ElementAt(node);
                        return $"{node} {nameof(load)}({loads[tuple.vehicle] += load})";
                    }

                    this.OutputHelper.WriteLine(Join(" -> ", tuple.path.Select(RenderLoad)));

                    int distance = tuple.path.Count() <= 2 ? default : tuple.path.Take(tuple.path.Length - 1)
                        .Zip(tuple.path.Skip(1), (x, y) => scope.Matrix[x, y] ?? default).Sum();

                    this.OutputHelper.WriteLine($"Distance of the {nameof(tuple.path)}: {distance}");
                    this.OutputHelper.WriteLine($"Load of the {nameof(tuple.path)}: {loads[tuple.vehicle]}");
                }

                scope.SolutionPaths.Select((_, i) => (i, _.ToArray())).ToList().ForEach(OnReportEach);
            }

            Report();

            base.OnVerifySolutionScope(scope);

            scope.SolutionPaths.AssertEqual(scope.VehicleCount, x => x.Count);

            void OnVerifyExpectedPath((int vehicle, int[] expectedPath) tuple) =>
                scope.SolutionPaths[tuple.vehicle].AssertCollectionEqual(tuple.expectedPath);

            scope.ExpectedPaths.AssertEqual(scope.VehicleCount, x => x.Count)
                .Select((x, i) => (i, x)).ToList().ForEach(OnVerifyExpectedPath);
        }
    }
}
