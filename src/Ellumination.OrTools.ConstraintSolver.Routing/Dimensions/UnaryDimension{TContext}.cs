using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using RoutingModel = Google.OrTools.ConstraintSolver.RoutingModel;
    using OnEvaluateTransitDelegate = Google.OrTools.ConstraintSolver.LongToLong;

    /// <summary>
    /// Specifies the Unary Transit Evaluation Callback.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="Context"/>
    /// <see cref="OnEvaluateTransitDelegate"/>
    public abstract class UnaryDimension<TContext> : Dimension<TContext, OnEvaluateTransitDelegate>
        where TContext : Context
    {
        /// <inheritdoc/>
        protected sealed override OnEvaluateTransitDelegate OnZeroTransitCost =>
            this.DefaultCallbacks.ZeroTransitCostUnaryCallback;

        /// <inheritdoc/>
        protected override int OnRegisterCallbackWithModel(OnEvaluateTransitDelegate callback, bool positive)
        {
            int RegisterCallbackWith(RoutingModel model) => !positive
                ? model.RegisterUnaryTransitCallback(callback)
                : model.RegisterPositiveUnaryTransitCallback(callback)
                ;

            return RegisterCallbackWith(this.Context.Model);
        }

        /// <inheritdoc/>
        protected UnaryDimension(TContext context, int coefficient, bool shouldRegister = true)
            : base(context, coefficient, shouldRegister)
        {
        }
    }
}
