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
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TDepot"></typeparam>
    /// <typeparam name="TVehicle"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="OnEvaluateTransitDelegate"/>
    /// <see cref="Context{TNode, TDepot, TVehicle}"/>
    /// <see cref="Dimension{TNode, TDepot, TVehicle, TContext, TCallback}"/>
    public abstract class UnaryDimension<TNode, TDepot, TVehicle, TContext>
        : Dimension<TNode, TDepot, TVehicle, TContext, OnEvaluateTransitDelegate>
        where TDepot : TNode
        where TContext : Context<TNode, TDepot, TVehicle>
    {
        /// <summary>
        /// Gets the Unary Dimension Zero Transit Cost response.
        /// </summary>
        protected sealed override OnEvaluateTransitDelegate OnZeroTransitCost { get; }

        /// <inheritdoc/>
        protected override OnEvaluateTransitDelegate EvaluationCallback => this.OnEvaluateUnaryTransit;

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
        protected UnaryDimension(TContext context, bool shouldRegister = true)
            : base(context, shouldRegister)
        {
            this.OnZeroTransitCost = delegate { return this.ZeroTransitCost; };
        }
    }
}
