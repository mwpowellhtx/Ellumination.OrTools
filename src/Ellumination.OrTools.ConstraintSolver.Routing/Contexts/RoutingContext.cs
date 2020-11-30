using System.Collections.Generic;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
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
    /// <remarks>In theory we think that subscribers can potentially run on just
    /// <see cref="RoutingContext"/> as their modeling foundation. However, we also
    /// allow for a more <see cref="DomainContext{TNode, TDepot, TVehicle}"/> specific
    /// comprehension.</remarks>
    public class RoutingContext : ManagerContext
    {
        private RoutingModel _model;

        /// <summary>
        /// Gets the Model associated with the Context.
        /// </summary>
        public virtual RoutingModel Model => this._model ?? (this._model = this.ModelParameters == null
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

        /// <summary>
        /// Constructs the Context given Node and Vehicle Counts.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        public RoutingContext(int nodeCount, int vehicleCount)
            : base(nodeCount, vehicleCount)
        {
        }

        // TODO: TBD: so may rethink the whole "edges" equation after all...
        // TODO: TBD: for sure there needs to be better validation and exception handling involved.
        // TODO: TBD: at the very least not to let invalid combinations fall through where the whole thing fails without explanation.
        // TODO: TBD: but moreover for so-called "edges" or rather node versus depot establishment to be a consumer decision more than anything else.
        // TODO: TBD: needs to be depot-aware, in the sense that we can possibly establish index evaluation based on depot versus node.
        /// <summary>
        /// Constructs the Context given Node and Vehicle Counts, as well as Depot.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="depot">The Depot involved in the model. By default is <c>0</c>,
        /// but it can be anything, as long as it aligns within the <em>zero based number
        /// of nodes</em>.</param>
        public RoutingContext(int nodeCount, int vehicleCount, int depot)
            : base(nodeCount, vehicleCount, depot)
        {
        }

        /// <summary>
        /// Constructs the Context given Node and Vehicle Counts, as well as Depot
        /// and optional <paramref name="modelParameters"/>.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="depot"></param>
        /// <param name="modelParameters"></param>
        public RoutingContext(int nodeCount, int vehicleCount, int depot, RoutingModelParameters modelParameters = default)
            : base(nodeCount, vehicleCount, depot)
        {
            this.ModelParameters = modelParameters;
        }

        /// <summary>
        /// Constructs the Context given Node and Vehicle Counts, as well as
        /// endpoint start and end coordinates.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="starts"></param>
        /// <param name="ends"></param>
        public RoutingContext(int nodeCount, int vehicleCount, IEndpointCoordinates starts, IEndpointCoordinates ends)
            : base(nodeCount, vehicleCount, starts, ends)
        {
        }

        /// <summary>
        /// Constructs the Context given Node and Vehicle Counts, as well as endpoint
        /// start and end coordinates, and optional <paramref name="modelParameters"/>.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="starts"></param>
        /// <param name="ends"></param>
        /// <param name="modelParameters">The Model parameters.</param>
        public RoutingContext(int nodeCount, int vehicleCount, IEndpointCoordinates starts, IEndpointCoordinates ends
            , RoutingModelParameters modelParameters = default)
            : base(nodeCount, vehicleCount, starts, ends)
        {
            this.ModelParameters = modelParameters;
        }

        /// <summary>
        /// Constructs the Context given Node and Vehicle Counts, endpoints, and optkional
        /// <paramref name="modelParameters"/>.
        /// </summary>
        /// <param name="nodeCount">The number of Nodes in the model.</param>
        /// <param name="vehicleCount">The number of Vehicles in the model.</param>
        /// <param name="eps">The Endpoints involved during the Context.</param>
        /// <param name="modelParameters">The Model parameters.</param>
        public RoutingContext(int nodeCount, int vehicleCount, IEndpoints eps
            , RoutingModelParameters modelParameters = default)
            : base(nodeCount, vehicleCount, eps)
        {
            this.ModelParameters = modelParameters;
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this.Model?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
