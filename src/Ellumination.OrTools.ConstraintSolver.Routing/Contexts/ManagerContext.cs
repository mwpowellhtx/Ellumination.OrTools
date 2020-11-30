using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Google.OrTools.ConstraintSolver;
    // These make sense as aliases, but should not make them first class derivations.
    using IEndpoints = IEnumerable<(int start, int end)>;
    // This is because we want to keep these close to the generic IEnumerable.
    using IEndpointCoordinates = IEnumerable<int>;

    /// <summary>
    /// Extends the <see cref="Context"/> for use with the <see cref="RoutingIndexManager"/>.
    /// Everything in this intermediate base class is focused on the <see cref="Manager"/>.
    /// Also serves as the foundation for <see cref="RoutingContext"/>. While changes to the
    /// moving parts, Nodes or Vehicles, as the case may be, depending on the domain model,
    /// is possible, we advise against it in a given Context. If such mutations are desired,
    /// then create a new Context instance with those moving parts having been mutated.
    /// </summary>
    public abstract class ManagerContext : Context
    {
        /// <summary>
        /// Gets the NodeCount. Note that <em>Nodes</em>, so called, also include the
        /// <em>Depots</em>.
        /// </summary>
        public int NodeCount { get; }

        /// <summary>
        /// Gets the VehicleCount.
        /// </summary>
        public int VehicleCount { get; }

        /// <summary>
        /// Gets the Depot used to inform the Context.
        /// </summary>
        public int? Depot { get; }

        /// <summary>
        /// Gets the Vehicle Start and End Endpoints. Each of the Endpoint indices
        /// must align with the <see cref="NodeCount"/>.
        /// </summary>
        public virtual IEndpoints Endpoints { get; private set; }

        /// <summary>
        /// Returns the Decoupled <paramref name="ep"/> Endpoint.
        /// </summary>
        /// <param name="ep">An Endpoint to be Decoupled.</param>
        /// <returns></returns>
        private static IEndpointCoordinates DecoupleEndpoint((int start, int end) ep)
        {
            var (start, end) = ep;
            yield return start;
            yield return end;
        }

        /// <summary>
        /// Gets the DepotCoordinates based upon the Coupled <see cref="Endpoints"/>.
        /// </summary>
        public virtual IEndpointCoordinates DepotCoordinates => this.Endpoints.OrEmpty()
            .SelectMany(DecoupleEndpoint).OrderBy(x => x).Distinct();

        /// <summary>
        /// Returns whether <paramref name="index"/> aligned with <see cref="NodeCount"/>
        /// Is considered a Depot.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual bool IsNodeAtIndexDepot(long index) => this.DepotCoordinates.Contains(this.IndexToNode(index));

        private readonly RoutingIndexManager _manager;

        /// <summary>
        /// Gets the Manager associated with the Context.
        /// </summary>
        internal virtual RoutingIndexManager Manager => this._manager;

        /// <summary>
        /// Returns the translation of the <see cref="RoutingIndexManager.IndexToNode"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual int IndexToNode(long index) => this.Manager.IndexToNode(index);

        /// <summary>
        /// Returns the translation of the <see cref="RoutingIndexManager.NodeToIndex"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual long NodeToIndex(int node) => this.Manager.NodeToIndex(node);

        /// <summary>
        /// Returns the translation of the <see cref="RoutingIndexManager.NodesToIndices"/>.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public virtual long[] NodesToIndices(params int[] nodes) => this.Manager.NodesToIndices(nodes);

        /// <summary>
        /// Initializes the Context using either <see cref="Depot"/> or
        /// <see cref="Endpoints"/>. Assumes that the contributing factors
        /// have already been Validated prior to invoking this method.
        /// </summary>
        /// <param name="manager">Receives the result after initialization.</param>
        /// <returns></returns>
        private bool TryOnInitialize(out RoutingIndexManager manager)
        {
            var (nodeCount, vehicleCount, depot, eps) = (
                this.NodeCount
                , this.VehicleCount
                , this.Depot
                , this.Endpoints
            );

            bool TryInitializeDepot(out RoutingIndexManager result)
            {
                var actual = depot ?? default;

                result = null;

                if (depot != null)
                {

                    result = new RoutingIndexManager(nodeCount, vehicleCount, actual);
                    this.Endpoints = Enumerable.Range(0, vehicleCount).Select(_ => (actual, actual)).ToArray();
                    return true;
                }

                return result != null;
            }

            bool TryInitializeEndpoints(out RoutingIndexManager result)
            {
                var actual = eps.OrEmpty();

                result = null;

                if (eps != null)
                {
                    var starts = actual.Select(x => x.start).ToArray();
                    var ends = actual.Select(x => x.end).ToArray();
                    result = new RoutingIndexManager(nodeCount, vehicleCount, starts, ends);
                }

                return result != null;
            }

            return TryInitializeDepot(out manager) || TryInitializeEndpoints(out manager);

        }

        /// <summary>
        /// Validates that the <paramref name="depot"/> is correct given
        /// <paramref name="nodeCount"/>.
        /// </summary>
        /// <param name="depot">A Depot being Validated.</param>
        /// <param name="nodeCount">The NodeCount informing Depot Validation.</param>
        /// <returns>The Depot following Validation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when Validation fails.</exception>
        /// <remarks>Some Validation required, otherwise we incur the risk of silent failure.</remarks>
        /// <see cref="Context.RenderOutOfRangeMessage"/>
        private static int ValidateDepot(int depot, int nodeCount)
        {
            if (depot < 0 || depot >= nodeCount)
            {
                throw new ArgumentOutOfRangeException(nameof(depot), RenderOutOfRangeMessage(
                    (nameof(depot), depot)
                    , (nameof(nodeCount), nodeCount)
                ))
                {
                    Data = {
                        { nameof(depot), depot },
                        { nameof(nodeCount), nodeCount }
                    }
                };
            }

            return depot;
        }

        /// <summary>
        /// Validates the Decoupled Endpoint <paramref name="coord"/> at the
        /// <paramref name="index"/>, given <paramref name="nodeCount"/>.
        /// </summary>
        /// <param name="coord">The Coordinate being Validated.</param>
        /// <param name="index">The Index of the Decoupled Coordinate.</param>
        /// <param name="nodeCount">The number of Nodes.</param>
        /// <returns>The Validated <paramref name="coord"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when Validation fails.</exception>
        /// <remarks>Ditto <see cref="ValidateDepot"/> remarks.</remarks>
        /// <see cref="ValidateDepot"/>
        /// <see cref="Context.RenderOutOfRangeMessage"/>
        private static int ValidateDecoupledEndpointCoord(int coord, int index, int nodeCount)
        {
            try
            {
                return ValidateDepot(coord, nodeCount);
            }
            catch (ArgumentOutOfRangeException _)
            {
                throw new ArgumentOutOfRangeException(nameof(coord), RenderOutOfRangeMessage(
                    (nameof(coord), coord)
                    , (nameof(index), index)
                    , (nameof(nodeCount), nodeCount)
                ))
                {
                    Data = {
                        { nameof(coord), coord },
                        { nameof(index), index },
                        { nameof(nodeCount), nodeCount }
                    }
                };
            }
        }

        /// <summary>
        /// Validates the <see cref="Endpoints"/> <paramref name="coords"/> given
        /// <paramref name="vehicleCount"/> and <paramref name="nodeCount"/>.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes.</param>
        /// <param name="vehicleCount">The number of Vehicles.</param>
        /// <param name="coords">The Endpoint Coordinates being Validated.</param>
        /// <returns>The Validated <paramref name="coords"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when Validation fails.</exception>
        /// <remarks>Ditto <see cref="ValidateDepot"/> remarks.</remarks>
        /// <see cref="Context.RenderOutOfRangeMessage"/>
        /// <see cref="ValidateDecoupledEndpointCoord"/>
        private static IEndpointCoordinates ValidateEndpointCoords(int nodeCount, int vehicleCount, IEndpointCoordinates coords)
        {
            coords = coords.OrEmpty().ToArray();
            var coords_Count = coords.Count();

            if (coords_Count != vehicleCount)
            {
                throw new ArgumentOutOfRangeException(nameof(coords), RenderOutOfRangeMessage(
                    (nameof(nodeCount), nodeCount)
                    , (nameof(vehicleCount), vehicleCount)
                    , (nameof(coords), coords)
                ))
                {
                    Data = {
                        { nameof(nodeCount), nodeCount },
                        { nameof(vehicleCount), vehicleCount },
                        { nameof(coords), coords }
                    }
                };
            }

            int OnValidateEndpointCoord(int coord, int index) =>
                ValidateDecoupledEndpointCoord(coord, index, nodeCount);

            return coords.Select(OnValidateEndpointCoord).ToArray();
        }

        /// <summary>
        /// Zips the <paramref name="starts"/> and <paramref name="ends"/> Endpoints Coordinates
        /// given <paramref name="nodeCount"/> and <paramref name="vehicleCount"/>.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes.</param>
        /// <param name="vehicleCount">The number of Vehicles.</param>
        /// <param name="starts">The Endpoint Start Coordinates being zipped.</param>
        /// <param name="ends">The Endpoint End Coordinates being zipped.</param>
        /// <returns>The Zipped Endpoint Coordinates following Validation.</returns>
        /// <remarks>Ditto <see cref="ValidateDepot"/> remarks.</remarks>
        /// <see cref="ValidateEndpointCoords"/>
        /// <see cref="Enumerable.Zip"/>
        private static IEndpoints ZipEndpointCoords(int nodeCount, int vehicleCount
            , IEndpointCoordinates starts, IEndpointCoordinates ends) =>
            ValidateEndpointCoords(nodeCount, vehicleCount, starts).Zip(
                ValidateEndpointCoords(nodeCount, vehicleCount, ends)
                , (start, end) => (start, end)
            ).ToArray();

        /// <summary>
        /// Validates the <paramref name="eps"/> given <paramref name="nodeCount"/>
        /// and <paramref name="vehicleCount"/>.
        /// </summary>
        /// <param name="nodeCount">The numbre of Nodes.</param>
        /// <param name="vehicleCount">The number of Vehicles.</param>
        /// <param name="eps">The Endpoints being Validated.</param>
        /// <returns>The Validated <paramref name="eps"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when Validation fails.</exception>
        /// <remarks>Ditto <see cref="ValidateDepot"/> remarks.</remarks>
        /// <see cref="Context.RenderOutOfRangeMessage"/>
        /// <see cref="ValidateDecoupledEndpointCoord"/>
        private static IEndpoints ValidateEndpoints(int nodeCount, int vehicleCount, IEndpoints eps)
        {
            eps = eps.OrEmpty().ToArray();
            var eps_Count = eps.Count();

            if (eps_Count != vehicleCount)
            {
                throw new ArgumentOutOfRangeException(nameof(eps), RenderOutOfRangeMessage(
                    (nameof(nodeCount), nodeCount)
                    , (nameof(vehicleCount), vehicleCount)
                    , (nameof(eps), eps)
                ))
                {
                    Data = {
                        { nameof(nodeCount), nodeCount },
                        { nameof(vehicleCount), vehicleCount },
                        { nameof(eps), eps }
                    }
                };
            }

            (int start, int end) OnValidateEndpoint((int start, int end) ep, int index)
            {
                var (start, end) = ep;

                return (
                    ValidateDecoupledEndpointCoord(start, index, nodeCount)
                    , ValidateDecoupledEndpointCoord(end, index, nodeCount)
                );
            }

            return eps.Select(OnValidateEndpoint).ToArray();
        }

        /// <summary>
        /// Constructs the Context given Node and Vehicle Counts, assuming Zero Depot.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        protected ManagerContext(int nodeCount, int vehicleCount)
            : this(nodeCount, vehicleCount, default(int))
        {
        }

        /// <summary>
        /// Constructs the Context given Node and Vehicle Counts as well as Depot.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="depot">The Depot involved in the model. By default is <c>0</c>,
        /// but it can be anything, as long as it aligns within the <em>zero based number
        /// of nodes</em>.</param>
        protected ManagerContext(int nodeCount, int vehicleCount, int depot)
        {
            this.NodeCount = nodeCount;
            this.VehicleCount = vehicleCount;
            this.Depot = ValidateDepot(depot, nodeCount);
            this.TryOnInitialize(out this._manager);
        }

        /// <summary>
        /// Constructs the Context given Node and Vehicle Counts,
        /// as well as endpoint start and end coordinates.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="starts"></param>
        /// <param name="ends"></param>
        protected ManagerContext(int nodeCount, int vehicleCount, IEndpointCoordinates starts, IEndpointCoordinates ends)
        {
            this.NodeCount = nodeCount;
            this.VehicleCount = vehicleCount;
            // Since we are Validating here, do not incur the cost a second time during another ctor.
            this.Endpoints = ZipEndpointCoords(nodeCount, vehicleCount, starts, ends);
            this.TryOnInitialize(out this._manager);
        }

        /// <summary>
        /// Constructs the Context given Node and Vehicle Counts as well as endpoints.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="eps">The Endpoints involved during the Context.</param>
        protected ManagerContext(int nodeCount, int vehicleCount, IEndpoints eps)
        {
            this.NodeCount = nodeCount;
            this.VehicleCount = vehicleCount;
            // We Validate these Endpoints separately so as not to incur the cost for doing so a second time.
            this.Endpoints = ValidateEndpoints(nodeCount, vehicleCount, eps);
            this.TryOnInitialize(out this._manager);
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this.Manager?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
