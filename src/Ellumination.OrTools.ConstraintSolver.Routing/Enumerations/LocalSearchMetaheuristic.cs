namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using LocalSearchMetaheuristic = Google.OrTools.ConstraintSolver.LocalSearchMetaheuristic;
    using LocalSearchMetaheuristicType = Google.OrTools.ConstraintSolver.LocalSearchMetaheuristic.Types.Value;

    /// <summary>
    /// Provides a Routing friendly type bridging to the
    /// <see cref="LocalSearchMetaheuristicType"/> Google type.
    /// </summary>
    public enum LocalSearchMetaheuristic
    {
        /// <summary>
        /// Means <em>not set</em>. If the solver sees that, it will assume
        /// <see cref="Automatic"/> behavior. But this value will not override
        /// others upon a proto <em>MergeFrom</em> , whereas <see cref="Automatic"/> will.
        /// </summary>
        /// <see cref="Automatic"/>
        Unset = LocalSearchMetaheuristicType.Unset,

        /// <summary>
        /// Accepts improving, <em>cost-reducing</em>, local search neighbors until
        /// a local minimum is reached.
        /// </summary>
        GreedyDescent = LocalSearchMetaheuristicType.GreedyDescent,

        /// <summary>
        /// Uses <em>guided local search</em> to escape local minima. This is generally
        /// the most efficient metaheuristic for vehicle routing.
        /// </summary>
        /// <see cref="!:http://en.wikipedia.org/wiki/Guided_Local_Search"/>
        GuidedLocalSearch = LocalSearchMetaheuristicType.GuidedLocalSearch,

        /// <summary>
        /// Uses <em>simulated annealing</em> to escape local minima.
        /// </summary>
        /// <see cref="!:http://en.wikipedia.org/wiki/Simulated_annealing"/>
        SimulatedAnnealing = LocalSearchMetaheuristicType.SimulatedAnnealing,

        /// <summary>
        /// Uses <em>tabu search</em> to escape local minima.
        /// </summary>
        /// <see cref="!:http://en.wikipedia.org/wiki/Tabu_search"/>
        TabuSearch = LocalSearchMetaheuristicType.TabuSearch,

        // TODO: TBD: !? SetTabuVarsCallback
        /// <summary>
        /// Uses <see cref="TabuSearch"/> on a list of variables to escape local minima. The list
        /// of variables to use must be provided via the <c>SetTabuVarsCallback</c> callback.
        /// </summary>
        GenericTabuSearch = LocalSearchMetaheuristicType.GenericTabuSearch,

        /// <summary>
        /// Lets the solver select the metaheuristic.
        /// </summary>
        Automatic = LocalSearchMetaheuristicType.Automatic
    }
}
