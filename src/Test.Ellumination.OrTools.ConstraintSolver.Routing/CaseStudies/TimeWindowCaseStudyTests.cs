using System;
using System.Collections.Generic;
using System.Linq;

#if false

    /* TODO: TBD: this one will be key, we will most likely need to
     * include additional details in the solution/assignment event
     * handler in order to facilitate the intermittent bits, extrapolating
     * dimension, dim vars, mins, maxes, etc... I think we have the basics represented.
     * what we may not have there yet are bits like the assignment, what to do with
     * variables, mins, maxes, etc... */

    /// <summary>
    ///   Print the solution.
    /// </summary>
    static void PrintSolution(in DataModel data, in RoutingModel model, in RoutingIndexManager manager, in Assignment solution)
    {
        // TODO: TBD: we may need to expose dimensional lookups after all...
        // TODO: TBD: which in this context, and that of the event pub/sub, makes a certain amount of sense...
        RoutingDimension timeDimension = model.GetMutableDimension("Time");
        // Inspect solution.
        var totalTime = default(long);
        for (var vehicle = 0; vehicle < data.VehicleNumber; vehicle++)
        {
            long index;
            Console.WriteLine("Route for Vehicle {0}:", vehicle);
            for (index = model.Start(vehicle); !model.IsEnd(index); index = solution.Value(model.NextVar(index)))
            {
                // TODO: TBD: would it be worthwhile including the cumulvars for each dimension at the given index?
                // TODO: TBD: in any event, starting to sound like we need to define a first class descriptor...
                // TODO: TBD: rather than the ad-hoc value tuple... since we need "both" indexes, etc...
                var timeVar = timeDimension.CumulVar(index);
                Console.Write("{0} Time({1},{2}) -> ", manager.IndexToNode(index), solution.Min(timeVar), solution.Max(timeVar));
            }
            var endTimeVar = timeDimension.CumulVar(index);
            Console.WriteLine("{0} Time({1},{2})", manager.IndexToNode(index), solution.Min(endTimeVar), solution.Max(endTimeVar));
            Console.WriteLine("Time of the route: {0}min", solution.Min(endTimeVar));
            totalTime += solution.Min(endTimeVar);
        }
        Console.WriteLine("Total time of all routes: {0}min", totalTime);
    }

