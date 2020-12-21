using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Assignment = Google.OrTools.ConstraintSolver.Assignment;
    using RoutingSearchParameters = Google.OrTools.ConstraintSolver.RoutingSearchParameters;
    using OrConstraintSolver = Google.OrTools.ConstraintSolver.operations_research_constraint_solver;
    using Duration = Google.Protobuf.WellKnownTypes.Duration;
    /*
     * TODO: TBD: introduce an intermediate enum with translation to/from or-tools values...
     * TODO: TBD: mostly because the reason being there is too much class nesting there...
     * TODO: TBD: and for more convenient usage...
            public enum Value
            {
                //
                // Summary:
                //     See the homonymous value in LocalSearchMetaheuristic.
                Unset = 0,
                //
                // Summary:
                //     --- Variable-based heuristics --- Iteratively connect two nodes which produce
                //     the cheapest route segment.
                GlobalCheapestArc = 1,
                //
                // Summary:
                //     Select the first node with an unbound successor and connect it to the node which
                //     produces the cheapest route segment.
                LocalCheapestArc = 2,
                //
                // Summary:
                //     --- Path addition heuristics --- Starting from a route "start" node, connect
                //     it to the node which produces the cheapest route segment, then extend the route
                //     by iterating on the last node added to the route.
                PathCheapestArc = 3,
                //
                // Summary:
                //     Same as PATH_CHEAPEST_ARC, but arcs are evaluated with a comparison-based selector
                //     which will favor the most constrained arc first. To assign a selector to the
                //     routing model, see RoutingModel::ArcIsMoreConstrainedThanArc() in routing.h for
                //     details.
                PathMostConstrainedArc = 4,
                //
                // Summary:
                //     Same as PATH_CHEAPEST_ARC, except that arc costs are evaluated using the function
                //     passed to RoutingModel::SetFirstSolutionEvaluator() (cf. routing.h).
                EvaluatorStrategy = 5,
                //
                // Summary:
                //     --- Path insertion heuristics --- Make all nodes inactive. Only finds a solution
                //     if nodes are optional (are element of a disjunction constraint with a finite
                //     penalty cost).
                AllUnperformed = 6,
                //
                // Summary:
                //     Iteratively build a solution by inserting the cheapest node at its cheapest position;
                //     the cost of insertion is based on the global cost function of the routing model.
                //     As of 2/2012, only works on models with optional nodes (with finite penalty costs).
                BestInsertion = 7,
                //
                // Summary:
                //     Iteratively build a solution by inserting the cheapest node at its cheapest position;
                //     the cost of insertion is based on the arc cost function. Is faster than BEST_INSERTION.
                ParallelCheapestInsertion = 8,
                //
                // Summary:
                //     Iteratively build a solution by inserting each node at its cheapest position;
                //     the cost of insertion is based on the arc cost function. Differs from PARALLEL_CHEAPEST_INSERTION
                //     by the node selected for insertion; here nodes are considered in decreasing order
                //     of distance to the start/ends of the routes, i.e. farthest nodes are inserted
                //     first. Is faster than SEQUENTIAL_CHEAPEST_INSERTION.
                LocalCheapestInsertion = 9,
                //
                // Summary:
                //     Savings algorithm (Clarke & Wright). Reference: Clarke, G. & Wright, J.W.: "Scheduling
                //     of Vehicles from a Central Depot to a Number of Delivery Points", Operations
                //     Research, Vol. 12, 1964, pp. 568-581
                Savings = 10,
                //
                // Summary:
                //     Sweep algorithm (Wren & Holliday). Reference: Anthony Wren & Alan Holliday: Computer
                //     Scheduling of Vehicles from One or More Depots to a Number of Delivery Points
                //     Operational Research Quarterly (1970-1977), Vol. 23, No. 3 (Sep., 1972), pp.
                //     333-344
                Sweep = 11,
                //
                // Summary:
                //     Select the first node with an unbound successor and connect it to the first available
                //     node. This is equivalent to the CHOOSE_FIRST_UNBOUND strategy combined with ASSIGN_MIN_VALUE
                //     (cf. constraint_solver.h).
                FirstUnboundMinValue = 12,
                //
                // Summary:
                //     Christofides algorithm (actually a variant of the Christofides algorithm using
                //     a maximal matching instead of a maximum matching, which does not guarantee the
                //     3/2 factor of the approximation on a metric travelling salesman). Works on generic
                //     vehicle routing models by extending a route until no nodes can be inserted on
                //     it. Reference: Nicos Christofides, Worst-case analysis of a new heuristic for
                //     the travelling salesman problem, Report 388, Graduate School of Industrial Administration,
                //     CMU, 1976.
                Christofides = 13,
                //
                // Summary:
                //     Iteratively build a solution by constructing routes sequentially, for each route
                //     inserting the cheapest node at its cheapest position until the route is completed;
                //     the cost of insertion is based on the arc cost function. Is faster than PARALLEL_CHEAPEST_INSERTION.
                SequentialCheapestInsertion = 14,
                //
                // Summary:
                //     Lets the solver detect which strategy to use according to the model being solved.
                Automatic = 15
            }
     */
    using FirstSolutionStrategyType = Google.OrTools.ConstraintSolver.FirstSolutionStrategy.Types.Value;
    /*
     * TODO: TBD: ditto here...
            public enum Value
            {
                //
                // Summary:
                //     Means "not set". If the solver sees that, it'll behave like for AUTOMATIC. But
                //     this value won't override others upon a proto MergeFrom(), whereas "AUTOMATIC"
                //     will.
                Unset = 0,
                //
                // Summary:
                //     Accepts improving (cost-reducing) local search neighbors until a local minimum
                //     is reached.
                GreedyDescent = 1,
                //
                // Summary:
                //     Uses guided local search to escape local minima (cf. http://en.wikipedia.org/wiki/Guided_Local_Search);
                //     this is generally the most efficient metaheuristic for vehicle routing.
                GuidedLocalSearch = 2,
                //
                // Summary:
                //     Uses simulated annealing to escape local minima (cf. http://en.wikipedia.org/wiki/Simulated_annealing).
                SimulatedAnnealing = 3,
                //
                // Summary:
                //     Uses tabu search to escape local minima (cf. http://en.wikipedia.org/wiki/Tabu_search).
                TabuSearch = 4,
                //
                // Summary:
                //     Uses tabu search on a list of variables to escape local minima. The list of variables
                //     to use must be provided via the SetTabuVarsCallback callback.
                GenericTabuSearch = 5,
                //
                // Summary:
                //     Lets the solver select the metaheuristic.
                Automatic = 6
            }
     */
    using LocalSearchMetaheuristic = Google.OrTools.ConstraintSolver.LocalSearchMetaheuristic.Types.Value;

    // TODO: TBD: from problem solver? to what? "plugin"? "visitor" pattern?
    /// <summary>
    /// The idea here is that you derive your <typeparamref name="TContext"/>
    /// according to your modeling needs. You similarly derive
    /// a <see cref="AssignableRoutingProblemSolver{TContext, TAssign}"/> along
    /// the same lines, and provide the hooks for the <see cref="Dimension"/> implementations
    /// that apply to your model. This is where you will
    /// <see cref="ContextExtensionMethods.AddDimension{TContext, TDimension}"/> each of those
    /// Dimensions. You may similarly apply any visitors that might otherwise mutate your Context
    /// prior to running the solution to assignment.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TAssign"></typeparam>
    /// <see cref="RoutingContext"/>
    /// <see cref="RoutingAssignmentEventArgs{TContext}"/>
    public abstract class AssignableRoutingProblemSolver<TContext, TAssign>
        : IAssignableRoutingProblemSolver<TContext, TAssign>
        where TContext : RoutingContext
        where TAssign : RoutingAssignmentEventArgs<TContext>
    {
        /// <summary>
        /// Gets the Visitors which apply for the
        /// <see cref="AssignableRoutingProblemSolver{TContext, TAssign}"/>.
        /// Override in order to specify the Visitors that Apply to the
        /// <typeparamref name="TContext"/>. Ordering or other constraints are completely left
        /// to consumer discretion as to how to present those Visitors, in what order, etc.
        /// </summary>
        protected virtual IEnumerable<IVisitor<TContext>> Visitors => Array.Empty<IVisitor<TContext>>();

        /// <summary>
        /// Applies the <paramref name="visitor"/> to the <paramref name="context"/>
        /// in the <see cref="Apply"/> Aggregate.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected virtual TContext OnAggregateVisitor(TContext context, IVisitor<TContext> visitor) => visitor.Apply(context);

        /// <summary>
        /// Applies each of the <paramref name="visitors"/> instances to the
        /// <paramref name="context"/>, allowing for each opportunity to potentially
        /// mutate the Context accordingly.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="visitors"></param>
        /// <returns></returns>
        private TContext Apply(TContext context, params IVisitor<TContext>[] visitors) => visitors.Aggregate(context, this.OnAggregateVisitor);

        /// <summary>
        /// Applies any Visitors to the <paramref name="context"/> prior to
        /// <see cref="AddDimensions"/>. This is critical, since any mutations to the Context
        /// will potentially disrupt the Dimension, so these must be resolved beforehand.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual TContext ApplyVisitors(TContext context) => this.Apply(context, this.Visitors.ToArray());

        /// <summary>
        /// Adds the Dimensions corresponding with the <paramref name="context"/>.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void AddDimensions(TContext context)
        {
        }

        /// <summary>
        /// Invokes the <see cref="ConfigureSearchParameters"/>.
        /// </summary>
        /// <param name="searchParams"></param>
        protected virtual void OnConfigureSearchParameters(ref RoutingSearchParameters searchParams)
        {
            var args = new RoutingSearchParametersEventArgs(searchParams);
            this.ConfigureSearchParameters?.Invoke(this, args);
            searchParams = args.SearchParameters;
        }

        /// <inheritdoc/>
        public event EventHandler<RoutingSearchParametersEventArgs> ConfigureSearchParameters;

        private RoutingSearchParameters _searchParams = OrConstraintSolver.DefaultRoutingSearchParameters();

        /// <summary>
        /// Gets the SearchParameters involved during the <see cref="Solve"/> operation.
        /// </summary>
        private RoutingSearchParameters SearchParameters
        {
            get
            {
                this.OnConfigureSearchParameters(ref this._searchParams);
                return this._searchParams;
            }
        }

        /// <summary>
        /// Returns a Created <see cref="RoutingAssignmentEventArgs{TContext}"/> given
        /// <paramref name="context"/> and <paramref name="assignments"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="assignments"></param>
        /// <returns></returns>
        protected virtual TAssign CreateAssignEventArgs(TContext context, params (int vehicle, int node, int? previousNode)[] assignments) =>
            (TAssign)Activator.CreateInstance(typeof(TAssign), context, assignments);

        /// <inheritdoc/>
        public virtual event EventHandler<TAssign> BeforeAssignment;

        /// <inheritdoc/>
        public virtual event EventHandler<TAssign> AfterAssignment;

        /// <inheritdoc/>
        public virtual event EventHandler<TAssign> BeforeAssignmentVehicle;

        /// <inheritdoc/>
        public virtual event EventHandler<TAssign> AfterAssignmentVehicle;

        /// <inheritdoc/>
        public virtual event EventHandler<TAssign> ForEachAssignmentNode;

        /// <summary>
        /// <see cref="BeforeAssignment"/> <typeparamref name="TAssign"/> dispatcher.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeAssignment(TAssign e) => this.BeforeAssignment?.Invoke(this, e);

        /// <summary>
        /// <see cref="AfterAssignment"/> <typeparamref name="TAssign"/> dispatcher.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAfterAssignment(TAssign e) => this.AfterAssignment?.Invoke(this, e);

        /// <summary>
        /// <see cref="BeforeAssignmentVehicle"/> <typeparamref name="TAssign"/> dispatcher.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeAssignmentVehicle(TAssign e) => this.BeforeAssignmentVehicle?.Invoke(this, e);

        /// <summary>
        /// <see cref="AfterAssignmentVehicle"/> <typeparamref name="TAssign"/>  dispatcher.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAfterAssignmentVehicle(TAssign e) => this.AfterAssignmentVehicle?.Invoke(this, e);

        /// <summary>
        /// <see cref="ForEachAssignmentNode"/> <typeparamref name="TAssign"/> dispatcher.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnForEachAssignmentNode(TAssign e) => this.ForEachAssignmentNode?.Invoke(this, e);

        /// <summary>
        /// Tries to Process the <see cref="Assignment"/> <paramref name="solution"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="solution"></param>
        /// <returns></returns>
        /// <see cref="OnBeforeAssignment"/>
        /// <see cref="OnAfterAssignment"/>
        /// <see cref="OnBeforeAssignmentVehicle"/>
        /// <see cref="OnAfterAssignmentVehicle"/>
        /// <see cref="OnForEachAssignmentNode"/>
        protected virtual bool TryProcessSolution(TContext context, Assignment solution)
        {
            // A simple shorthand local helper method.
            TAssign OnCreateAssignEventArgs(IEnumerable<(int vehicle, int node, int? previousNode)> assignments) =>
                this.CreateAssignEventArgs(context, assignments.ToArray());

            var context_Model = context.Model;
            var context_VehicleCount = context.VehicleCount;

            var solution_Assignments = new List<(int vehicle, int node, int? previousNode)>();

            // TODO: TBD: sketching in the callbacks...
            // TODO: TBD: we think that perhaps an "assignment" can be an IEnumerable of the corresponding tuple...
            // TODO: TBD: that should give us adequate flexibility how to report, when, with what context, etc...
            this.OnBeforeAssignment(OnCreateAssignEventArgs(solution_Assignments));

            // i: vehicle index, specifically declared int for clarity.
            for (int vehicleIndex = 0; vehicleIndex < context_VehicleCount; vehicleIndex++)
            {
                long j;
                long? previous = null;

                var vehicle_Assignments = new List<(int vehicle, int node, int? previousNode)>();

                this.OnBeforeAssignmentVehicle(OnCreateAssignEventArgs(vehicle_Assignments));

                // Yes, we are able to assembly an Assignment tuple given the available bits.
                (int vehicle, int node, int? previousNode) CreateAssignmentTuple() => (
                    vehicleIndex
                    , context.IndexToNode(j)
                    , previous.HasValue ? context.IndexToNode(previous.Value) : (int?)null
                );

                // The same whether within the Node loop, or the "last" leg.
                void OnForEachAssignments(params (int vehicle, int node, int? previousNode)[] assignments)
                {
                    this.OnForEachAssignmentNode(OnCreateAssignEventArgs(assignments));
                    vehicle_Assignments.AddRange(assignments);
                }

                // j: node index, specifically declared long for clarity.
                for (j = context_Model.Start(vehicleIndex); !context_Model.IsEnd(j); j = solution.Value(context_Model.NextVar(j)))
                {
                    OnForEachAssignments(CreateAssignmentTuple());
                    previous = j;
                }

                OnForEachAssignments(CreateAssignmentTuple());

                solution_Assignments.AddRange(vehicle_Assignments);
                this.OnAfterAssignmentVehicle(OnCreateAssignEventArgs(vehicle_Assignments));
            }

            this.OnAfterAssignment(OnCreateAssignEventArgs(solution_Assignments));

            return true;
        }

        /// <summary>
        /// Tries to Solve the <paramref name="context"/> <see cref="RoutingContext.Model"/>
        /// given <paramref name="searchParams"/>. Receives the <paramref name="solution"/>
        /// prior to response.
        /// </summary>
        /// <param name="context">The Context whose Model is being Solved. Assumes that the
        /// <see cref="Visitors"/> have all been applied and any <see cref="Context"/> mutations
        /// that were going to occur have all occurred. So, should be safe now to add dimensions
        /// to the <see cref="RoutingContext.Model"/>.</param>
        /// <param name="searchParams">The SearchParameters influencing the
        /// solution.</param>
        /// <param name="solution">Receives the Solution on response.</param>
        /// <returns>Whether a <paramref name="solution"/> was properly received.</returns>
        protected virtual bool TrySolve(TContext context, RoutingSearchParameters searchParams, out Assignment solution)
        {
            this.AddDimensions(context);

            var model = context.Model;

            solution = null;

            try
            {
                // TODO: TBD: is there a way to probe for status during and/or before/after the search itself?
                solution = searchParams == null
                    ? model.Solve()
                    : model.SolveWithParameters(searchParams);

                /* TODO: TBD: we can capture "debug" output from the solver,
                 * but as far as we can determine this requires/assumes there
                 * was a solution obtained from the solver in the first place.
                 * When solution is null, then what? */
            }
            catch //(Exception ex)
            {
                // TODO: TBD: potentially log any exceptions...
            }

            return solution != null;
        }


        /// <inheritdoc/>
        public virtual void Solve(TContext context)
        {
            Assignment solution;

            {
                context = this.ApplyVisitors(context);

                // TODO: TBD: could it be that search params are in fact required after all?
                var searchParams = this.SearchParameters ?? OrConstraintSolver.DefaultRoutingSearchParameters();
                searchParams.FirstSolutionStrategy = FirstSolutionStrategyType.PathCheapestArc;
                //searchParams.LocalSearchMetaheuristic = LocalSearchMetaheuristic.GuidedLocalSearch;
                //searchParams.TimeLimit = new Duration { Seconds = 7 };

                if (!this.TrySolve(context, searchParams, out solution))
                {
                    return;
                }
            }

            // Problem solver owns the Solution so therefore we can use it here.
            using (solution)
            {
                if (!this.TryProcessSolution(context, solution))
                {
                    // TODO: TBD: log when processing could not be accomplished...
                }
            }
        }

        /// <summary>
        /// Gets whether IsDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.Dispose(true);
            }

            this.IsDisposed = true;
        }
    }
}
