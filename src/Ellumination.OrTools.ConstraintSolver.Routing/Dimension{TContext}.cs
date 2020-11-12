﻿using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Google.OrTools.ConstraintSolver;

    // TODO: TBD: will consider dimension bits next...
    // TODO: TBD: for now this is simply a placeholder...
    // TODO: TBD: with the intention of cross referencing with the Context in some way shape or form...
    /// <summary>
    /// Provides several layers of evaluation. First, the transit indices themselves must
    /// be evaluated for validatity, whether in binary or unary form. Second, the transit
    /// arc cost must be evaluated once the indices have been validated, again whether in
    /// binary or unary form. Overall, Dimension does not have enough contextual information
    /// to know whether <see cref="RoutingModel.RegisterTransitCallback"/>,
    /// <see cref="RoutingModel.RegisterPositiveTransitCallback"/>,
    /// <see cref="RoutingModel.RegisterUnaryTransitCallback"/> or
    /// <see cref="RoutingModel.RegisterPositiveUnaryTransitCallback"/> should be invoked
    /// when registering with the <see cref="Context.Model"/>. Any additional bits like
    /// domain specific constraints, etc, are also a derived issue, up to the subscriber
    /// to implement. What Dimension can provide, however, is sufficient framework scaffold
    /// in order to meet the callbacks when they become necessary.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="RoutingModel"/>
    /// <see cref="RoutingModel.RegisterTransitCallback"/>
    /// <see cref="RoutingModel.RegisterPositiveTransitCallback"/>
    /// <see cref="RoutingModel.RegisterUnaryTransitCallback"/>
    /// <see cref="RoutingModel.RegisterPositiveUnaryTransitCallback"/>
    public abstract class Dimension<TContext>
        where TContext : Context
    {
        /// <summary>
        /// Gets the Context associated with the Dimension.
        /// </summary>
        protected TContext Context { get; }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="context"></param>
        protected Dimension(TContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Override in order to complete the domain model specific Dimension evaluation
        /// given the two indices <paramref name="i"/> and <paramref name="j"/>. When this
        /// method is invoked both indices will have already been validated and should be
        /// correct for the domain model specific calculation.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns>Returns the calculated route arc cost in the current Dimension.</returns>
        protected virtual int OnEvaluateTransit(int i, int j) => default;

        /// <summary>
        /// Override in order to complete the unary domain model specific Dimension evaluation
        /// given the index <paramref name="i"/>. When this method is invokved, the index will
        /// have already been validated and should be corrrect for the domain model specific
        /// calculation.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Returns the calculated route arc cost in the current Dimension.</returns>
        protected virtual int OnEvaluateUnaryTransit(int i) => default;

        /// <summary>
        /// Evaluates the <paramref name="index"/> and returns either the
        /// <paramref name="default"/> or <c>null</c> consequently, which either
        /// seals the response, or leaves transit evaluation open for subsequent
        /// closure by the Dimension. Index must be within the appropriate
        /// <see cref="Context.StartEdge"/> or not in exces of
        /// <see cref="Context.NodeCount"/>, regardless of the incorporated
        /// <see cref="Context.Edge"/> factor.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        protected virtual long? EvaluateIndex(long index, long @default = default) =>
            index < this.Context.StartEdge || index >= this.Context.NodeCount ? @default : (long?)null;

        /// <summary>
        /// Callback that occurs when <see cref="RoutingModel.RegisterTransitCallback"/>
        /// or <see cref="RoutingModel.RegisterPositiveTransitCallback"/>. In this version
        /// of the callback we evaluate the indices concerning the model nodes, and as long
        /// as those are valid, then we pass those normalized indices for qualitative evaluation
        /// aligned with the domain model.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected virtual long OnEvaluateTransit(long i, long j)
        {
            (int i, int j, int delta) CalculateNormalizedIndexes(int delta) =>
                ((int)i + delta, (int)j + delta, delta);

            var indexes = CalculateNormalizedIndexes(-this.Context.StartEdge);

            return this.EvaluateIndex(i)
                ?? this.EvaluateIndex(j)
                ?? this.OnEvaluateTransit(indexes.i, indexes.j)
                ;
        }

        /// <summary>
        /// Callback that occurs when <see cref="RoutingModel.RegisterUnaryTransitCallback"/>
        /// or <see cref="RoutingModel.RegisterPositiveUnaryTransitCallback"/>. In this version
        /// of the callback we evaluate the indices concerning the model nodes, and as long
        /// as those are valid, then we pass those normalized indices for qualitative evaluation
        /// aligned with the domain model.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        protected long OnEvaluateUnaryTransit(long i)
        {
            (int i, int delta) CalculateNormalizedIndex(int delta) =>
                ((int)i + delta, delta);

            var index = CalculateNormalizedIndex(-this.Context.StartEdge);

            return this.EvaluateIndex(i)
                ?? this.OnEvaluateUnaryTransit(index.i)
                ;
        }
    }
}