#endif // false

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Google.Protobuf.WellKnownTypes;
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    // TODO: TBD: making an alias here because we think we may refactor to collections suite...
    using Assignment = Google.OrTools.ConstraintSolver.Assignment;
    using DistanceMatrix = Distances.DistanceMatrix;
    using RoutingDimension = Google.OrTools.ConstraintSolver.RoutingDimension;
    using RoutingModel = Google.OrTools.ConstraintSolver.RoutingModel;
    using FirstSolutionStrategyType = Google.OrTools.ConstraintSolver.FirstSolutionStrategy.Types.Value;
    using LocalSearchMetaheuristicType = Google.OrTools.ConstraintSolver.LocalSearchMetaheuristic.Types.Value;
    using static String;

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/vrptw"/>
    public class TimeWindowCaseStudyTests : OrToolsRoutingCaseStudyTests<DistanceMatrix, RoutingContext
        , DefaultRoutingAssignmentEventArgs, DefaultRoutingProblemSolver, TimeWindowCaseStudyScope>
    {
        public TimeWindowCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected class TimeWindowDimension : Dimension
        {
            /// <summary>
            /// Gets the Scope for Private use.
            /// </summary>
            private TimeWindowCaseStudyScope Scope { get; }

            /// <summary>
            /// Gets the Matrix for Private use.
            /// </summary>
            private DistanceMatrix Matrix => this.Scope.Matrix;

            /// <summary>
            /// Constructs the dimension.
            /// </summary>
            /// <param name="context"></param>
            /// <param name="scope"></param>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#distance_callback"/>
            public TimeWindowDimension(RoutingContext context, TimeWindowCaseStudyScope scope)
                : base(context, scope.DimensionCoefficient)
            {
                if (this.Coefficient > 0)
                {
                    this.Scope = scope;

                    var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                    // TODO: TBD: I'm not sure we actually ever add the dimension here ...
                    // TODO: TBD: should review the example again to make sure that we actually should/are/do... (take your pick)
                    this.SetArcCostEvaluators(evaluatorIndex);

                    // TODO: TBD: with parameters that could be captured in a context, etc...
                    this.AddDimension(evaluatorIndex, scope.VehicleCap, scope.SlackMaximumOrDefault, scope.ZeroAccumulatorOrDefault);

                    {
                        // TODO: TBD: could/should probably refactor this to a separate method...
                        // https://developers.google.com/optimization/routing/vrptw#time_window_constraints

                        var this_Context_DepotCoords = this.Context.DepotCoordinates.ToArray();

                        bool DepotCoordinatesDoNotContain(int _) => !this_Context_DepotCoords.Contains(_);

                        // TODO: TBD: we may want to follow a similar tract here re: RC versus RoutingIndexManager...
                        // TODO: TBD: however we just prefer to abstract those sorts of bits through the RC...
                        // TODO: TBD: if there is a case for it, we will, but for now, the interface is sufficient...
                        void OnSetCumulVarTimeWindow(int node, in (long start, long end) tw) => this.SetCumulVarRange(
                            node, tw, (RoutingContext _, int i) => _.NodeToIndex(i)
                        );

                        void OnSetCumulVarRange(int node) => OnSetCumulVarTimeWindow(
                            node
                            , scope.TimeWindows.ElementAt(node)
                        );

                        // Add Time Window constraints for each Node except the Depot(s).
                        var nodes = Enumerable.Range(default, this.Context.NodeCount)
                            .Where(DepotCoordinatesDoNotContain).ToList();

                        nodes.ForEach(OnSetCumulVarRange);
                    }

                    {
                        // Add time window constraints for each vehicle start node.
                        void OnSetVehicleTimeWindow(int vehicle) => this.SetCumulVarRange(
                            vehicle, scope.TimeWindows.First(), (RoutingModel model, int i) => model.Start(i)
                        );

                        var vehicles = Enumerable.Range(default, scope.VehicleCount).ToList();

                        vehicles.ForEach(OnSetVehicleTimeWindow);
                    }

                    {
                        // Arrange for each Vehicle Start and End Finalizer concerns.
                        void OnArrangeVehicleFinalizers(int vehicle)
                        {
                            // Illustrating how we can leverage either the root methods or extension methods.
                            this.AddVariableMinimizedByFinalizer(vehicle, (RoutingContext _, int i) => _.Model.Start(i));
                            this.AddVariableMinimizedByFinalizer(vehicle, (RoutingModel model, int i) => model.End(i));
                        }

                        var vehicles = Enumerable.Range(default, scope.VehicleCount).ToList();

                        vehicles.ForEach(OnArrangeVehicleFinalizers);
                    }
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

        ///// <summary>
        ///// Arranges the Circuit Board Case Study <paramref name="scope"/>.
        ///// </summary>
        ///// <param name="scope"></param>
        //protected virtual void OnArrangeTimeWindowCaseStudyScope(TimeWindowCaseStudyScope scope)
        //{
        //    scope.ExpectedTotalDistance = 2790;
        //    scope.ExpectedPaths = Range<int[]>(
        //            Range(0, 1, 279, 2, 278, 277, 247, 248, 249, 246, 244, 243, 242, 241, 240
        //                , 239, 238, 237, 236, 235, 234, 233, 232, 231, 230, 245, 250, 229, 228
        //                , 227, 226, 225, 224, 223, 222, 221, 220, 219, 218, 217, 216, 215, 214
        //                , 213, 212, 211, 210, 209, 208, 251, 254, 255, 257, 256, 253, 252, 207
        //                , 206, 205, 204, 203, 202, 142, 141, 146, 147, 140, 139, 265, 136, 137
        //                , 138, 148, 149, 177, 176, 175, 178, 179, 180, 181, 182, 183, 184, 186
        //                , 185, 192, 196, 197, 198, 144, 145, 143, 199, 201, 200, 195, 194, 193
        //                , 191, 190, 189, 188, 187, 163, 164, 165, 166, 167, 168, 169, 171, 170
        //                , 172, 105, 106, 104, 103, 107, 109, 110, 113, 114, 116, 117, 61, 62
        //                , 63, 65, 64, 84, 85, 115, 112, 86, 83, 82, 87, 111, 108, 89, 90, 91
        //                , 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 92, 79, 88, 81, 80, 78
        //                , 77, 76, 74, 75, 73, 72, 71, 70, 69, 66, 68, 67, 57, 56, 55, 54
        //                , 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 58, 60, 59, 42, 41
        //                , 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 124, 123, 122, 121
        //                , 120, 119, 118, 156, 157, 158, 173, 162, 161, 160, 174, 159, 150, 151
        //                , 155, 152, 154, 153, 128, 129, 130, 131, 18, 19, 20, 127, 126, 125, 28
        //                , 27, 26, 25, 21, 24, 22, 23, 13, 12, 14, 11, 10, 9, 7, 8, 6, 5
        //                , 275, 274, 273, 272, 271, 270, 15, 16, 17, 132, 133, 269, 268, 134
        //                , 135, 267, 266, 264, 263, 262, 261, 260, 258, 259, 276, 3, 4, 0).ToArray()
        //        ).ToList();
        //}

        /// <summary>
        /// Do some TimeWindowBackground.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <param name="matrix"></param>
        /// <param name="expectedPaths"></param>
        [Background]
        public void TimeWindowBackground(TimeWindowCaseStudyScope scope, RoutingContext context, DistanceMatrix matrix, IEnumerable<int[]> expectedPaths)
        {
            $"Verify {nameof(scope)} not null".x(() =>
                scope = this.Scope.AssertNotNull()
            );

            $"Verify {nameof(scope)}.{nameof(scope.Matrix)} not null".x(() =>
                matrix = scope.Matrix.AssertNotNull()
            );

            $"Verify {nameof(scope)}.{nameof(scope.Context)} not null".x(() =>
                context = scope.Context.AssertNotNull()
            ); ;

            $"Arrange the total distance".x(() => scope.ExpectedTotalDistance = 71);

            $"Arrange {nameof(scope)}.{nameof(scope.ExpectedTotalDistance)}".x(() =>
                expectedPaths = scope.ExpectedPaths.AssertEqual(scope.VehicleCount, x => x.Count)
            );

            // TODO: TBD: which even this is a bit boilerplate, for academic case study...
            $"Add {nameof(TimeWindowDimension)} to {nameof(scope)}.{nameof(scope.Context)}".x(
                () => scope.Context.AddDefaultDimension<TimeWindowDimension>(scope)
            );

            IEnumerable<(int vehicle, int node, long index, long min, long max)> OnSummarizeAssignments(
                RoutingDimension dim, Assignment solution, IEnumerable<RouteAssignmentItem> assignments) =>
                assignments.Select(_ => (var: dim.CumulVar(_.Index), node: _.Node, index: _.Index, vehicle: _.Vehicle))
                    .Select(_ => (_.vehicle, _.node, _.index, min: solution.Min(_.var), max: solution.Max(_.var)));

            void OnScopeProblemSolverAfterAssignmentVehicle(object sender, DefaultRoutingAssignmentEventArgs e)
            {
                var context_Dim = context.Dimensions.OfType<TimeWindowDimension>().AssertCollectionNotEmpty()
                    .SingleOrDefault().AssertNotNull();

                var e_Summary = OnSummarizeAssignments(context_Dim.MutableDimension, e.Solution, e.Assignments).ToArray();

                var vehicle = e_Summary.Select(_ => _.vehicle).Distinct()
                    .ToArray().AssertEqual(1, _ => _.Length).Single();

                this.OutputHelper.WriteLine($"Route for vehicle {vehicle}");

                this.OutputHelper.WriteLine(Join(" -> ", e_Summary.Select(_ => $"{_.node} Time({_.min}, {_.max})")));

                this.OutputHelper.WriteLine($"Time of the route: {e_Summary.Last().min} {scope.DistanceUnit}");
            }

            void OnScopeProblemSolverAfterAssignment(object sender, DefaultRoutingAssignmentEventArgs e)
            {
                var context_Dim = context.Dimensions.OfType<TimeWindowDimension>().AssertCollectionNotEmpty()
                    .SingleOrDefault().AssertNotNull();

                var e_Summary = OnSummarizeAssignments(context_Dim.MutableDimension, e.Solution, e.Assignments).ToArray();

                this.OutputHelper.WriteLine($"Total time of all routes: {e_Summary.GroupBy(_ => _.vehicle).Sum(_ => _.Last().min)}"
                    + $" {scope.DistanceUnit}");
            }

            $"And connect appropriate event handlers".x(() =>
            {
                scope.ProblemSolver.AfterAssignmentVehicle += OnScopeProblemSolverAfterAssignmentVehicle;
                scope.ProblemSolver.AfterAssignment += OnScopeProblemSolverAfterAssignment;
            }).Rollback(() =>
            {
                // TODO: TBD: which is occurring "after" scope has been disposed?
                // And disconnect on Rollback.
                scope.ProblemSolver.AfterAssignmentVehicle -= OnScopeProblemSolverAfterAssignmentVehicle;
                scope.ProblemSolver.AfterAssignment -= OnScopeProblemSolverAfterAssignment;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#main"/>
        /// <param name="scope"></param>
        [Scenario/*(Skip = "Remains an issue the expected solution is similar to a point then falls apart.")*/]
        public void Verify_ProblemSolver_Solution(TimeWindowCaseStudyScope scope)
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
        protected virtual void OnVerifyTimeWindowSolutionScope(TimeWindowCaseStudyScope scope) =>
            scope.AssertNotNull().VerifyAndReportSolution();

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what
        /// we expected. We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        protected override void OnVerifySolutionScope(TimeWindowCaseStudyScope scope)
        {
            base.OnVerifySolutionScope(scope);

            this.OnVerifyTimeWindowSolutionScope(scope);
        }
    }
}
