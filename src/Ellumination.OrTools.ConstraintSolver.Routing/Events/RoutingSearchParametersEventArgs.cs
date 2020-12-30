using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using RoutingSearchParameters = Google.OrTools.ConstraintSolver.RoutingSearchParameters;

    /// <summary>
    /// 
    /// </summary>
    public class RoutingSearchParametersEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or Sets the SearchParameters.
        /// </summary>
        public RoutingSearchParameters SearchParameters { get; set; }

        /// <summary>
        /// Gets or Sets the SearchParameters.
        /// </summary>
        public SearchParameters Parameters { get; set; }

        /// <summary>
        /// Constructs a new Event Arguments instance.
        /// </summary>
        /// <param name="searchParams"></param>
        internal RoutingSearchParametersEventArgs(RoutingSearchParameters searchParams)
        {
            this.SearchParameters = searchParams;
            this.Parameters = searchParams;
        }
    }
}
