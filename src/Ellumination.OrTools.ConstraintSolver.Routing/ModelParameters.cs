using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using ConstraintSolverParameters = Google.OrTools.ConstraintSolver.ConstraintSolverParameters;
    using RoutingModelParameters = Google.OrTools.ConstraintSolver.RoutingModelParameters;

    /// <summary>
    /// Offers an opportunity to adapt Model parameters into the wrapper framework.
    /// </summary>
    /// <see cref="!:https://github.com/google/or-tools/issues/2301">[dotnet::routing]
    /// Unseal the sealed parameters</see>
    /// <see cref="!:https://github.com/protocolbuffers/protobuf/issues/8169">[csharp]
    /// Allow for unsealed message output to be generated</see>
    public class ModelParameters : Parameters
    {
        /// <summary>
        /// Advanced settings. If set to <c>true</c> reduction of the underlying constraint
        /// model will be attempted when all vehicles have exactly the same cost structure.
        /// This can result in significant speedups.
        /// </summary>
        public virtual bool ReduceVehicleCostModel { get; set; }

        /// <summary>
        /// Cache callback calls if the number of nodes in the model is less or equal to this
        /// value.
        /// </summary>
        public virtual int MaxCallbackCacheSize { get; set; }

        /// <summary>
        /// Gets the Parameters to use in the underlying constraint solver.
        /// </summary>
        public virtual ConstraintSolverParameters SolverParameters { get; private set; } = new ConstraintSolverParameters();

        /// <summary>
        /// 
        /// </summary>
        internal ModelParameters()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        internal ModelParameters(RoutingModelParameters source)
            : base(source) =>
            CopyFrom(this, source);

        /// <summary>
        /// Copies <paramref name="target"/> From the <paramref name="source"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ModelParameters CopyFrom(ModelParameters target, RoutingModelParameters source)
        {
            target.MaxCallbackCacheSize = source.MaxCallbackCacheSize;
            target.ReduceVehicleCostModel = source.ReduceVehicleCostModel;
            target.SolverParameters = source.SolverParameters;
            return target;
        }

        /// <summary>
        /// Copies <paramref name="source"/> To the <paramref name="target"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static RoutingModelParameters CopyTo(ModelParameters source, RoutingModelParameters target)
        {
            target.MaxCallbackCacheSize = source.MaxCallbackCacheSize;
            target.ReduceVehicleCostModel = source.ReduceVehicleCostModel;
            target.SolverParameters = source.SolverParameters;
            return target;
        }

        /// <summary>
        /// Implicitly converts <paramref name="source"/> to a
        /// <see cref="RoutingModelParameters"/> instance.
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator RoutingModelParameters(ModelParameters source) =>
            source == null ? null : CopyTo(source, new RoutingModelParameters());

        /// <summary>
        /// Implicitly converts <paramref name="source"/> to a <see cref="ModelParameters"/>
        /// instance.
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ModelParameters(RoutingModelParameters source) =>
            source == null ? null : CopyFrom(new ModelParameters(), source);
    }
}
