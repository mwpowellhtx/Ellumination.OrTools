using System.Collections.Generic;
using System.Linq;

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
        internal virtual Solver Solver => this.Model?.solver();

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
        /// Gets an instance of <see cref="RoutingModelVehicleVariableLookup"/> for Private use.
        /// </summary>
        /// <see cref="VehicleVarLookup"/>
        private RoutingModelVehicleVariableLookup PrivateVehicleVarLookup => this.Model;

        /// <summary>
        /// Gets an instance of <see cref="IRoutingModelVehicleVariableLookup"/> for Protected use.
        /// </summary>
        /// <see cref="PrivateVehicleVarLookup"/>
        protected virtual IRoutingModelVehicleVariableLookup VehicleVarLookup => this.PrivateVehicleVarLookup;

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
        /// Returns the cost of the transit arc between two nodes for a given
        /// <paramref name="vehicle"/>. Input are variable indices of <paramref name="node"/>.
        /// This returns <c>0</c> when <paramref name="vehicle"/> &lt; <c>0</c>.
        /// </summary>
        /// <param name="previousNode"></param>
        /// <param name="node"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public virtual long GetArcCostForVehicle(int previousNode, int node, int vehicle) => this.Model.GetArcCostForVehicle(
            this.NodeToIndex(previousNode)
            , this.NodeToIndex(node)
            , vehicle
        );

        /// <summary>
        /// Adds <paramref name="constraints"/> to the <see cref="Model"/>.
        /// </summary>
        /// <param name="constraints"></param>
        /// <remarks>These methods are intentionally Internal because subscribers
        /// are expected to add Constraints via <see cref="Dimension"/>.</remarks>
        internal void AddConstraints(IEnumerable<Constraint> constraints)
        {
            void OnAddConstraints(Solver solver) => constraints.OrEmpty().ToList().ForEach(solver.Add);

            OnAddConstraints(this.Solver);
        }

        /// <summary>
        /// Adds <paramref name="constraints"/> to the <see cref="Model"/>.
        /// </summary>
        /// <param name="constraints"></param>
        /// <remarks>These methods are intentionally Internal because subscribers
        /// are expected to add Constraints via <see cref="Dimension"/>.</remarks>
        internal void AddConstraints(params Constraint[] constraints)
        {
            void OnAddConstraints(Solver solver) => constraints.ToList().ForEach(solver.Add);

            OnAddConstraints(this.Solver);
        }

        /// <summary>
        /// Adds <paramref name="constraint"/> to the <see cref="Model"/>.
        /// </summary>
        /// <param name="constraint"></param>
        /// <remarks>These methods are intentionally Internal because subscribers
        /// are expected to add Constraints via <see cref="Dimension"/>.</remarks>
        internal void AddConstraint(Constraint constraint) => this.AddConstraints(constraint);

        /// <summary>
        /// Adds a Pickup and Delivery requirement to the <see cref="Model"/>.
        /// </summary>
        /// <param name="dim">The Dimension informing the Pickup and Delivery pair.</param>
        /// <param name="solver">The Solver for use with the Pickup and Delivery pair.</param>
        /// <param name="pickupNode">The domain based Pickup Node.</param>
        /// <param name="deliveryNode">The domain based Delivery Node.</param>
        /// <remarks>This is protectedly accessible because we cannot know at an architectural
        /// scaffold level which Dimension instances may want to make the invocation. So we
        /// expose it for protected use.</remarks>
        protected virtual void OnAddPickupAndDelivery(Dimension dim, Solver solver
            , int pickupNode, int deliveryNode)
        {
            var (pickupIndex, deliveryIndex) = (
                this.NodeToIndex(pickupNode)
                , this.NodeToIndex(deliveryNode)
            );

            // TODO: TBD: should validate we have a dimension, mutabledimension, etc...
            this.Model.AddPickupAndDelivery(pickupIndex, deliveryIndex);

            IEnumerable<Constraint> GetPickupAndDeliveryConstraints(
                IRoutingModelVehicleVariableLookup vehicleVarLookup
                , IRoutingDimensionAccumulatorLookup accumulatorLookup)
            {
                // Requires that each item picked up / delivered by same vehicle.
                yield return vehicleVarLookup[pickupIndex] == vehicleVarLookup[deliveryIndex];

                /* Requires that each item must be picked up before it is delivered. In other
                 * words, vehicle cumulative distance at an item pickup location is at most its
                 * cumulative distance at the delivery location. */

                yield return accumulatorLookup[pickupIndex] <= accumulatorLookup[deliveryIndex];

                /* Additionally, from an architectural scaffold perspective, it is super easy to
                 * construct these boolean style DSL phrases. SUPER easy. What is more important
                 * to us is to facilitate access to those variables in as transparent, easy to
                 * use, manner as possible. */
            }

            // Add each of the resolved Constraints to the Model Solver.
            GetPickupAndDeliveryConstraints(this.VehicleVarLookup, dim.AccumulatorLookup)
                .ToList().ForEach(solver.Add);
        }

        /// <summary>
        /// Adds a Pickup and Delivery requirement to the <see cref="Model"/>.
        /// </summary>
        /// <param name="dim">The Dimension informing the Pickup and Delivery pair.</param>
        /// <param name="pickupNode">The domain based Pickup Node.</param>
        /// <param name="deliveryNode">The domain based Delivery Node.</param>
        /// <remarks>This is publicly accessible because we cannot know at an architectural
        /// scaffold level which Dimension instances may want to make the invocation. So we
        /// expose it for public use.</remarks>
        public virtual void AddPickupAndDelivery(Dimension dim, int pickupNode, int deliveryNode) =>
            this.OnAddPickupAndDelivery(dim, this.Solver, pickupNode, deliveryNode);

        /// <summary>
        /// Adds the Pickup and Delivery <paramref name="nodes"/> pairs corresponding
        /// to the <paramref name="dim"/>.
        /// </summary>
        /// <param name="dim">The Dimension corresponding to the <paramref name="nodes"/> pairs.</param>
        /// <param name="nodes">The Pickup and Delivery pair nodes.</param>
        public virtual void AddPickupsAndDeliveries(Dimension dim, params (int pickup, int delivery)[] nodes)
        {
            // For optimum use throughout the span of Nodes.
            var this_Solver = this.Solver;

            void OnAddPickupAndDelivery((int pickup, int delivery) node) =>
                this.OnAddPickupAndDelivery(dim, this_Solver, node.pickup, node.delivery);

            nodes.ToList().ForEach(OnAddPickupAndDelivery);
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
