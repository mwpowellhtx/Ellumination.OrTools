using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Distances;
    using Google.OrTools.ConstraintSolver;
    // These make sense as aliases, but should not make them first class derivations.
    using IEndpoints = IEnumerable<(int start, int end)>;
    // This is because we want to keep these close to the generic IEnumerable.
    using IEndpointCoordinates = IEnumerable<int>;

    // TODO: TBD: then from here, context invokes model ability to search...
    // TODO: TBD: with or without search params...
    // TODO: TBD: and I should think also delegation of the response...
    /// <summary>
    /// Represents the basic building blocks of a <see cref="RoutingModel"/> Context. While
    /// changes to the moving parts, Nodes or Vehicles, as the case may be, depending on the
    /// domain model, is possible, we advise against it in a given Context. If such mutations
    /// are desired, then create a new Context instance with those moving parts having been
    /// mutated.
    /// </summary>
    public class Context : IDisposable
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
        public virtual bool IsNodeAtIndexDepot(int index)
        {
            if (index < 0 || index >= this.NodeCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index)
                    , $"{nameof(index)} {index} out of range"
                    + $" with {nameof(this.NodeCount)} count {this.NodeCount}"
                );
            }

            return true;
        }

        private RoutingModel _model;

        /// <summary>
        /// Gets the Manager associated with the Context.
        /// </summary>
        public RoutingIndexManager Manager { get; protected set; }

        /// <summary>
        /// Gets the Model associated with the Context.
        /// </summary>
        public virtual RoutingModel Model => this._model ?? (this._model = ModelParameters == null
            ? new RoutingModel(this.Manager)
            : new RoutingModel(this.Manager, this.ModelParameters)
        );

        /// <summary>
        /// Gets the Solver associated with the <see cref="Model"/>.
        /// </summary>
        protected virtual Solver Solver => this.Model?.solver();

        /// <summary>
        /// Gets or Sets the Parameters associated with the Context. May be <c>null</c>,
        /// in which case nothing is relayed to the <see cref="Model"/>, or Set up until
        /// the moment when <see cref="Model"/> is warmed up, at which point the
        /// opportunity will have been lost to have set or configured the Parameters
        /// accordingly.
        /// </summary>
        /// <see cref="Model"/>
        private RoutingModelParameters ModelParameters { get; }

        /// <summary>
        /// Gets the SearchParameters.
        /// </summary>
        protected RoutingSearchParameters SearchParameters { get; private set; }

        //// TODO: TBD: getting to search params...
        //// TODO: TBD: will need to consider how the closure to run the search should operate...
        //// TODO: TBD: with or without params...
        //// TODO: TBD: then, I think, also including the closure with solution, nodes, vehicles, etc...
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public Context OnSearchParameters()

        /// <summary>
        /// Gets or Sets the DistancesMatrix.
        /// </summary>
        /// <remarks><see cref="Context"/> must allow for an expression of the
        /// <see cref="DistanceMatrix"/>. However, we think that <see cref="Context"/>
        /// lacks sufficient context, all punning aside, in order to determine
        /// appropriate serialization, merging, updates, with the matrix that
        /// informs that <see cref="Context"/>.</remarks>
        public virtual DistanceMatrix Distances { get; set; }

        /// <summary>
        /// Initializes the Context using either <see cref="Depot"/> or
        /// <see cref="Endpoints"/>. Assumes that the contributing factors
        /// have already been Validated prior to invoking this method.
        /// </summary>
        /// <returns></returns>
        private bool TryOnInitialize()
        {
            var (nodeCount, vehicleCount, depot, eps) = (
                this.NodeCount
                , this.VehicleCount
                , this.Depot
                , this.Endpoints
            );

            bool TryInitializeDepot()
            {
                var actual = depot ?? default;

                if (depot != null)
                {
                    this.Manager = new RoutingIndexManager(nodeCount, vehicleCount, actual);
                    this.Endpoints = Enumerable.Range(0, vehicleCount).Select(_ => (actual, actual)).ToArray();
                    return true;
                }

                return false;
            }

            bool TryInitializeEndpoints()
            {
                var actual = eps.OrEmpty();

                if (eps != null)
                {
                    var starts = actual.Select(x => x.start).ToArray();
                    var ends = actual.Select(x => x.end).ToArray();
                    this.Manager = new RoutingIndexManager(nodeCount, vehicleCount, starts, ends);
                    return true;
                }

                return false;
            }

            return TryInitializeDepot() || TryInitializeEndpoints();
        }

        /// <summary>
        /// Renders a <see cref="string"/> Message informing a Validation
        /// <see cref="Exception"/>.
        /// </summary>
        /// <param name="pairs">The Pairs being combined into a message.</param>
        /// <returns></returns>
        /// <see cref="string.Join"/>
        protected static string RenderOutOfRangeMessage(params (string key, object value)[] pairs)
        {
            string RenderPair((string key, object value) pair) => $"{pair.key}: {pair.value}";
            return $"{{ {string.Join(", ", pairs.Select(RenderPair))} }} out of range";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enclosure"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected static string RenderArray<T>(string enclosure, params T[] values)
        {
            string RenderValue(T value) => $"{value}";
            return string.Join(string.Join(", ", values.Select(RenderValue)), enclosure.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        protected static IEnumerable<T> Range<T>(params T[] values)
        {
            foreach (var value in values)
            {
                yield return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        protected static string RenderArray<T>(params T[] values) => RenderArray("[]", values);

        /// <summary>
        /// Validates that the <paramref name="depot"/> is correct given
        /// <paramref name="nodeCount"/>.
        /// </summary>
        /// <param name="depot">A Depot being Validated.</param>
        /// <param name="nodeCount">The NodeCount informing Depot Validation.</param>
        /// <returns>The Depot following Validation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when Validation fails.</exception>
        /// <remarks>Some Validation required, otherwise we incur the risk of silent failure.</remarks>
        /// <see cref="RenderOutOfRangeMessage"/>
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
        /// <see cref="RenderOutOfRangeMessage"/>
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
        /// <see cref="RenderOutOfRangeMessage"/>
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
        /// <see cref="RenderOutOfRangeMessage"/>
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
        /// Gets the Dimensions for Internal use.
        /// </summary>
        /// <remarks>It is a tiny bit verbose, we do admit, yet for type reasons
        /// we must shoe in Dimensions in this manner.</remarks>
        internal virtual ICollection<IDimension> InternalDimensions { get; } = new List<IDimension>();

        /// <summary>
        /// Gets the Dimensions.
        /// </summary>
        public virtual IEnumerable<IDimension> Dimensions => this.InternalDimensions;

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        public Context(int nodeCount, int vehicleCount)
            : this(nodeCount, vehicleCount, default)
        {
        }

        // TODO: TBD: so may rethink the whole "edges" equation after all...
        // TODO: TBD: for sure there needs to be better validation and exception handling involved.
        // TODO: TBD: at the very least not to let invalid combinations fall through where the whole thing fails without explanation.
        // TODO: TBD: but moreover for so-called "edges" or rather node versus depot establishment to be a consumer decision more than anything else.
        // TODO: TBD: needs to be depot-aware, in the sense that we can possibly establish index evaluation based on depot versus node.
        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="depot">The Depot involved in the model. By default is <c>0</c>,
        /// but it can be anything, as long as it aligns within the <em>zero based number
        /// of nodes</em>.</param>
        public Context(int nodeCount, int vehicleCount, int depot)
            : this(nodeCount, vehicleCount, depot, default)
        {
        }

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="depot"></param>
        /// <param name="modelParameters"></param>
        public Context(int nodeCount, int vehicleCount, int depot, RoutingModelParameters modelParameters = default)
        {
            this.NodeCount = nodeCount;
            this.VehicleCount = vehicleCount;
            // TODO: TBD: verify that depot indexes are correct...
            this.Depot = ValidateDepot(depot, nodeCount);
            this.ModelParameters = modelParameters;
            this.TryOnInitialize();
        }

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="starts"></param>
        /// <param name="ends"></param>
        public Context(int nodeCount, int vehicleCount, IEndpointCoordinates starts, IEndpointCoordinates ends)
            : this(nodeCount, vehicleCount, starts, ends, default)
        {
        }

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="starts"></param>
        /// <param name="ends"></param>
        /// <param name="modelParameters">The Model parameters.</param>
        public Context(int nodeCount, int vehicleCount, IEndpointCoordinates starts, IEndpointCoordinates ends
            , RoutingModelParameters modelParameters = default)
        {
            this.NodeCount = nodeCount;
            this.VehicleCount = vehicleCount;
            // Since we are Validating here, do not incur the cost a second time during another ctor.
            this.Endpoints = ZipEndpointCoords(nodeCount, vehicleCount, starts, ends);
            this.ModelParameters = modelParameters;
            this.TryOnInitialize();
        }

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="eps">The Endpoints involved during the Context.</param>
        /// <param name="modelParameters">The Model parameters.</param>
        public Context(int nodeCount, int vehicleCount, IEndpoints eps, RoutingModelParameters modelParameters = default)
        {
            this.NodeCount = nodeCount;
            this.VehicleCount = vehicleCount;
            // We Validate these Endpoints separately so as not to incur the cost for doing so a second time.
            this.Endpoints = ValidateEndpoints(nodeCount, vehicleCount, eps);
            this.ModelParameters = modelParameters;
            this.TryOnInitialize();
        }

        /// <summary>
        /// Gets whether the object IsDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                // TODO: TBD: determine whether Model **AND** Manager both need to be disposed...
                this.Manager?.Dispose();
                this.Model?.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            this.IsDisposed = true;
        }
    }
}
