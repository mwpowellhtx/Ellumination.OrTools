using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Google.OrTools.ConstraintSolver;
    using static IncludeEdge;

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
    public abstract class Context : IDisposable
    {
        /// <summary>
        /// Gets the Edge whether <see cref="IncludeStart"/> or <see cref="IncludeEnd"/> edges
        /// are included in the modeled <see cref="NodeCount"/>.
        /// </summary>
        public IncludeEdge Edge { get; }

        /// <summary>
        /// 
        /// </summary>
        public int StartEdge => this.Edge.Summarize(IncludeStart);

        /// <summary>
        /// 
        /// </summary>
        public int EndEdge => this.Edge.Summarize(IncludeEnd);

        /// <summary>
        /// 
        /// </summary>
        public int EdgeCount => this.StartEdge + this.EndEdge;

        /// <summary>
        /// Gets the NodeCount. May not reflect the actual count of
        /// <see cref="Context{TNode, TVehicle}.Nodes"/>, as a function of the
        /// <see cref="Edge"/>, <see cref="StartEdge"/> and <see cref="EndEdge"/>.
        /// </summary>
        /// <see cref="Edge"/>
        /// <see cref="StartEdge"/>
        /// <see cref="EndEdge"/>
        /// <see cref="IncludeStart"/>
        /// <see cref="IncludeEnd"/>
        /// <see cref="IncludeEdge"/>
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
        public virtual IEnumerable<(int start, int end)> Endpoints { get; protected set; }

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
        /// Gets or Sets the Parameters associated with the Context. May be Null, in which
        /// case nothing is relayed to the <see cref="Model"/>, or Set up until the moment
        /// when <see cref="Model"/> is warmed up, at which point the opportunity will have
        /// been lost to have set or configured the Parameters accordingly.
        /// </summary>
        /// <see cref="Model"/>
        private RoutingModelParameters ModelParameters { get; }

        /// <summary>
        /// Initializes the Context.
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryOnInitialize()
        {
            bool TryInitializeDepot(int nodeCount, int vehicleCount, int? depot)
            {
                var tried = depot != null;
                var actual = depot ?? default;

                if (tried)
                {
                    this.Manager = new RoutingIndexManager(nodeCount, vehicleCount, actual);
                    this.Endpoints = Enumerable.Range(0, vehicleCount).Select(_ => (actual, actual)).ToArray();
                }

                return tried;
            }

            bool TryInitializeEndpoints(int nodeCount, int vehicleCount, IEnumerable<(int start, int end)> endpoints)
            {
                var tried = endpoints != null;
                var actual = endpoints.OrEmpty();

                if (tried)
                {
                    var starts = actual.Select(x => x.start).ToArray();
                    var ends = actual.Select(x => x.end).ToArray();
                    this.Manager = new RoutingIndexManager(nodeCount, vehicleCount, starts, ends);
                }

                return tried;
            }

            return TryInitializeDepot(this.NodeCount, this.VehicleCount, this.Depot)
                || TryInitializeEndpoints(this.NodeCount, this.VehicleCount, this.Endpoints);
        }

        protected Context(IncludeEdge edge = default, RoutingModelParameters modelParameters = null)
            : this(default, edge, null)
        {
        }

        protected Context(int depot, IncludeEdge edge = default, RoutingModelParameters modelParameters = default)
        {
        }

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="vehicleCount"></param>
        /// <param name="nodeCount"></param>
        /// <param name="depot"></param>
        /// <param name="edge"></param>
        protected Context(int nodeCount, int vehicleCount, int depot = default, IncludeEdge edge = default)
            : this(nodeCount, vehicleCount, depot, edge, null)
        {
        }

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="vehicleCount"></param>
        /// <param name="nodeCount"></param>
        /// <param name="depot"></param>
        /// <param name="edge"></param>
        /// <param name="modelParameters"></param>
        protected Context(int nodeCount, int vehicleCount, int depot = default, IncludeEdge edge = default, RoutingModelParameters modelParameters = default)
        {
            this.NodeCount = nodeCount;
            this.VehicleCount = vehicleCount;
            // TODO: TBD: verify that depot indexes are correct...
            this.Depot = depot;
            this.ModelParameters = modelParameters;
            this.TryOnInitialize();
        }

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="vehicleCount"></param>
        /// <param name="nodeCount"></param>
        /// <param name="starts"></param>
        /// <param name="ends"></param>
        /// <param name="edge"></param>
        protected Context(int nodeCount, int vehicleCount, IEnumerable<int> starts, IEnumerable<int> ends, IncludeEdge edge = default)
            : this(nodeCount, vehicleCount, starts, ends, edge, null)
        {
        }

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="vehicleCount"></param>
        /// <param name="nodeCount"></param>
        /// <param name="starts"></param>
        /// <param name="ends"></param>
        /// <param name="edge"></param>
        /// <param name="modelParameters"></param>
        protected Context(int nodeCount, int vehicleCount, IEnumerable<int> starts, IEnumerable<int> ends, IncludeEdge edge = default, RoutingModelParameters modelParameters)
        {
            this.NodeCount = nodeCount;
            this.VehicleCount = vehicleCount;
            this.Edge = edge;
            // TODO: TBD: verify that depot indexes are correct...
            this.Endpoints = starts.Zip(ends, (start, end) => (start, end)).ToArray();
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
