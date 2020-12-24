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
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    // TODO: TBD: making an alias here because we think we may refactor to collections suite...
    using Assignment = Google.OrTools.ConstraintSolver.Assignment;
    using DistanceMatrix = Distances.DistanceMatrix;
    using RoutingDimension = Google.OrTools.ConstraintSolver.RoutingDimension;
    using FirstSolutionStrategyType = Google.OrTools.ConstraintSolver.FirstSolutionStrategy.Types.Value;
    using static String;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TScope"></typeparam>
    /// <typeparam name="TDimension"></typeparam>
    /// <see cref="!:https://developers.google.com/optimization/routing/vrptw"/>
    public abstract class TimeWindowCaseStudyTests<TScope, TDimension> : OrToolsRoutingCaseStudyTests<
        DistanceMatrix, RoutingContext, DefaultRoutingAssignmentEventArgs, DefaultRoutingProblemSolver
        , TScope>
        where TScope : TimeWindowCaseStudyScope
        where TDimension : TimeWindowDimension
    {
        protected TimeWindowCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        protected virtual void OnBackgroundAddDimensions(TScope scope)
        {
        }

        /// <summary>
        /// Do some TimeWindowBackground.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <param name="matrix"></param>
        /// <param name="expectedPaths"></param>
        [Background]
        public void TimeWindowBackground(TScope scope, RoutingContext context, DistanceMatrix matrix, IEnumerable<int[]> expectedPaths)
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

            $"Arrange {nameof(scope)}.{nameof(scope.ExpectedTotalDistance)}".x(() =>
                expectedPaths = scope.ExpectedPaths.AssertEqual(scope.VehicleCount, x => x.Count)
            );

            // TODO: TBD: which even this is a bit boilerplate, for academic case study...
            $"Add {typeof(TDimension).Name} to {nameof(scope)}.{nameof(scope.Context)}".x(
                () => scope.Context.AddDefaultDimension<TDimension>(scope)
            );

            IEnumerable<(int vehicle, int node, long index, long min, long max)> OnSummarizeAssignments(
                RoutingDimension dim, Assignment solution, IEnumerable<RouteAssignmentItem> assignments) =>
                assignments.Select(x => (vehicle: x.Vehicle, node: x.Node, index: x.Index, var: dim.CumulVar(x.Index)))
                    .Select(_ => (_.vehicle, _.node, _.index, min: solution.Min(_.var), max: solution.Max(_.var)));

            void OnScopeProblemSolverAfterAssignmentVehicle(object sender, DefaultRoutingAssignmentEventArgs e)
            {
                var context_Dim = context.Dimensions.OfType<TDimension>().AssertCollectionNotEmpty()
                    .SingleOrDefault().AssertNotNull();

                var e_Summary = OnSummarizeAssignments(context_Dim.MutableDimension, e.Solution, e.Assignments).ToArray();

                var vehicle = e_Summary.Select(_ => _.vehicle).Distinct()
                    .ToArray().AssertEqual(1, _ => _.Length).Single();

                this.OutputHelper.WriteLine($"Route for vehicle {vehicle}");

                this.OutputHelper.WriteLine(Join(" -> ", e_Summary.Select(_ => $"{_.node} Time({_.min}, {_.max})")));

                // Verified by Distinct Vehicle above that we have at least One.
                this.OutputHelper.WriteLine($"Time of the route: {e_Summary.Last().min} {scope.DistanceUnit}");
            }

            void OnScopeProblemSolverAfterAssignment(object sender, DefaultRoutingAssignmentEventArgs e)
            {
                var context_Dim = context.Dimensions.OfType<TDimension>().AssertCollectionNotEmpty()
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
        public void Verify_ProblemSolver_Solution(TScope scope)
        {
            $"Verify {nameof(scope)}".x(() => scope = this.Scope.AssertNotNull());

            void OnConfigureSearchParameters(object sender, RoutingSearchParametersEventArgs e)
            {
                var e_searchParams = e.SearchParameters;
                e_searchParams.FirstSolutionStrategy = FirstSolutionStrategyType.PathCheapestArc;
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
        protected virtual void OnVerifyTimeWindowSolutionScope(TScope scope) =>
            scope.AssertNotNull().VerifyAndReportSolution();

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what
        /// we expected. We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        protected override void OnVerifySolutionScope(TScope scope)
        {
            base.OnVerifySolutionScope(scope);

            this.OnVerifyTimeWindowSolutionScope(scope);
        }
    }
}
