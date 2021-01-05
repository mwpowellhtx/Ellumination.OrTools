using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Distances;
    using Google.OrTools.ConstraintSolver;
    // Ditto re: Context aliases.
    using IEndpoints = IEnumerable<(int start, int end)>;
    using IEndpointCoordinates = IEnumerable<int>;

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
    /// See also <see cref="RoutingContext"/> for notes. Same concerns vetted with this
    /// Context as with the base class. Basically, if changes to the moving parts, either
    /// <see cref="Nodes"/> or <see cref="Vehicles"/> or both, then we suggest creating
    /// a fresh <see cref="RoutingContext"/> instance.
    /// </summary>
    /// <typeparam name="TNode">From a domain modeling perspective &quot;Node&quot; is
    /// simply a moniker. Used to represent the destination or points of interest being
    /// traversed by each <typeparamref name="TVehicle"/>.</typeparam>
    /// <typeparam name="TDepot">Some of the <see cref="Nodes"/> must of needs also be
    /// considered a Depot. The work of the Context ctors is to sort out the various
    /// permutations how that may be expressed when configuring your domain specific
    /// Context.</typeparam>
    /// <typeparam name="TVehicle">From a domain modeling perspective, &quot;Vehicle&quot;
    /// is just a moniker. Used to represent whatever or whom ever is traversing the network
    /// of <typeparamref name="TNode"/> instances during the Routing solution.</typeparam>
    public abstract class DomainContext<TNode, TDepot, TVehicle> : RoutingContext
        where TDepot : TNode
    {
        /// <summary>
        /// Gets the Nodes.
        /// </summary>
        /// <remarks>At this level Nodes and Vehicles are simply along for the ride. Any solver
        /// initialization that will have been performed will have been done at the base class
        /// level.</remarks>
        public virtual IEnumerable<TNode> Nodes { get; private set; }

        /// <summary>
        /// Returns whether the <paramref name="node"/> Is a <typeparamref name="TDepot"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsDepot(TNode node) => node is TDepot;

        /// <summary>
        /// Returns whether the <see cref="Nodes"/> At the <paramref name="node"/>
        /// index Is a <typeparamref name="TDepot"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsNodeDepot(int node) => IsDepot(this.Nodes.ElementAtOrDefault(node));

        /// <inheritdoc/>
        public override bool IsNodeAtIndexDepot(long index) => this.IsNodeDepot(this.IndexToNode(index));

        /// <summary>
        /// Gets the Depots from among the Nodes used to inform the model. The only caveat
        /// we should mention with this property is the loss of proper modeling perspective
        /// Index versus Node comprehension.
        /// </summary>
        /// <remarks>Note that it is not about edges so much as which <see cref="Nodes"/>
        /// represent a <typeparamref name="TDepot"/>.</remarks>
        /// <see cref="ManagerContext.IndexToNode"/>
        /// <see cref="ManagerContext.NodeToIndex"/>
        /// <see cref="ManagerContext.NodesToIndices"/>
        public virtual IEnumerable<TDepot> Depots => this.Nodes.OfType<TDepot>();

        /// <summary>
        /// Gets the Vehicles.
        /// </summary>
        public virtual IEnumerable<TVehicle> Vehicles { get; private set; }

        /// <summary>
        /// Gets or Sets the <see cref="Context.Distances"/>.
        /// </summary>
        /// <remarks></remarks>
        /// <see cref="Context.Distances"/>
        public new virtual LocationDistanceMatrix Distances
        {
            get => base.Distances as LocationDistanceMatrix;
            set => base.Distances = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="vehicle"></param>
        /// <param name="depotSelector"></param>
        /// <returns></returns>
        private static int FindEndpointCoord(IEnumerable<TNode> nodes, TVehicle vehicle
            , Func<TVehicle, TNode, bool> depotSelector = default) =>
            nodes.ToList().FindIndex(node => node is TDepot && depotSelector?.Invoke(vehicle, node) == true);

        /// <summary>
        /// Validates the <paramref name="depot"/> given <paramref name="nodes"/>.
        /// </summary>
        /// <param name="depot">The Depot being Validated.</param>
        /// <param name="nodes">The Nodes being Validated.</param>
        /// <returns>The <paramref name="depot"/> following Validation.</returns>
        private static int ValidateDepot(int depot, IEnumerable<TNode> nodes)
        {
            nodes = nodes.OrEmpty().ToArray();
            var nodes_Count = nodes.Count();

            if (depot < 0 || depot >= nodes_Count)
            {
                throw new ArgumentOutOfRangeException(nameof(depot), RenderOutOfRangeMessage(
                    (nameof(depot), depot)
                    , (nameof(nodes), RenderArray(nodes))
                ))
                {
                    Data = {
                        { nameof(depot), depot },
                        { nameof(nodes), nodes }
                    }
                };
            }

            // TODO: TBD: could call Type.FullName a more human readable rendering there...
            return nodes.ElementAtOrDefault(depot) is TDepot
                ? depot
                : throw new ArgumentException($"{nameof(nodes)}[{depot}] is not a"
                    + $" {typeof(TDepot).FullName} depot type", nameof(depot));
        }

        /// <summary>
        /// Validates the Endpoint <paramref name="coords"/> given <paramref name="nodes"/>
        /// and <paramref name="vehicles"/>.
        /// </summary>
        /// <param name="nodes">The Nodes being Validated.</param>
        /// <param name="vehicles">The Vehicles being Validated.</param>
        /// <param name="coords">The Endpoint Coordinates being Validated.</param>
        /// <returns>The Endpoint Coordinate <paramref name="coords"/> following Validation.</returns>
        /// <remarks>Ditto <see cref="ValidateDepot"/> concerning Validation.</remarks>
        private static IEndpointCoordinates ValidateEndpointCoords(IEnumerable<TNode> nodes
            , IEnumerable<TVehicle> vehicles, IEndpointCoordinates coords)
        {
            nodes = nodes.OrEmpty().ToArray();
            vehicles = vehicles.OrEmpty().ToArray();

            var vehicles_Count = vehicles.Count();

            int OnValidateEndpointCoord(int coord, int index)
            {
                if (index < 0 || index >= vehicles_Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), RenderOutOfRangeMessage(
                        (nameof(coord), coord),
                        (nameof(index), index)
                    ))
                    {
                        Data = {
                            { nameof(coord), coord },
                            { nameof(index), index },
                            { nameof(nodes), nodes },
                            { nameof(vehicles), vehicles },
                            { nameof(coords), coords }
                        }
                    };
                }

                try
                {
                    return ValidateDepot(coord, nodes);
                }
                catch
                {
                    throw new ArgumentOutOfRangeException(nameof(coord), RenderOutOfRangeMessage(
                        (nameof(coord), coord),
                        (nameof(index), index)
                    ))
                    {
                        Data = {
                            { nameof(coord), coord },
                            { nameof(index), index },
                            { nameof(nodes), nodes },
                            { nameof(vehicles), vehicles },
                            { nameof(coords), coords }
                        }
                    };
                }
            }

            return coords.Select(OnValidateEndpointCoord).ToArray();
        }

        /// <summary>
        /// Validates the <paramref name="eps"/> Endpoints given <paramref name="nodes"/>
        /// and <paramref name="vehicles"/>.
        /// </summary>
        /// <param name="nodes">The Nodes being Validated.</param>
        /// <param name="vehicles">The Vehicles being Validated.</param>
        /// <param name="eps">The Endpoints being Validated.</param>
        /// <returns>The <paramref name="eps"/> Endpoints following Validation.</returns>
        /// <remarks>Ditto <see cref="ValidateDepot"/> concerning Validation.</remarks>
        private static IEndpoints ValidateEndpoints(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles, IEndpoints eps)
        {
            eps = eps.OrEmpty();

            nodes = nodes.OrEmpty().ToArray();
            vehicles = vehicles.OrEmpty().ToArray();

            var eps_Count = eps.Count();
            var vehicles_Count = vehicles.Count();

            if (eps_Count != vehicles_Count)
            {
                throw new ArgumentException($"Endpoint {nameof(eps)} count {eps_Count}"
                    + $" must equal vehicle count {vehicles_Count}", nameof(eps)
                );
            }

            (int start, int end) OnValidateEndpoint((int start, int end) ep, int index)
            {
                var (start, end) = ep;

                try
                {
                    return (
                        ValidateDepot(start, nodes)
                        , ValidateDepot(end, nodes)
                    );
                }
                catch
                {
                    throw new ArgumentOutOfRangeException(nameof(ep), RenderOutOfRangeMessage(
                        (nameof(ep), RenderArray("()", start, end)),
                        (nameof(index), index)
                    ))
                    {
                        Data = {
                            { nameof(nodes), nodes },
                            { nameof(vehicles), vehicles },
                            { nameof(eps), eps },
                            { nameof(ep), ep },
                            { nameof(start), start },
                            { nameof(end), end },
                            { nameof(index), index }
                        }
                    };
                }
            }

            return eps.Select(OnValidateEndpoint).ToArray();
        }

        /// <summary>
        /// All Contexts are constructed knowing about <paramref name="nodes"/>
        /// and <paramref name="vehicles"/>.
        /// </summary>
        /// <param name="nodes">The number of nodes or locations involved in the Context.</param>
        /// <param name="vehicles">The number of vehicles involved in the Context.</param>
        public DomainContext(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles)
            : this(nodes, vehicles, default(int))
        {
        }

        /// <summary>
        /// Constructs a Context knowing about a single <paramref name="depot"/> and parameters.
        /// </summary>
        /// <param name="nodes">The number of nodes or locations involved in the Context.</param>
        /// <param name="vehicles">The number of vehicles involved in the Context.</param>
        /// <param name="depot">Meaning &quot;home base&quot; or &quot;headquarters&quot;,
        /// basically where ever the vehicles are operating as their base of operations for
        /// scheduling purposes.</param>
        public DomainContext(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles, int depot)
            : base(nodes.OrEmpty().Count(), vehicles.OrEmpty().Count(), ValidateDepot(depot, nodes))
        {
            this.Nodes = nodes.OrEmpty().ToArray();
            this.Vehicles = vehicles.OrEmpty().ToArray();
        }

        /// <summary>
        /// Constructs a Context knowing about Endpoint <paramref name="starts"/>
        /// and <paramref name="ends"/> Coordinates, and parameters.
        /// </summary>
        /// <param name="nodes">The number of nodes or locations involved in the Context.</param>
        /// <param name="vehicles">The number of vehicles involved in the Context.</param>
        /// <param name="starts">Not every Vehicle necessarily Starts at the same location.</param>
        /// <param name="ends">Additionally, not every Vehicle may End at the same location.</param>
        public DomainContext(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles, IEndpointCoordinates starts, IEndpointCoordinates ends)
            : base(nodes.OrEmpty().Count(), vehicles.OrEmpty().Count()
                  , ValidateEndpointCoords(nodes, vehicles, starts), ValidateEndpointCoords(nodes, vehicles, ends))
        {
            this.Nodes = nodes.OrEmpty().ToArray();
            this.Vehicles = vehicles.OrEmpty().ToArray();
        }

        /// <summary>
        /// Constructs a Context with the ability to invoke a <paramref name="depotSelector"/>
        /// for each of the <paramref name="vehicles"/> and <paramref name="nodes"/>.
        /// </summary>
        /// <param name="nodes">The number of nodes or locations involved in the Context.</param>
        /// <param name="vehicles">The number of vehicles involved in the Context.</param>
        /// <param name="depotSelector">A Depot selector.</param>
        public DomainContext(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles, Func<TVehicle, TNode, bool> depotSelector)
            : this(nodes, vehicles, vehicles.OrEmpty().Select(vehicle => FindEndpointCoord(nodes, vehicle, depotSelector)).ToArray())
        {
        }

        /// <summary>
        /// Constructs a Context knowing about Endpoint Starts and Ends
        /// <paramref name="epCoords"/>.
        /// </summary>
        /// <param name="nodes">The number of nodes or locations involved in the Context.</param>
        /// <param name="vehicles">The number of vehicles involved in the Context.</param>
        /// <param name="epCoords">The endpoint coordinates.</param>
        public DomainContext(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles, IEndpointCoordinates epCoords)
            // Whether it validates once, no need to repeat the request.
            : base(nodes.OrEmpty().Count(), vehicles.OrEmpty().Count(), ValidateEndpointCoords(nodes, vehicles, epCoords), epCoords)
            //                                                          ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^  ^^^^^^^^
        {
            this.Nodes = nodes.OrEmpty().ToArray();
            this.Vehicles = vehicles.OrEmpty().ToArray();
        }

        /// <summary>
        /// Constructs a Context knowing about the Endpoint <paramref name="eps"/> tuples.
        /// </summary>
        /// <param name="nodes">The number of nodes or locations involved in the Context.</param>
        /// <param name="vehicles">The number of vehicles involved in the Context.</param>
        /// <param name="eps">The Depot endpoints.</param>
        public DomainContext(IEnumerable<TNode> nodes, IEnumerable<TVehicle> vehicles, IEndpoints eps)
            : base(nodes.OrEmpty().Count(), vehicles.OrEmpty().Count(), ValidateEndpoints(nodes, vehicles, eps))
        {
        }
    }
}
