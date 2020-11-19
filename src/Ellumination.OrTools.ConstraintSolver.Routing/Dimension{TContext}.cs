using System;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Distances;
    using Google.OrTools.ConstraintSolver;

    // TODO: TBD: will consider dimension bits next...
    // TODO: TBD: for now this is simply a placeholder...
    // TODO: TBD: with the intention of cross referencing with the Context in some way shape or form...
    /// <summary>
    /// Provides several layers of evaluation. First, the transit indices themselves must
    /// be evaluated for validatity, whether in binary or unary form. Second, the transit
    /// arc cost must be evaluated once the indices have been validated, again whether in
    /// binary or unary form. Overall, Dimension does not have enough contextual information
    /// to know whether <see cref="RoutingModel.RegisterTransitCallback"/> or
    /// <see cref="RoutingModel.RegisterUnaryTransitCallback"/> should be invoked
    /// when registering with the <see cref="Context.Model"/>. Any additional bits like
    /// domain specific constraints, etc, are also a derived issue, up to the subscriber
    /// to implement. What Dimension can provide, however, is sufficient framework scaffold
    /// in order to meet the callbacks when they become necessary.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="RoutingModel"/>
    /// <see cref="RoutingModel.RegisterTransitCallback"/>
    /// <see cref="RoutingModel.RegisterUnaryTransitCallback"/>
    public abstract class Dimension<TContext> : Dimension
        where TContext : Context
    {
        /// <summary>
        /// Gets the Default or Zero Transit Cost, <c>default</c> or <c>0L</c>.
        /// </summary>
        protected virtual long ZeroTransitCost { get; } = default;

        /// <summary>
        /// Gets the IntraDepotTransitCost. Default Cost prohibits Transit between Depots
        /// in a low-cost-wins Routing Model, <c>1 &lt;&lt; 16</c>, or <em>64 times 1024</em>.
        /// Essentially, we want to leave adequate room for any Coefficients to take effect,
        /// as well as particularly well within any <see cref="int.MaxValue"/> or
        /// <see cref="long.MaxValue"/> overruns.
        /// </summary>
        /// <see cref="InterDepotTransitCost"/>
        protected virtual long IntraDepotTransitCost { get; } = 1 << 16;

        /// <summary>
        /// Conversely to <see cref="IntraDepotTransitCost"/>, always permit Transit between
        /// Depot and non Depot Nodes. Default Cost is the <c>default</c>, or <c>0</c>.
        /// </summary>
        /// <see cref="IntraDepotTransitCost"/>
        protected virtual long InterDepotTransitCost { get; } = default;

        /// <summary>
        /// Gets the <typeparamref name="TContext"/> associated with the
        /// <see cref="Dimension{TContext}"/>.
        /// </summary>
        protected new virtual TContext Context => base.Context as TContext;

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="context"></param>
        protected Dimension(TContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Override in order to complete the domain model specific Dimension evaluation
        /// given the two indices <paramref name="i"/> and <paramref name="j"/>. When this
        /// method is invoked both indices will have already been validated and should be
        /// correct for the domain model specific calculation.
        /// </summary>
        /// <param name="i">Node from which whose transit arc cost is being evaluated.</param>
        /// <param name="j">Node to which whose transit arc cost is being evaluated.</param>
        /// <returns>Returns the calculated route arc cost in the current Dimension.</returns>
        protected virtual int OnEvaluateTransit(int i, int j) => default;

        /// <summary>
        /// Override in order to complete the unary domain model specific Dimension evaluation
        /// given the index <paramref name="i"/>. When this method is invokved, the index will
        /// have already been validated and should be corrrect for the domain model specific
        /// calculation.
        /// </summary>
        /// <param name="i">Index of the node whose transit arc cost is being evaluated.</param>
        /// <returns>Returns the calculated route arc cost in the current Dimension.</returns>
        protected virtual int OnEvaluateUnaryTransit(int i) => default;

        /// <summary>
        /// Returns whether the <see cref="Context.DepotCoordinates"/> Contain the
        /// <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool GetDepotCoordinatesContainIndex(long index) =>
            this.Context.DepotCoordinates.Any(x => x == index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">The Index being screend for validity.</param>
        /// <param name="cost">The Cost incurred when Evaluation determines
        /// whether <paramref name="index"/> is a Depot.</param>
        /// <param name="default">The Default value being returned when screened.</param>
        /// <returns></returns>
        protected virtual long? OnEvaluateIndex(long index, long? cost = null, long @default = default) =>
            this.GetDepotCoordinatesContainIndex(index) ? (cost ?? @default) : (long?)null;

        /// <summary>
        /// Evaluates whether <see cref="IntraDepotTransitCost"/> or
        /// <see cref="InterDepotTransitCost"/> occurs given <paramref name="depots"/>.
        /// Defaults to <c>null</c>. <paramref name="depots"/> may be either One or Two
        /// elements, depending on whether the Binary or Unary Transit evaluation occurs.
        /// </summary>
        /// <param name="depots"></param>
        /// <returns></returns>
        /// <see cref="OnEvaluateTransit(long, long)"/>
        /// <see cref="OnEvaluateUnaryTransit(long)"/>
        protected virtual long? OnEvaluateIntraOrInterDepotTransitCost(params bool[] depots)
        {
            void OnValidateDepots(int length)
            {
                // Expecting One or Two flags, Unary or Binary Transit evaluation, respectively.
                if (length == 1 || length == 2)
                {
                    return;
                }

                throw new InvalidOperationException($"Invalid number of {nameof(depots)}:"
                    + $" [{string.Join(", ", depots.Select(x => $"{x}".ToLower()))}]"
                )
                {
                    Data = {
                        { nameof(length), length },
                        { nameof(depots), depots }
                    }
                };
            }

            OnValidateDepots(depots.Length);

            return depots.Length > 1 && depots.All(x => x) ? this.IntraDepotTransitCost
                : depots.Any(x => x) ? this.InterDepotTransitCost : (long?)null;
        }

        /// <summary>
        /// In this implementation, the most common usage is for both<paramref name="i"/> and
        /// <paramref name="j"/> indices to be evaluated first prior to invoking the domain
        /// model oriented Transit Callback. There may be some Dimensions in which that is
        /// not desired, or that only one of the indices should be screened prior to doing so.
        /// That is for the Dimension author to evaluate the most appropriate implementation
        /// for his or her routing model.
        /// </summary>
        /// <param name="i">Node from which whose transit arc cost is being evaluated.</param>
        /// <param name="j">Node to which whose transit arc cost is being evaluated.</param>
        /// <param name="default">The Default value being returned when screend out.</param>
        /// <returns></returns>
        /// <see cref="OnEvaluateIndex"/>
        protected virtual long? OnEvaluateIndices(long i, long j, long @default = default) =>
            this.OnEvaluateIntraOrInterDepotTransitCost(
                this.GetDepotCoordinatesContainIndex(i)
                , this.GetDepotCoordinatesContainIndex(j)
            )
            ?? this.OnEvaluateIndex(i, this.IntraDepotTransitCost, @default)
            ?? this.OnEvaluateIndex(j, this.IntraDepotTransitCost, @default)
            ;

        /// <summary>
        /// Callback that occurs when <see cref="RoutingModel.RegisterTransitCallback"/>
        /// is invoked. In this version of the callback we evaluate the indices concerning
        /// the model nodes, and as long as those are valid, then we pass those normalized
        /// indices for qualitative evaluation aligned with the domain model.
        /// </summary>
        /// <param name="i">Node from which whose transit arc cost is being evaluated.</param>
        /// <param name="j">Node to which whose transit arc cost is being evaluated.</param>
        /// <returns></returns>
        /// <see cref="OnEvaluateIndices"/>
        /// <see cref="OnEvaluateTransit(int, int)"/>
        protected virtual long OnEvaluateTransit(long i, long j) =>
            // We must only discern between regular Nodes and Depot Nodes.
            this.OnEvaluateIndices(i, j)
                ?? this.OnEvaluateTransit((int)i, (int)j);

        /// <summary>
        /// Callback that occurs when <see cref="RoutingModel.RegisterUnaryTransitCallback"/>
        /// is invoked. In this version of the callback we evaluate the indices concerning the
        /// model nodes, and as long as those are valid, then we pass those normalized indices
        /// for qualitative evaluation aligned with the domain model.
        /// </summary>
        /// <param name="i">Index of the node whose transit arc cost is being evaluated.</param>
        /// <returns></returns>
        /// <see cref="OnEvaluateIndex"/>
        /// <see cref="OnEvaluateUnaryTransit(int)"/>
        protected virtual long OnEvaluateUnaryTransit(long i) =>
            this.OnEvaluateIntraOrInterDepotTransitCost(
                this.GetDepotCoordinatesContainIndex(i)
            )
            // Important that we make the transition so to speak to Int32 here...
            ?? this.OnEvaluateUnaryTransit((int)i)
            ;
    }
}
