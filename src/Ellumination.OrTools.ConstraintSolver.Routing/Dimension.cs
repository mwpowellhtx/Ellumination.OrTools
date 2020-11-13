using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Distances;
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// 
    /// </summary>
    public abstract class Dimension
    {
        /// <summary>
        /// Gets the Id associated with the Dimension.
        /// </summary>
        protected virtual Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the Context associated with the Dimension.
        /// </summary>
        protected virtual Context Context { get; }

        /// <summary>
        /// Gets the Dimension Name. This is used especially when Registering the Dimension
        /// with the <see cref="Context.Model"/> and when obtaining the corresponding
        /// <see cref="RoutingDimension"/>. Defaults simply to the <see cref="object.GetType"/>
        /// <see cref="MemberInfo.Name"/>. Override in order to furnish a more specific Name
        /// depending upon the Dimension use case.
        /// </summary>
        public virtual string Name => this.GetType().Name;

        // TODO: TBD: is there a safer manner in which to identify whether the dimension has been added to the model?
        /// <summary>
        /// Gets the <see cref="RoutingDimension"/> corresponding to the <see cref="Context.Model"/> and
        /// <see cref="Name"/>, or Die in the process.
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
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected Dimension(Context context)
        {
            this.Context = context;

            this.Distances = new Dictionary<Guid, DistanceMatrix>();

            void ValidateMatrix(DistanceMatrix _)
            {
                // TODO: TBD: and we may actually put a validation in...
            }

            ValidateMatrix(this[this.Id]);
        }
    }
}
