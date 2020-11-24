using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using RoutingModel = Google.OrTools.ConstraintSolver.RoutingModel;
    using OnEvaluateTransitDelegate = Google.OrTools.ConstraintSolver.LongLongToLong;

    /// <summary>
    /// Specifies the Binary Transit Evaluation Callback.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="Context"/>
    /// <see cref="OnEvaluateTransitDelegate"/>
    public abstract class BinaryDimension<TContext> : Dimension<TContext, OnEvaluateTransitDelegate>
        where TContext : Context
    {
        /// <inheritdoc/>
        protected sealed override OnEvaluateTransitDelegate OnZeroTransitCost =>
            this.DefaultCallbacks.ZeroTransitCostBinaryCallback;

        /// <inheritdoc/>
        protected override int OnRegisterCallbackWithModel(OnEvaluateTransitDelegate callback, bool positive)
        {
            int RegisterCallbackWith(RoutingModel model) => !positive
                ? model.RegisterTransitCallback(callback)
                : model.RegisterPositiveTransitCallback(callback)
                ;

            return RegisterCallbackWith(this.Context.Model);
        }

        /// <inheritdoc/>
        protected BinaryDimension(TContext context, int coefficient, bool shouldRegister = true)
            : base(context, coefficient, shouldRegister)
        {
        }
    }
}
