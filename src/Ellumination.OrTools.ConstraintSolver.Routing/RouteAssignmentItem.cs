namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Google.OrTools.ConstraintSolver;

    // TODO: TBD: do we provide a Domain friendly version of the same?
    // TODO: TBD: our only hesitation there lies in the fact that it would add more of a generic type list...
    /// <summary>
    /// Provides a first class <see cref="Assignment"/> Route comprehension, whose primary
    /// objective it is to capture not only domain level indexing, but also model level indexing.
    /// </summary>
    public class RouteAssignmentItem
    {
        /// <summary>
        /// Gets the Context for use by the subscriber.
        /// </summary>
        public virtual RoutingContext Context { get; }

        /// <summary>
        /// Gets the Model for use by the subscriber.
        /// </summary>
        public virtual RoutingModel Model => this.Context.Model;

        /// <summary>
        /// Gets the Manager for use by the subscriber.
        /// </summary>
        public virtual RoutingIndexManager Manager => this.Context.Manager;

        /// <summary>
        /// Gets the Vehicle involved during the <see cref="Assignment"/>.
        /// </summary>
        public virtual int Vehicle { get; }

        /// <summary>
        /// Gets the Node involved during the <see cref="Assignment"/>.
        /// </summary>
        public virtual int Node { get; }

        /// <summary>
        /// Gets the Previous Node involved during the <see cref="Assignment"/>.
        /// </summary>
        public virtual int? PreviousNode { get; }

        /// <summary>
        /// Gets the Index corresponding to the <see cref="Node"/>.
        /// </summary>
        public long Index { get; }

        /// <summary>
        /// Gets the PreviousIndex corresponding to the <see cref="PreviousNode"/>.
        /// </summary>
        public virtual long? PreviousIndex { get; }

        /// <summary>
        /// Allows an item instance to be deconstructed along similar lines as a Value Tuple.
        /// </summary>
        /// <param name="vehicle">Receives the <see cref="Vehicle"/> value.</param>
        /// <param name="node">Receives the <see cref="Node"/> value.</param>
        /// <param name="previousNode">Receives the <see cref="PreviousNode"/> value.</param>
        public virtual void Deconstruct(out int vehicle, out int node, out int? previousNode) =>
            (vehicle, node, previousNode) = (this.Vehicle, this.Node, this.PreviousNode);

        /// <summary>
        /// Allows an item instance to be deconstructed along similar lines as a Value Tuple.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="index"></param>
        /// <param name="node"></param>
        /// <param name="previousIndex"></param>
        /// <param name="previousNode"></param>
        public virtual void Deconstruct(out int vehicle, out long index, out int node
            , out long? previousIndex, out int? previousNode) =>
            (vehicle, index, node, previousIndex, previousNode)
                = (this.Vehicle, this.Index, this.Node, this.PreviousIndex, this.PreviousNode);

        /// <summary>
        /// Constructs a <see cref="RouteAssignmentItem"/> for Internal use.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vehicle"></param>
        /// <param name="index"></param>
        /// <param name="node"></param>
        /// <param name="previousIndex"></param>
        /// <param name="previousNode"></param>
        internal RouteAssignmentItem(RoutingContext context, int vehicle, long index, int node
            , long? previousIndex, int? previousNode)
        {
            this.Context = context;
            this.Vehicle = vehicle;
            this.Index = index;
            this.Node = node;
            this.PreviousIndex = previousIndex;
            this.PreviousNode = previousNode;
            // TODO: TBD: short of also capturing cumulvars ... would that be helpful?
            // TODO: TBD: we think possibly, but this also requires semi-intimate knowledge of the dimensions, names, ids, etc...
        }
    }
}
