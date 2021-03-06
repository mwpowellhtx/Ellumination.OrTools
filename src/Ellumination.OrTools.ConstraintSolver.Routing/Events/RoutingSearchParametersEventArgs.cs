﻿using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using RoutingSearchParameters = Google.OrTools.ConstraintSolver.RoutingSearchParameters;

    /// <summary>
    /// Provides an <see cref="EventHandler{TEventArgs}"/> opportunity to relay some options
    /// into the Routing Search algorithms.
    /// </summary>
    public class RoutingSearchParametersEventArgs : EventArgs
    {
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
            this.Parameters = searchParams;
        }
    }
}
