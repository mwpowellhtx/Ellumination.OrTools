using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Google.OrTools.ConstraintSolver;

    // https://developers.google.com/optimization/routing/pickup_delivery
    // https://developers.google.com/optimization/reference/constraint_solver/routing_index_manager/RoutingIndexManager/
    // https://github.com/google/or-tools/blob/v8.0/ortools/constraint_solver/routing_index_manager.cc#L26
    // https://github.com/google/or-tools
    // https://developers.google.com/optimization/routing/routing_options
    // https://developers.google.com/optimization/routing/routing_tasks
    // https://developers.google.com/optimization/routing/routing_tasks#setting-start-and-end-locations-for-routes
    // https://developers.google.com/optimization/routing/penalties
    // https://developers.google.com/optimization/routing/google_direction
    // https://developers.google.com/optimization/routing/cvrptw_resources
    // https://developers.google.com/optimization/routing/vrptw
    // https://developers.google.com/optimization/routing/pickup_delivery
    // https://developers.google.com/optimization/routing/cvrp#c_1
    // https://developers.google.com/optimization/routing/tsp
    // https://developers.google.com/optimization/routing
    // https://github.com/settings/repositories
    // https://github.com/mwpowellhtx/Ellumination.OrTools

    /// <summary>
    /// See also <see cref="Context"/> for notes. Same concerns vetted with this Context as with
    /// the base class. Basically, if changes to the moving parts, either <see cref="Nodes"/>
    /// or <see cref="Vehicles"/> or both, then we suggest creating a fresh <see cref="Context"/>
    /// instance.
    /// </summary>
    /// <typeparam name="TNode">From a domain modeling perspective &quot;Node&quot; is simply
    /// a moniker. Used to represent the destination or points of interest being traversed by
    /// each <typeparamref name="TVehicle"/>.</typeparam>
    /// <typeparam name="TVehicle">From a domain modeling perspective, &quot;Vehicle&quot;
    /// is just a moniker. Used to represent whatever or whom ever is traversing the network
    /// of <typeparamref name="TNode"/> instances during the Routing solution.</typeparam>
    public abstract class Context<TNode, TVehicle> : Context
    {
        /// <summary>
        /// Gets or Sets the Nodes.
        /// </summary>
        /// <remarks>At this level Nodes and Vehicles are simply along for the ride. Any solver
        /// initialization that will have been performed will have been done at the base class
        /// level.</remarks>
        public virtual IEnumerable<TNode> Nodes { get; set; }

        /// <summary>
        /// Gets or Sets the Vehicles.
        /// </summary>
        public virtual IEnumerable<TVehicle> Vehicles { get; set; }

        /// <summary>
        /// Protected Context Constructor with unspecied <see cref="RoutingModelParameters"/>.
        /// </summary>
        /// <param name="nodes">The number of nodes or locations involved in the Context.</param>
        /// <param name="vehicles">The number of vehicles involved in the Context.</param>
        /// <param name="depot">Meaning &quot;home base&quot; or &quot;headquarters&quot;,
        /// basically where ever the vehicles are operating as their base of operations for
        /// scheduling purposes.</param>
        protected Context(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles, int depot = 0)
            : this(nodes, vehicles, depot, null)
        {
        }

        /// <summary>
        /// Protected Context Constructor.
        /// </summary>
        /// <param name="nodes">The number of nodes or locations involved in the Context.</param>
        /// <param name="vehicles">The number of vehicles involved in the Context.</param>
        /// <param name="depot">Meaning &quot;home base&quot; or &quot;headquarters&quot;,
        /// basically where ever the vehicles are operating as their base of operations for
        /// scheduling purposes.</param>
        /// <param name="parameters"></param>
        protected Context(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles, int depot = 0, RoutingModelParameters parameters = null)
            : base(nodes.OrEmpty().Count(), vehicles.OrEmpty().Count(), depot, parameters)
        {
            // TODO: TBD: verify that the depot index is valid, that is, within range of the nodes.
            this.Nodes = nodes.OrEmpty().ToArray();
            this.Vehicles = vehicles.OrEmpty().ToArray();
        }

        /// <summary>
        /// Protected Context Constructor with unspecied <see cref="RoutingModelParameters"/>.
        /// </summary>
        /// <param name="nodes">The number of nodes or locations involved in the Context.</param>
        /// <param name="vehicles">The number of vehicles involved in the Context.</param>
        /// <param name="starts">Not every Vehicle necessarily Starts at the same location.</param>
        /// <param name="ends">Additionally, not every Vehicle may End at the same location.</param>
        protected Context(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles, IEnumerable<int> starts, IEnumerable<int> ends)
            : this(nodes, vehicles, starts, ends, null)
        {
        }

        /// <summary>
        /// Protected Context Constructor.
        /// </summary>
        /// <param name="nodes">The number of nodes or locations involved in the Context.</param>
        /// <param name="vehicles">The number of vehicles involved in the Context.</param>
        /// <param name="starts">Not every Vehicle necessarily Starts at the same location.</param>
        /// <param name="ends">Additionally, not every Vehicle may End at the same location.</param>
        /// <param name="modelParameters"></param>
        protected Context(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles, IEnumerable<int> starts, IEnumerable<int> ends, RoutingModelParameters modelParameters)
            : base(nodes.OrEmpty().Count(), vehicles.OrEmpty().Count(), starts, ends, modelParameters)
        {
            this.Nodes = nodes.OrEmpty().ToArray();
            this.Vehicles = vehicles.OrEmpty().ToArray();
        }
    }
}
