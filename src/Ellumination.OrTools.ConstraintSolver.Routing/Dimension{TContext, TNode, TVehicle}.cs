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
    /// <typeparam name="TVehicle"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="Dimension{TNode, TVehicle, TContext, TCallback}"/>
    /// <see cref="OnEvaluateTransitDelegate"/>
    /// <see cref="Context{TNode, TVehicle}"/>
    public abstract class Dimension<TNode, TVehicle, TContext>
        : Dimension<TNode, TVehicle, TContext, OnEvaluateTransitDelegate>
        where TContext : Context<TNode, TVehicle>
    {
        /// <inheritdoc/>
        protected override long OnEvaluateTransit(long i, long j) => this.IsPositive
            ? Math.Max(default, base.OnEvaluateTransit(i, j))
            : base.OnEvaluateTransit(i, j)
            ;

        /// <inheritdoc/>
        protected override OnEvaluateTransitDelegate EvaluationCallback => this.OnEvaluateTransit;

        /// <inheritdoc/>
        protected override int OnRegisterCallbackWithModel(OnEvaluateTransitDelegate callback)
        {
            int RegisterCallbackWith(RoutingModel model) => this.IsPositive
                ? model.RegisterPositiveTransitCallback(callback)
                : model.RegisterTransitCallback(callback)
                ;

            return RegisterCallbackWith(this.Context.Model);
        }

        /// <inheritdoc/>
        protected Dimension(TContext context, bool positive, bool shouldRegister = true)
            : base(context, positive, shouldRegister)
        {
        }
    }
}
