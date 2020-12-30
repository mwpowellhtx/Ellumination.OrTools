namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using LocalSearchMetaheuristic = Google.OrTools.ConstraintSolver.LocalSearchMetaheuristic;
    using FirstSolutionStrategyType = Google.OrTools.ConstraintSolver.FirstSolutionStrategy.Types.Value;

    /// <summary>
    /// Provides a Routing friendly type bridging to the <see cref="FirstSolutionStrategyType"/>
    /// Google type.
    /// </summary>
    public enum FirstSolutionStrategy
    {
        /// <summary>
        /// See the <em>homonymous value</em> in <see cref="LocalSearchMetaheuristic"/>.
        /// </summary>
        /// <see cref="LocalSearchMetaheuristic"/>
        /// <see cref="!:https://dictionary.com/browse/homonymous">Of or having the same name.</see>
        Unset = FirstSolutionStrategyType.Unset,

        /// <summary>
        /// <b>Variable-based heuristics</b>, iteratively connect two nodes which produce the
        /// cheapest route segment.
        /// </summary>
        GlobalCheapestArc = FirstSolutionStrategyType.GlobalCheapestArc,

        /// <summary>
        /// Select the first node with an unbound successor and connect it to the node which
        /// produces the cheapest route segment.
        /// </summary>
        LocalCheapestArc = FirstSolutionStrategyType.LocalCheapestArc,

        /// <summary>
        /// <b>Path addition heuristics</b>, starting from a route <em>start</em> node, connect
        /// it to the node which produces the cheapest route segment, then extend the route by
        /// iterating on the last node added to the route.
        /// </summary>
        PathCheapestArc = FirstSolutionStrategyType.PathCheapestArc,

        /// <summary>
        /// Same as <see cref="PathCheapestArc"/>, but arcs are evaluated with a comparison-based
        /// selector which will favor the most constrained arc first. To assign a selector to the
        /// routing model, see <c>RoutingModel.ArcIsMoreConstrainedThanArc</c> for details.
        /// </summary>
        /// <see cref="!:https://github.com/google/or-tools/blob/master/ortools/constraint_solver/routing.h#L1296"/>
        /// <see cref="!:https://github.com/google/or-tools/blob/master/ortools/constraint_solver/routing.cc#L3959"/>
        PathMostConstrainedArc = FirstSolutionStrategyType.PathCheapestArc,

        /// <summary>
        /// Same as <see cref="PathCheapestArc"/>, except that arc costs are evaluated using the
        /// function passed to <c>RoutingModel.SetFirstSolutionEvaluator</c>.
        /// </summary>
        /// <see cref="!:https://github.com/google/or-tools/blob/master/ortools/constraint_solver/routing.h#L956"/>
        EvaluatorStrategy = FirstSolutionStrategyType.EvaluatorStrategy,

        /// <summary>
        /// <b>Path insertion heuristics</b>, make all nodes inactive. Only finds a solution
        /// if nodes are optional, are element of a disjunction constraint with a finite penalty
        /// cost.
        /// </summary>
        AllUnperformed = FirstSolutionStrategyType.AllUnperformed,

        /// <summary>
        /// Iteratively build a solution by inserting the cheapest node at its cheapest position.
        /// The cost of insertion is based on the global cost function of the routing model. As of
        /// <em>2/2012</em>, only works on models with optional nodes, with finite penalty costs.
        /// </summary>
        BestInsertion = FirstSolutionStrategyType.BestInsertion,

        /// <summary>
        /// Iteratively build a solution by inserting the cheapest node at its cheapest position.
        /// The cost of insertion is based on the arc cost function. Is faster than
        /// <see cref="BestInsertion"/>.
        /// </summary>
        ParallelCheapestInsertion = FirstSolutionStrategyType.ParallelCheapestInsertion,

        /// <summary>
        /// Iteratively build a solution by inserting each node at its cheapest position. The
        /// cost of insertion is based on the arc cost function. Differs from
        /// <see cref="ParallelCheapestInsertion"/> by the node selected for insertion, here
        /// nodes are considered in decreasing order of distance to the start and ends of the
        /// routes, i.e. farthest nodes are inserted first, is faster than
        /// <see cref="SequentialCheapestInsertion"/>.
        /// </summary>
        LocalCheapestInsertion = FirstSolutionStrategyType.LocalCheapestInsertion,

        /// <summary>
        /// Savings algorithm (Clarke &amp; Wright).
        /// </summary>
        /// <see cref="!:https://pubsonline.informs.org/doi/abs/10.1287/opre.12.4.568">Clarke, G.
        /// &amp; Wright, J.W., <em>Scheduling of Vehicles from a Central Depot to a Number of
        /// Delivery Points</em>, Operations Research, Vol. 12, 1964, pp. 568-581</see>
        Savings = FirstSolutionStrategyType.Savings,

        /// <summary>
        /// Sweep algorithm (Wren &amp; Holliday).
        /// </summary>
        /// <see cref="!:https://doi.org/10.1057/jors.1972.53">Wren, A., Holliday, A.,
        /// <em>Computer Scheduling of Vehicles from One or More Depots to a Number of
        /// Delivery Points</em>, J Oper Res Soc 23, 333–344 (1972)</see>
        /// <see cref="!:https://link.springer.com/article/10.1057/jors.1972.53">Wren,
        /// A., Holliday, A., <em>Computer Scheduling of Vehicles from One or More Depots
        /// to a Number of Delivery Points</em>, J Oper Res Soc 23, 333–344 (1972)</see>
        Sweep = FirstSolutionStrategyType.Sweep,

        /// <summary>
        /// Select the first node with an unbound successor and connect it to the first available
        /// node. This is equivalent to the <c>CHOOSE_FIRST_UNBOUND</c> <em>strategy</em> combined
        /// with <c>ASSIGN_MIN_VALUE</c> <em>value</em>.
        /// </summary>
        /// <see cref="!:https://github.com/google/or-tools/blob/master/ortools/constraint_solver/constraint_solver.h#L279"/>
        /// <see cref="!:https://github.com/google/or-tools/blob/master/ortools/constraint_solver/constraint_solver.h#L358"/>
        FirstUnboundMinValue = FirstSolutionStrategyType.FirstUnboundMinValue,

        /// <summary>
        /// <em>Christofides algorithm</em>, actually a variant of the <em>Christofides
        /// algorithm</em> using a maximal matching instead of a maximum matching, which does not
        /// guarantee the 3/2 factor of the approximation on a metric travelling salesman). Works
        /// on generic vehicle routing models by extending a route until no nodes can be inserted
        /// on it.
        /// </summary>
        /// <see cref="!:https://en.wikipedia.org/wiki/Approximation_algorithm"/>
        /// <see cref="!:https://en.wikipedia.org/wiki/Christofides_algorithm"/>
        /// <see cref="!:https://apps.dtic.mil/dtic/tr/fulltext/u2/a025602.pdf">Nicos Christofides,
        /// <em>Worst-case analysis of a new heuristic for the travelling salesman problem</em>,
        /// Report 388, Graduate School of Industrial Administration, CMU, 1976.</see>
        Christofides = FirstSolutionStrategyType.Christofides,

        /// <summary>
        /// Iteratively build a solution by constructing routes sequentially, for each route
        /// inserting the cheapest node at its cheapest position until the route is completed.
        /// The cost of insertion is based on the arc cost function, faster than
        /// <see cref="ParallelCheapestInsertion"/>.
        /// </summary>
        /// <see cref="!:https://github.com/google/or-tools/blob/master/ortools/constraint_solver/routing_enums.proto#L81"/>
        SequentialCheapestInsertion = FirstSolutionStrategyType.SequentialCheapestInsertion,

        /// <summary>
        /// Lets the solver detect which strategy to use according to the model being solved.
        /// </summary>
        Automatic = FirstSolutionStrategyType.Automatic
    }
}
