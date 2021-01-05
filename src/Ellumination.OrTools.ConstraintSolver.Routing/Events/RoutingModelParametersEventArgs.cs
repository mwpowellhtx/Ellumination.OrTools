using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using RoutingSearchParameters = Google.OrTools.ConstraintSolver.RoutingSearchParameters;

    /// <summary>
    /// Provides an <see cref="EventHandler{TEventArgs}"/> opportunity to relay some options
    /// into the Routing Model.
    /// </summary>
    public class RoutingModelParametersEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or Sets the ModelParameters. May leave this unset, or <c>null</c>, which
        /// initialize the Model without a <em>parameters</em> argument.
        /// </summary>
        public ModelParameters Parameters { get; set; }

        /// <summary>
        /// Constructs a new Event Arguments instance.
        /// </summary>
        internal RoutingModelParametersEventArgs()
        {
        }
    }
}
