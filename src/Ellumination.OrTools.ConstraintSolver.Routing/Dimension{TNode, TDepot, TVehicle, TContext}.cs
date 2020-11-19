using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using RoutingModel = Google.OrTools.ConstraintSolver.RoutingModel;
    using OnEvaluateTransitDelegate = Google.OrTools.ConstraintSolver.LongLongToLong;

    /// <summary>
    /// This Dimension invokes the Binary Transit Evaluation Callback.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TDepot"></typeparam>
    /// <typeparam name="TVehicle"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="Dimension{TNode, TDepot, TVehicle, TContext, TCallback}"/>
    /// <see cref="OnEvaluateTransitDelegate"/>
    /// <see cref="Context{TNode, TDepot, TVehicle}"/>
    public abstract class Dimension<TNode, TDepot, TVehicle, TContext>
        : Dimension<TNode, TDepot, TVehicle, TContext, OnEvaluateTransitDelegate>
        where TDepot : TNode
        where TContext : Context<TNode, TDepot, TVehicle>
    {
        /// <summary>
        /// Gets the Binary Dimension Zero Transit Cost response.
        /// </summary>
        protected sealed override OnEvaluateTransitDelegate OnZeroTransitCost { get; }

        /// <inheritdoc/>
        protected override OnEvaluateTransitDelegate EvaluationCallback => this.OnEvaluateTransit;

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
        protected Dimension(TContext context, bool shouldRegister = true)
            : base(context, shouldRegister)
        {
            this.OnZeroTransitCost = delegate { return this.ZeroTransitCost; };
        }
    }
}
