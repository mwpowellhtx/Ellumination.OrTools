using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using RoutingModel = Google.OrTools.ConstraintSolver.RoutingModel;
    using OnEvaluateTransitDelegate = Google.OrTools.ConstraintSolver.LongToLong;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TVehicle"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public abstract class UnaryDimension<TNode, TVehicle, TContext>
        : Dimension<TNode, TVehicle, TContext, OnEvaluateTransitDelegate>
        where TContext : Context<TNode, TVehicle>
    {
        /// <inheritdoc/>
        protected override long OnEvaluateUnaryTransit(long i) => this.IsPositive
            ? Math.Max(default, base.OnEvaluateUnaryTransit(i))
            : base.OnEvaluateUnaryTransit(i)
            ;

        /// <inheritdoc/>
        protected override OnEvaluateTransitDelegate EvaluationCallback => this.OnEvaluateUnaryTransit;

        /// <inheritdoc/>
        protected override int OnRegisterCallbackWithModel(OnEvaluateTransitDelegate callback)
        {
            int RegisterCallbackWith(RoutingModel model) => this.IsPositive
                ? model.RegisterPositiveUnaryTransitCallback(callback)
                : model.RegisterUnaryTransitCallback(callback)
                ;

            return RegisterCallbackWith(this.Context.Model);
        }

        /// <inheritdoc/>
        protected UnaryDimension(TContext context, bool positive, bool shouldRegister = true)
            : base(context, positive, shouldRegister)
        {
        }
    }
}
