using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Google.OrTools.ConstraintSolver;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Distances;
    using RoutingTransitEvaluationCallback = LongLongToLong;
    using RoutingUnaryTransitEvaluationCallback = LongToLong;
    using OnRegisterRoutingTransitEvalution = Func<LongLongToLong, int>;
    using TranslateRoutingContextIndexCallback = Func<RoutingContext, int, long>;

    /// <summary>
    /// Dimension is the adapter layer handling all things
    /// <see cref="RoutingUnaryTransitEvaluationCallback"/> and
    /// <see cref="RoutingTransitEvaluationCallback"/> vis-a-vis the
    /// <see cref="RoutingIndexManager"/> and <see cref="RoutingModel"/>. Above all, the
    /// value add here is that <see cref="Dimension"/> does the heavy lifting of interfacing
    /// the underlying <see cref="RoutingContext.Model"/> with the rest of your routing model,
    /// so that you can focus on lines of business and leave that lifting to an adapter layer.
    /// </summary>
    public abstract class Dimension : IDimension<RoutingContext>
    {
        /// <summary>
        /// Gets the Id associated with the Dimension.
        /// </summary>
        protected virtual Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the Context associated with the Dimension.
        /// </summary>
        protected virtual RoutingContext Context { get; }

        /// <summary>
        /// <see cref="RoutingUnaryTransitEvaluationCallback"/> adapter delegation. Google
        /// OrTools uses this delegate in order to facilitate evaluation, however this is
        /// so poorly documented, so we add this intermediate layer in an effort to better
        /// document.
        /// </summary>
        /// <param name="index">An Index that is in terms of the
        /// <see cref="RoutingIndexManager"/>. Use the method
        /// <see cref="RoutingIndexManager.IndexToNode"/> in order to translate the
        /// Routing Index to <see cref="ManagerContext.NodeCount"/> friendly one.</param>
        /// <returns></returns>
        protected delegate long UnaryTransitEvaluationCallback(long index);

        /// <summary>
        /// Ditto <see cref="UnaryTransitEvaluationCallback"/>, except evaluating in terms
        /// of the transit <see cref="RoutingTransitEvaluationCallback"/> delegate
        /// </summary>
        /// <param name="fromIndex">Ditto Unary Index.</param>
        /// <param name="toIndex">Ditto Unary Index.</param>
        /// <returns></returns>
        protected delegate long TransitEvaluationCallback(long fromIndex, long toIndex);

        private readonly ICollection<int> _registeredCallbacks = new List<int>();

        /// <summary>
        /// Handles registering a couple of default evaluations with
        /// the underlying <see cref="RoutingContext.Model"/> vis-a-vis
        /// the <paramref name="onRegister"/> adapter.
        /// </summary>
        /// <typeparam name="TCallback"></typeparam>
        /// <param name="i"></param>
        /// <param name="callback"></param>
        /// <param name="onRegister"></param>
        /// <returns></returns>
        private int OnRegisterTransitEvaluationCallback<TCallback>(int i, TCallback callback, Func<TCallback, int> onRegister)
        {
            if (this._registeredCallbacks.Count < i + 1)
            {
                var index = onRegister.Invoke(callback);
                this._registeredCallbacks.Add(index);
            }

            return this._registeredCallbacks.ElementAt(i);
        }

        /// <summary>
        /// Handles registering the <paramref name="callback"/> vis-a-vis the given
        /// <paramref name="onRegister"/> invocation.
        /// </summary>
        /// <typeparam name="TCallback"></typeparam>
        /// <param name="callback"></param>
        /// <param name="onRegister"></param>
        /// <returns></returns>
        private int OnRegisterTransitEvaluationCallback<TCallback>(TCallback callback, Func<TCallback, int> onRegister)
            where TCallback : Delegate
        {
            var index = onRegister.Invoke(callback);
            TryValidateCallbackIndex(index);
            this._registeredCallbacks.Add(index);
            return index;
        }

        /// <summary>
        /// Registers the <see cref="TransitEvaluationCallback"/>
        /// <paramref name="callback"/> in either <paramref name="positive"/>
        /// or not modes. We adapt the <paramref name="callback"/> in terms of
        /// <see cref="RoutingTransitEvaluationCallback"/> in the scope of this
        /// method for use with the underlying <see cref="RoutingContext.Model"/>.
        /// Returns the index of the callback following registration.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="positive"></param>
        /// <returns></returns>
        protected virtual int RegisterTransitEvaluationCallback(TransitEvaluationCallback callback, bool positive = false)
        {
            var this_Context_Model = this.Context.Model;

            int OnRegisterCallback(RoutingTransitEvaluationCallback adaptedCb) => positive
                ? this_Context_Model.RegisterPositiveTransitCallback(adaptedCb)
                : this_Context_Model.RegisterTransitCallback(adaptedCb)
                ;

            return this.OnRegisterTransitEvaluationCallback<RoutingTransitEvaluationCallback>(
                (fromIndex, toIndex) => callback.Invoke(fromIndex, toIndex)
                , OnRegisterCallback);
        }

        /// <summary>
        /// Registers the <see cref="UnaryTransitEvaluationCallback"/>
        /// <paramref name="callback"/> in either <paramref name="positive"/>
        /// or not modes. We adapt the <paramref name="callback"/> in terms of
        /// <see cref="RoutingUnaryTransitEvaluationCallback"/> in the scope of this
        /// method for use with the underlying <see cref="RoutingContext.Model"/>.
        /// Returns the index of the callback following registration.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="positive"></param>
        /// <returns></returns>
        protected virtual int RegisterUnaryTransitEvaluationCallback(UnaryTransitEvaluationCallback callback, bool positive = false)
        {
            var this_Context_Model = this.Context.Model;

            int OnRegisterCallback(RoutingUnaryTransitEvaluationCallback adaptedCb) => positive
                ? this_Context_Model.RegisterPositiveUnaryTransitCallback(adaptedCb)
                : this_Context_Model.RegisterUnaryTransitCallback(adaptedCb)
                ;

            return this.OnRegisterTransitEvaluationCallback<RoutingUnaryTransitEvaluationCallback>(
                (index) => callback.Invoke(index)
                , OnRegisterCallback);
        }

        /// <summary>
        /// Validates the Callback <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected static bool TryValidateCallbackIndex(int index) => index >= 0
            ? true
            : throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} {index} out of range");

        /// <summary>
        /// Gets the Default Binary and Unary Callbacks.
        /// </summary>
        protected virtual Callbacks DefaultCallbacks { get; }

        // TODO: TBD: positive versus not positive...
        /// <summary>
        /// Gets the Registered index of the <see cref="Callbacks.ZeroUnaryTransitEvaluation"/>.
        /// </summary>
        protected int ZeroUnaryTransitEvaluationIndex
        {
            get
            {
                var index = this.OnRegisterTransitEvaluationCallback(0
                    , this.DefaultCallbacks.ZeroUnaryTransitEvaluation
                    , this.Context.Model.RegisterUnaryTransitCallback);

                TryValidateCallbackIndex(index);

                return index;
            }
        }

        /// <summary>
        /// Gets the Registered index of the <see cref="Callbacks.ZeroTransitEvaluation"/>.
        /// This index depends on <see cref="ZeroUnaryTransitEvaluationIndex"/> having been
        /// resolved before hand.
        /// </summary>
        /// <see cref="Callbacks.ZeroTransitEvaluation"/>
        /// <see cref="ZeroUnaryTransitEvaluationIndex"/>
        protected int ZeroTransitEvaluationIndex
        {
            get
            {
                // Must be initialized first.
                TryValidateCallbackIndex(this.ZeroUnaryTransitEvaluationIndex);

                return this.OnRegisterTransitEvaluationCallback(1
                    , this.DefaultCallbacks.ZeroTransitEvaluation
                    , this.Context.Model.RegisterTransitCallback);
            }
        }

        /// <summary>
        /// Gets the RegisteredCallbacks, excepting for the first two, which are reserved for
        /// <see cref="ZeroUnaryTransitEvaluationIndex"/> and
        /// <see cref="ZeroTransitEvaluationIndex"/>, respectively. Every other one, however,
        /// we potentially need to know about for subsequent usage adding dimensions to the
        /// underlying <see cref="RoutingContext.Model"/>.
        /// </summary>
        /// <see cref="ZeroTransitEvaluationIndex"/>
        /// <see cref="ZeroUnaryTransitEvaluationIndex"/>
        protected IEnumerable<int> RegisteredCallbacks
        {
            get
            {
                // Must be initialized first.
                TryValidateCallbackIndex(this.ZeroTransitEvaluationIndex);
                return this._registeredCallbacks.Skip(2);
            }
        }

        private string _name;

        /// <summary>
        /// Gets the Name of the Dimension especially for use with
        /// <see cref="RoutingContext.Model"/>. Override in order to specialize the Name
        /// in accordance with your Dimension. Defaults to <see cref="Type.FullName"/>
        /// via <see cref="object.GetType"/>.
        /// </summary>
        public virtual string Name => this._name ?? (this._name = this.GetType().FullName);

        /// <summary>
        /// Gets whether the underlying Routing Model Has the Registered Dimension.
        /// </summary>
        public virtual bool IsAdded => this.Context.Model.HasDimension(this.Name);

        // TODO: TBD: is there a safer manner in which to identify whether the dimension has been added to the model?
        /// <summary>
        /// Gets the <see cref="RoutingDimension"/> corresponding to the
        /// <see cref="RoutingContext.Model"/> and <see cref="Name"/>,
        /// or Die in the process.
        /// </summary>
        protected virtual RoutingDimension RoutingDimensionOrDie => this.Context.Model.GetDimensionOrDie(this.Name);

        /// <summary>
        /// Gets the <see cref="RoutingModel.GetMutableDimension"/> <see cref="RoutingDimension"/>
        /// corresponding with the Registered <see cref="Name"/>.
        /// </summary>
        public virtual RoutingDimension MutableDimension => this.Context.Model.GetMutableDimension(this.Name);

        /// <summary>
        /// Gets an instance of <see cref="RoutingDimensionAccumulatorLookup"/> for Private use.
        /// </summary>
        /// <see cref="AccumulatorLookup"/>
        private RoutingDimensionAccumulatorLookup PrivateAccumulatorLookup => this.MutableDimension;

        /// <summary>
        /// Gets the AccumulatorLookup corresponding to the Dimension.
        /// </summary>
        /// <see cref="PrivateAccumulatorLookup"/>
        public virtual IRoutingDimensionAccumulatorLookup AccumulatorLookup => this.PrivateAccumulatorLookup;

        // TODO: TBD: we think that perhaps future versions could support distance matrix as part of the equation...
        // TODO: TBD: then, whether these bits are pre-calculated in some way... is a decision for later...
        // TODO: TBD: for now, the callback is the callback, the moment when such calculation should occur.

        // TODO: TBD: we think that possible distance matrices are such an inherent part of dimension calculation...
        // TODO: TBD: or at least allowing for that possibility, i.e. via some override through an intermediate class...

        ///// <summary>
        ///// Gets the Distances for use throughout the Dimension. Dimension may have one that
        ///// is aligned with itself. Or they may be aligned with other bits, the vehicles, or
        ///// nodes, etc, themselves, as the case may be, on a case by case Dimension basis.
        ///// </summary>
        //protected virtual IDictionary<Guid, DistanceMatrix> Distances { get; }

        ///// <summary>
        ///// Override in order to return an appropriate <see cref="DistanceMatrix"/> instnace.
        ///// </summary>
        ///// <param name="context"></param>
        ///// <returns></returns>
        //protected virtual DistanceMatrix CreateMatrix(Context context) => new DistanceMatrix(context.NodeCount);

        ///// <summary>
        ///// Returns the <see cref="DistanceMatrix"/> corresponding with the
        ///// <paramref name="key"/>.
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        ///// <see cref="DistanceMatrix"/>
        ///// <see cref="this"/>
        //protected DistanceMatrix GetMatrix(Guid key)
        //{
        //    var this_Distances = this.Distances;
        //    if (!this_Distances.ContainsKey(key))
        //    {
        //        this_Distances.Add(key, this.CreateMatrix(this.Context));
        //    }
        //    return this_Distances[key];
        //}

        ///// <summary>
        ///// Indexer <see cref="GetMatrix"/> the <see cref="DistanceMatrix"/> corresponding
        ///// with the <paramref name="key"/>.
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        ///// <see cref="GetMatrix"/>
        //protected DistanceMatrix this[Guid key] => this.GetMatrix(key);

        /// <summary>
        /// Gets or Sets the Coefficient for use during <see cref="RoutingContext.Model"/>
        /// registration.
        /// </summary>
        public virtual int Coefficient { get; set; }

        /// <summary>
        /// Adds the <see cref="Dimension"/> to the <see cref="RoutingModel"/>. We are able
        /// to default <paramref name="slackMax"/> and <paramref name="zeroAccumulator"/>,
        /// as well as leverage <see cref="Name"/> properties for convenience, convention,
        /// etc. The only bits that are really essential in terms of adaptation are
        /// <paramref name="evaluatorIndex"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="evaluatorIndex">The Registered Evaluator index.</param>
        /// <param name="capacity">The vehicle Capacity for all vehicles being routed.</param>
        /// <param name="slackMax">The slack maximum.</param>
        /// <param name="zeroAccumulator">Whether to zero the accumulator.</param>
        /// <returns></returns>
        protected virtual bool AddDimension(int evaluatorIndex, long capacity, long slackMax = default, bool zeroAccumulator = true) =>
            this.Context.Model.AddDimension(evaluatorIndex, slackMax, capacity, zeroAccumulator, this.Name);

        /// <summary>
        /// Adds the <see cref="Dimension"/> to the <see cref="RoutingModel"/>. We are able
        /// to default <paramref name="slackMax"/> and <paramref name="zeroAccumulator"/>,
        /// as well as leverage <see cref="Name"/> properties for convenience, convention,
        /// etc. The only bits that are really essential in terms of adaptation are
        /// <paramref name="evaluatorIndex"/> and <paramref name="capacities"/>.
        /// </summary>
        /// <param name="evaluatorIndex">The Registed Evaluator index.</param>
        /// <param name="capacities">The Capacities for all of the vehicles being routed.</param>
        /// <param name="slackMax">The slack maximum.</param>
        /// <param name="zeroAccumulator">Whether ot zero the accumulator.</param>
        /// <returns></returns>
        protected virtual bool AddDimension(int evaluatorIndex, long[] capacities, long slackMax = default, bool zeroAccumulator = true) =>
            this.Context.Model.AddDimensionWithVehicleCapacity(evaluatorIndex, slackMax, capacities, zeroAccumulator, this.Name);

        /// <summary>
        /// Adds the <see cref="Dimension"/> to the <see cref="RoutingModel"/>. We are able
        /// to default <paramref name="slackMax"/> and <paramref name="zeroAccumulator"/>,
        /// as well as leverage <see cref="Name"/> properties for convenience, convention,
        /// etc. The only bits that are really essential in terms of adaptation are
        /// <paramref name="evaluatorIndices"/> and <paramref name="capacities"/>.
        /// </summary>
        /// <param name="evaluatorIndices">The Registed Evaluator indices.</param>
        /// <param name="capacities">The Capacities for each of the vehicles being routed.</param>
        /// <param name="slackMax">The slack maximum.</param>
        /// <param name="zeroAccumulator">Whether ot zero the accumulator.</param>
        /// <returns></returns>
        protected virtual bool AddDimension(int[] evaluatorIndices, long[] capacities, long slackMax = default, bool zeroAccumulator = true) =>
            this.Context.Model.AddDimensionWithVehicleTransitAndCapacity(evaluatorIndices, slackMax, capacities, zeroAccumulator, this.Name);

        /// <summary>
        /// Adds the <see cref="Dimension"/> to the <see cref="RoutingModel"/>. We are able
        /// to default <paramref name="slackMax"/> and <paramref name="zeroAccumulator"/>,
        /// as well as leverage <see cref="Name"/> properties for convenience, convention,
        /// etc. The only bits that are really essential in terms of adaptation are
        /// <paramref name="evaluatorIndices"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="evaluatorIndices">The Registed Evaluator indices.</param>
        /// <param name="capacity">The Capacity for all vehicles being routed.</param>
        /// <param name="slackMax">The slack maximum.</param>
        /// <param name="zeroAccumulator">Whether ot zero the accumulator.</param>
        /// <returns></returns>
        protected virtual bool AddDimension(int[] evaluatorIndices, long capacity, long slackMax = default, bool zeroAccumulator = true) =>
            this.Context.Model.AddDimensionWithVehicleTransits(evaluatorIndices, slackMax, capacity, zeroAccumulator, this.Name);

        /// <summary>
        /// Adds the Constant <paramref name="value"/> <see cref="Dimension"/>
        /// to the <see cref="RoutingModel"/>. We are able to default and
        /// <paramref name="zeroAccumulator"/>, as well as leverage <see cref="Name"/>
        /// properties for convenience, convention, etc. The only bits that are really
        /// essential in terms of adaptation are <paramref name="value"/> and
        /// <paramref name="capacity"/>.
        /// </summary>
        /// <param name="value">A constant Value for all vehicles being routed.</param>
        /// <param name="capacity">The Capacity for all vehicles being routed.</param>
        /// <param name="zeroAccumulator">Whether ot zero the accumulator.</param>
        /// <returns></returns>
        protected virtual bool AddConstantDimension(long value, long capacity, bool zeroAccumulator = true) =>
            this.Context.Model.AddConstantDimension(value, capacity, zeroAccumulator, this.Name);

        /// <summary>
        /// Adds the Constant <paramref name="value"/> <see cref="Dimension"/>
        /// to the <see cref="RoutingModel"/>. We are able to default and
        /// <paramref name="slackMax"/> and <paramref name="zeroAccumulator"/>,
        /// as well as leverage <see cref="Name"/> properties for convenience,
        /// convention, etc. The only bits that are really essential in terms of
        /// adaptation are <paramref name="value"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="value">A constant Value for all vehicles being routed.</param>
        /// <param name="capacity">The Capacity for all vehicles being routed.</param>
        /// <param name="slackMax">The slack maximum.</param>
        /// <param name="zeroAccumulator">Whether ot zero the accumulator.</param>
        /// <returns></returns>
        protected virtual bool AddConstantDimension(long value, long capacity, long slackMax = default, bool zeroAccumulator = true) =>
            this.Context.Model.AddConstantDimensionWithSlack(value, capacity, slackMax, zeroAccumulator, this.Name);

        /// <summary>
        /// Adds the Constant <paramref name="values"/> <see cref="Dimension"/>
        /// to the <see cref="RoutingModel"/>. We are able to default and
        /// <paramref name="zeroAccumulator"/>, as well as leverage
        /// <see cref="Name"/> properties for convenience, convention, etc.
        /// The only bits that are really essential in terms of adaptation
        /// are <paramref name="values"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="values">The constant Values per each vehicle being routed.</param>
        /// <param name="capacity">The Capacity for all vehicles being routed.</param>
        /// <param name="zeroAccumulator">Whether ot zero the accumulator.</param>
        /// <returns></returns>
        protected virtual bool AddVectorDimension(long[] values, long capacity, bool zeroAccumulator = true) =>
            this.Context.Model.AddVectorDimension(values, capacity, zeroAccumulator, this.Name);

        // TODO: TBD: eventually will probably want to add pickup and delivery comprehension...
        // TODO: TBD: possibly as a function of the Context as well...
        // TODO: TBD: local search? filter? operator?
        // TODO: TBD: search monitor... for debug reporting...
        // TODO: TBD: variable? weighted/max/min/target to finalizer?
        // TODO: TBD: locks?
        // TODO: TBD: arc more constrained?
        // TODO: TBD: close model? with params?
        // TODO: TBD: cost classes?
        // TODO: TBD: setting arc/fixed cost evaluators? vehicles?
        // TODO: TBD: and numerous others may require adaptation...
        // TODO: TBD: we might consider several callback style helper functions for the different form factors...
        // TODO: TBD: involving indices, routind model, etc...

        /// <summary>
        /// Sets the Arc Cost <paramref name="evaluatorIndex"/> for the specified
        /// <paramref name="vehicleIndex"/> participating in the model.
        /// </summary>
        /// <param name="evaluatorIndex"></param>
        /// <param name="vehicleIndex"></param>
        protected virtual void SetArcCostEvaluator(int evaluatorIndex, int vehicleIndex) =>
            this.Context.Model.SetArcCostEvaluatorOfVehicle(evaluatorIndex, vehicleIndex);

        /// <summary>
        /// Sets the Arc Cost <paramref name="evaluatorIndex"/> for All
        /// <see cref="ManagerContext.VehicleCount"/> participating in the model.
        /// </summary>
        /// <param name="evaluatorIndex"></param>
        protected virtual void SetArcCostEvaluators(int evaluatorIndex) =>
            this.Context.Model.SetArcCostEvaluatorOfAllVehicles(evaluatorIndex);

        // TODO: TBD: for now we assume params indices...
        // TODO: TBD: eventually we may also want an IEnumerable<long> overload...
        /// <summary>
        /// Adds a Disjunction to the Model assuming the <paramref name="indices"/>.
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        protected virtual int AddDisjunction(params long[] indices) =>
            this.Context.Model.AddDisjunction(indices);

        /// <summary>
        /// Adds a Disjunction given <paramref name="penalty"/>.
        /// </summary>
        /// <param name="penalty"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        protected virtual int AddDisjunction(long penalty, params long[] indices) =>
            this.Context.Model.AddDisjunction(indices, penalty);

        /// <summary>
        /// Adds a Disjunction given <paramref name="penalty"/> and
        /// <paramref name="maxCardinality"/>.
        /// </summary>
        /// <param name="penalty"></param>
        /// <param name="maxCardinality"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        protected virtual int AddDisjunction(long penalty, long maxCardinality, params long[] indices) =>
            this.Context.Model.AddDisjunction(indices, maxCardinality, penalty);

        /// <summary>
        /// Sets the Allowed <paramref name="vehicleIndices"/> corresponding
        /// to the <paramref name="node"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="vehicleIndices"></param>
        protected virtual void SetAllowedVehiclesForNode(int node, params int[] vehicleIndices) =>
            this.Context.Model.SetAllowedVehiclesForIndex(vehicleIndices, this.Context.NodeToIndex(node));

        /// <summary>
        /// Sets the <see cref="Dimension"/> as the Primary Constrained dimension.
        /// Leverages the <see cref="Name"/> property in order to do this.
        /// </summary>
        protected virtual void SetPrimaryConstrained() =>
            this.Context.Model.SetPrimaryConstrainedDimension(this.Name);

        /// <summary>
        /// Adds the <paramref name="constraints"/> via <see cref="RoutingContext"/>.
        /// </summary>
        /// <param name="constraints"></param>
        /// <returns></returns>
        /// <see cref="RoutingContext.AddConstraints(IEnumerable{Constraint})"/>
        protected virtual Dimension AddConstraints(IEnumerable<Constraint> constraints)
        {
            this.Context.AddConstraints(constraints);
            return this;
        }

        /// <summary>
        /// Adds the <paramref name="constraints"/> via <see cref="RoutingContext"/>.
        /// </summary>
        /// <param name="constraints"></param>
        /// <returns></returns>
        /// <see cref="RoutingContext.AddConstraints(Constraint[])"/>
        protected virtual Dimension AddConstraints(params Constraint[] constraints)
        {
            this.Context.AddConstraints(constraints);
            return this;
        }

        /// <summary>
        /// Adds the <paramref name="constraint"/> via <see cref="RoutingContext"/>.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        /// <see cref="RoutingContext.AddConstraint(Constraint)"/>
        protected virtual Dimension AddConstraint(Constraint constraint)
        {
            this.Context.AddConstraint(constraint);
            return this;
        }

        /// <summary>
        /// Adds a <paramref name="pickupNode"/> and <paramref name="deliveryNode"/>
        /// pair to the model, which nodes must both be serviced by the same Vehicle.
        /// </summary>
        /// <param name="pickupNode">The Node from which Pickup occurs.</param>
        /// <param name="deliveryNode">The Node to which Delivery occurs.</param>
        /// <see cref="RoutingContext.AddPickupAndDelivery"/>
        protected virtual void AddPickupAndDelivery(int pickupNode, int deliveryNode) =>
            this.Context.AddPickupAndDelivery(this, pickupNode, deliveryNode);

        /// <summary>
        /// Adds many <paramref name="nodes"/> Pickup and Delivery Constraints.
        /// </summary>
        /// <param name="nodes">The Nodes Adding Pickup and Delivery Constraints.</param>
        /// <remarks>Also requires that the Dimension has already been registered with
        /// the Model prior adding the pickups and deliveries constraints.</remarks>
        /// <see cref="RoutingContext.AddPickupsAndDeliveries"/>
        protected virtual void AddPickupsAndDeliveries(params (int pickup, int delivery)[] nodes) =>
            this.Context.AddPickupsAndDeliveries(this, nodes);

        /// <summary>
        /// Adds many <paramref name="nodes"/> Pickup and Delivery Constraints.
        /// </summary>
        /// <param name="nodes">The Nodes Adding Pickup and Delivery Constraints.</param>
        /// <remarks>Also requires that the Dimension has already been registered with
        /// the Model prior adding the pickups and deliveries constraints.</remarks>
        /// <see cref="RoutingContext.AddPickupsAndDeliveries"/>
        protected virtual void AddPickupsAndDeliveries(IEnumerable<(int pickup, int delivery)> nodes) =>
            this.Context.AddPickupsAndDeliveries(this, nodes.OrEmpty().ToArray());

        /// <summary>
        /// Sets the Accumulator Variable <em>Lower-</em> and <em>Upper-Bounds</em>
        /// corresponding to the Index <paramref name="i"/> translated according to
        /// <paramref name="onTranslateIndex"/> terms. Assumes that the Dimension has
        /// been added to the Model.
        /// </summary>
        /// <param name="i">A Domain oriented Index.</param>
        /// <param name="lowerBound">The Lower Bound.</param>
        /// <param name="upperBound">The Upper Bound.</param>
        /// <param name="onTranslateIndex">Translates the Index <paramref name="i"/> in Model terms.</param>
        /// <remarks>We deliberately control access as Protected Internal because both derived
        /// assets as well as internal extension methods must be able to successfully access
        /// the method.</remarks>
        protected internal virtual void SetCumulVarRange(int i, long lowerBound, long upperBound
            , TranslateRoutingContextIndexCallback onTranslateIndex)
        {
            var index = onTranslateIndex.Invoke(this.Context, i);
            this.MutableDimension.CumulVar(index).SetRange(lowerBound, upperBound);
        }

        /// <summary>
        /// Sets the Accumulator Variable <em>Lower-</em> and <em>Upper-Bounds</em>
        /// corresponding to the Index <paramref name="i"/> translated according to
        /// <paramref name="onTranslateIndex"/> terms. Assumes that the Dimension has
        /// been added to the Model.
        /// </summary>
        /// <param name="i">A Domain oriented Index.</param>
        /// <param name="bounds">The Lower and Upper Bounds.</param>
        /// <param name="onTranslateIndex">Translates the Index <paramref name="i"/> in Model terms.</param>
        /// <remarks>We deliberately control access as Protected Internal because both derived
        /// assets as well as internal extension methods must be able to successfully access
        /// the method.</remarks>
        protected internal virtual void SetCumulVarRange(int i, in (long lower, long upper) bounds
            , TranslateRoutingContextIndexCallback onTranslateIndex) =>
            this.SetCumulVarRange(i, bounds.lower, bounds.upper, onTranslateIndex);

        // TODO: TBD: could perhaps add other variations of the finalizer concerns...
        /// <summary>
        /// Adds a Variable Minimized by Finalizer instruction to the Model. Assumes that
        /// the Dimension has been added to the Model.
        /// </summary>
        /// <param name="i">A Domain oriented Index.</param>
        /// <param name="onTranslateIndex">Translates the Index <paramref name="i"/> in Model terms.</param>
        /// <remarks>As with the other helper methods, the purpose of the finalizer methods is to
        /// abstract away the mechanics of arranging such, in order to allow the Dimension author
        /// to focus on the important aspects pertaining to the dimension in particular.
        /// Additionally, we deliberately control access as Protected Internal because both
        /// derived assets as well as internal extension methods must be able to successfully
        /// access the method.</remarks>
        /// <see cref="AddVariableMaximizedByFinalizer(int, TranslateRoutingContextIndexCallback)"/>
        protected internal virtual void AddVariableMinimizedByFinalizer(int i
            , TranslateRoutingContextIndexCallback onTranslateIndex)
        {
            var this_Context = this.Context;
            var index = onTranslateIndex.Invoke(this_Context, i);
            this_Context.Model.AddVariableMinimizedByFinalizer(this.MutableDimension.CumulVar(index));
        }

        /// <summary>
        /// Adds a Variable Minimized by Finalizer instruction to the Model. Assumes that
        /// the Dimension has been added to the Model.
        /// </summary>
        /// <param name="i">A Domain oriented Index.</param>
        /// <param name="onTranslateIndex">Translates the Index <paramref name="i"/> in Model terms.</param>
        /// <remarks>As with the other helper methods, the purpose of the finalizer methods is to
        /// abstract away the mechanics of arranging such, in order to allow the Dimension author
        /// to focus on the important aspects pertaining to the dimension in particular.
        /// Additionally, we deliberately control access as Protected Internal because both
        /// derived assets as well as internal extension methods must be able to successfully
        /// access the method.</remarks>
        /// <see cref="AddVariableMinimizedByFinalizer(int, TranslateRoutingContextIndexCallback)"/>
        protected internal virtual void AddVariableMaximizedByFinalizer(int i, TranslateRoutingContextIndexCallback onTranslateIndex)
        {
            var this_Context = this.Context;
            var index = onTranslateIndex.Invoke(this_Context, i);
            this_Context.Model.AddVariableMaximizedByFinalizer(this.MutableDimension.CumulVar(index));
        }

        /// <summary>
        /// Constructs a Dimension given <paramref name="context"/> and
        /// <paramref name="coefficient"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="coefficient"></param>
        protected Dimension(RoutingContext context, int coefficient)
        {
            this.Context = context;

            this.DefaultCallbacks = new Callbacks();

            /* TODO: TBD: Borderline bidirectional relationship going on here, but there is
             * not enough, we think, to justify a dependency on our Bidirectionals package.
             * Of course, this could change and the cost/benefit is there after all:
             * https://nuget.org/packages/Ellumination.Collections.Bidirectionals */

            context.InternalDimensions.Add(this);

            //this.Distances = new Dictionary<Guid, DistanceMatrix>();

            this.Coefficient = coefficient;

            TryValidateCallbackIndex(this.ZeroTransitEvaluationIndex);

            //void ValidateMatrix(DistanceMatrix _)
            //{
            //    // TODO: TBD: and we may actually put a validation in...
            //}

            //ValidateMatrix(this[this.Id]);
        }

        /// <summary>
        /// Represents convenience <see cref="Zero"/> oriented Callbacks.
        /// </summary>
        protected class Callbacks
        {
            /// <summary>
            /// Gets the Zero value.
            /// </summary>
            private long Zero { get; } = default;

            /// <summary>
            /// Internal constructor.
            /// </summary>
            internal Callbacks() : this(default)
            {
            }

            /// <summary>
            /// Internal constructor.
            /// </summary>
            /// <param name="zero"></param>
            internal Callbacks(long zero)
            {
                this.Zero = zero;
                this.OnZeroTransitEvaluation = delegate { return this.Zero; };
                this.OnZeroUnaryTransitEvaluation = delegate { return this.Zero; };
            }

            /// <summary>
            /// Internal constructor.
            /// </summary>
            /// <param name="onTransitEvaluation"></param>
            /// <param name="onUnaryTransitEvaluation"></param>
            internal Callbacks(
                RoutingTransitEvaluationCallback onTransitEvaluation
                , RoutingUnaryTransitEvaluationCallback onUnaryTransitEvaluation
            )
            {
                this.OnZeroTransitEvaluation = onTransitEvaluation;
                this.OnZeroUnaryTransitEvaluation = onUnaryTransitEvaluation;
            }

            /// <summary>
            /// 
            /// </summary>
            private RoutingTransitEvaluationCallback OnZeroTransitEvaluation { get; }

            /// <summary>
            /// 
            /// </summary>
            internal RoutingTransitEvaluationCallback ZeroTransitEvaluation =>
                this.OnZeroTransitEvaluation;

            /// <summary>
            /// 
            /// </summary>
            private RoutingUnaryTransitEvaluationCallback OnZeroUnaryTransitEvaluation { get; }

            /// <summary>
            /// 
            /// </summary>
            internal RoutingUnaryTransitEvaluationCallback ZeroUnaryTransitEvaluation =>
                this.OnZeroUnaryTransitEvaluation;
        }
    }
}
