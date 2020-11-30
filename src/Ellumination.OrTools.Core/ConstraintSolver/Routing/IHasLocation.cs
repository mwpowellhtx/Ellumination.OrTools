using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Represents an asset that HasLocation.
    /// </summary>
    /// <see cref="!:https://thefreedictionary.com/locatable"/>
    /// <see cref="!:https://dictionary.com/browse/locatable"/>
    public interface ILocatable
    {
        /// <summary>
        /// Gets or Sets the Location.
        /// </summary>
        string Location { get; set; }
    }
}
