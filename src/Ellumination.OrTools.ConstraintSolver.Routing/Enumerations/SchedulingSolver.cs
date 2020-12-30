namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using SchedulingSolverType = Google.OrTools.ConstraintSolver.RoutingSearchParameters.Types.SchedulingSolver;

    /// <summary>
    /// Underlying solver to use in dimension scheduling, respectively for continuous and mixed
    /// models.
    /// </summary>
    public enum SchedulingSolver
    {
        /// <summary>
        /// <see cref="SchedulingSolverType.Unset"/>
        /// </summary>
        Unset = SchedulingSolverType.Unset,

        /// <summary>
        /// <see cref="SchedulingSolverType.Glop"/>
        /// </summary>
        Glop = SchedulingSolverType.Glop,

        /// <summary>
        /// <see cref="SchedulingSolverType.CpSat"/>
        /// </summary>
        CpSat = SchedulingSolverType.CpSat
    }
}
