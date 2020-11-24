using System;
using System.Collections.Generic;
using System.Reflection;
using Google.OrTools.ConstraintSolver;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Distances;
    using OnEvaluateBinaryTransitDelegate = LongLongToLong;
    using OnEvaluateUnaryTransitDelegate = LongToLong;

    /// <summary>
    /// 
    /// </summary>
    public abstract class Dimension : IDimension
    {
        /// <summary>
        /// Gets the ZeroTransitCost.
        /// </summary>
        protected virtual long ZeroTransitCost { get; } = default;

        /// <summary>
        /// Gets the Id associated with the Dimension.
        /// </summary>
        protected virtual Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the Context associated with the Dimension.
        /// </summary>
        protected virtual Context Context { get; }

        private string _name;

        /// <summary>
        /// Gets the Name of the Dimension especially for use with <see cref="Context.Model"/>.
        /// Override in order to specialize the Name in accordance with your Dimension. Defaults
        /// to <see cref="Type.FullName"/> via <see cref="object.GetType"/>.
        /// </summary>
        public virtual string Name => this._name ?? (this._name = this.GetType().FullName);

        /// <summary>
        /// Gets the <see cref="ICollection{T}"/> of RegisteredCallbackIndexes. Most Dimensions
        /// register a single Callback for an overarching Dimension that applies for all
        /// <see cref="Context.VehicleCount"/> elements. However, sometimes, we want to
        /// allow for multiple possible Vehicle specific callbacks to occur, in which
        /// case, we must allow for a collection of such Registered Callbacks.
        /// </summary>
        protected virtual ICollection<int> RegisteredCallbackIndexes { get; } = new List<int>();

        // TODO: TBD: is there a safer manner in which to identify whether the dimension has been added to the model?
        /// <summary>
        /// Gets the <see cref="RoutingDimension"/> corresponding to the
        /// <see cref="Context.Model"/> and <see cref="Name"/>, or Die in the process.
        /// </summary>
        public virtual RoutingDimension RoutingDimensionOrDie => this.Context.Model.GetDimensionOrDie(this.Name);

        /// <summary>
        /// Gets the Distances for use throughout the Dimension. Dimension may have one that
        /// is aligned with itself. Or they may be aligned with other bits, the vehicles, or
        /// nodes, etc, themselves, as the case may be, on a case by case Dimension basis.
        /// </summary>
        protected virtual IDictionary<Guid, DistanceMatrix> Distances { get; }

        /// <summary>
        /// Override in order to return an appropriate <see cref="DistanceMatrix"/> instnace.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual DistanceMatrix CreateMatrix(Context context) => new DistanceMatrix(context.NodeCount);

        /// <summary>
        /// Returns the <see cref="DistanceMatrix"/> corresponding with the
        /// <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <see cref="DistanceMatrix"/>
        /// <see cref="this"/>
        protected DistanceMatrix GetMatrix(Guid key)
        {
            var this_Distances = this.Distances;

            if (!this_Distances.ContainsKey(key))
            {
                this_Distances.Add(key, this.CreateMatrix(this.Context));
            }

            return this_Distances[key];
        }

        /// <summary>
        /// Indexer <see cref="GetMatrix"/> the <see cref="DistanceMatrix"/> corresponding
        /// with the <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <see cref="GetMatrix"/>
        protected DistanceMatrix this[Guid key] => this.GetMatrix(key);

        /// <summary>
        /// Gets the Coefficient for use during <see cref="Context.Model"/> registration.
        /// </summary>
        protected int Coefficient { get; }

        /// <summary>
        /// Gets the Default Binary and Unary Callbacks.
        /// </summary>
        protected virtual Callbacks DefaultCallbacks { get; }

        /// <summary>
        /// Constructs a Dimension given <paramref name="context"/> and
        /// <paramref name="coefficient"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="coefficient"></param>
        protected Dimension(Context context, int coefficient)
        {
            this.Context = context;

            this.DefaultCallbacks = new Callbacks(this.ZeroTransitCost);

            /* TODO: TBD: Borderline bidirectional relationship going on here, but there is
             * not enough, we think, to justify a dependency on our Bidirectionals package.
             * Of course, this could change and the cost/benefit is there after all:
             * https://nuget.org/packages/Ellumination.Collections.Bidirectionals */

            context.InternalDimensions.Add(this);

            this.Distances = new Dictionary<Guid, DistanceMatrix>();

            this.Coefficient = coefficient;

            void ValidateMatrix(DistanceMatrix _)
            {
                // TODO: TBD: and we may actually put a validation in...
            }

            ValidateMatrix(this[this.Id]);
        }

        /// <summary>
        /// 
        /// </summary>
        protected class Callbacks
        {
            /// <summary>
            /// Gets the Zero value.
            /// </summary>
            private long Zero { get; }

            /// <summary>
            /// Internal constructor.
            /// </summary>
            /// <param name="zero"></param>
            internal Callbacks(long zero)
            {
                this.Zero = zero;
            }

            /// <summary>
            /// 
            /// </summary>
            internal OnEvaluateBinaryTransitDelegate ZeroTransitCostBinaryCallback
            {
                get
                {
                    // TODO: TBD: refactor from the intermediate classes...
                    long OnEvaluateTransit(long fromIndex, long toIndex) => this.Zero;

                    return OnEvaluateTransit;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            internal OnEvaluateBinaryTransitDelegate EvaluateBinaryTransitCallback
            {
                get
                {
                    // TODO: TBD: refactor from the intermediate classes...
                    long OnEvaluateTransit(long fromIndex, long toIndex) => this.Zero;

                    return OnEvaluateTransit;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            internal OnEvaluateUnaryTransitDelegate ZeroTransitCostUnaryCallback
            {
                get
                {
                    // TODO: TBD: refactor from the intermediate classes...
                    long OnEvaluateTransit(long index) => this.Zero;

                    return OnEvaluateTransit;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            internal OnEvaluateUnaryTransitDelegate EvaluateUnaryTransitCallback
            {
                get
                {
                    // TODO: TBD: refactor from the intermediate classes...
                    long OnEvaluateTransit(long index) => this.Zero;

                    return OnEvaluateTransit;
                }
            }
        }
    }
}
