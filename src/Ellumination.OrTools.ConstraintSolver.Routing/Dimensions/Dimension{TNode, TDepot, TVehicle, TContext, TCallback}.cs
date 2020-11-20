using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Distances;

    // TODO: TBD: ditto basically a placeholder at this moment...
    /// <summary>
    /// Dimension naturally supports the Binary version of the Transit Evaluation
    /// Callback. Unless otherwise specified via the
    /// <see cref="UnaryDimension{TNode, TDepot, TVehicle, TContext}"/>, assume that
    /// the Binary callback will be invoked.
    /// </summary>
    /// <typeparam name="TNode">The type of the Node.</typeparam>
    /// <typeparam name="TDepot"></typeparam>
    /// <typeparam name="TVehicle">The type of the Vehicle.</typeparam>
    /// <typeparam name="TContext">The Context.</typeparam>
    /// <typeparam name="TCallback">A Callback <see cref="Delegate"/> type. We do not
    /// need to know specifically what the Delegate is at this level, only that such
    /// a callback is expected to occur when wiring up the registered callback with
    /// the Routing <see cref="Context.Model"/>.</typeparam>
    /// <see cref="Delegate"/>
    /// <see cref="Dimension{TContext}"/>
    /// <see cref="Context{TNode, TDepot, TVehicle}"/>
    public abstract class Dimension<TNode, TDepot, TVehicle, TContext, TCallback> : Dimension<TContext>
        where TDepot : TNode
        where TContext : Context<TNode, TDepot, TVehicle>
        where TCallback : Delegate
    {
        // TODO: TBD: need to connect the dots between registered callbacks and the corresponding matrix...
        // TODO: TBD: either corresponding to the Dimension matrix itself, or vehicle specific matrix...
        /// <inheritdoc/>
        protected override DistanceMatrix CreateMatrix(Context context)
        {
            if (context is TContext noviContext && typeof(ILocatable).IsAssignableFrom<TNode>())
            {
                // TODO: TBD: at this moment we might also consider whether a matrix merge is appropriate...
                // TODO: TBD: also involving context...
                var noviContext_Nodes = noviContext.Nodes;
                var locations = noviContext_Nodes.OfType<ILocatable>().Select(x => x.Location).ToArray();
                // TODO: TBD: validate that we have the same number of locations as we do the nodes themselves...
                // TODO: TBD: ...which we should have.
                return new LocationDistanceMatrix(locations);
            }

            return base.CreateMatrix(context);
        }

        /// <summary>
        /// Connects the dots given <paramref name="onRegister"/> and the expected
        /// <paramref name="callback"/>. Returns the Callback Index as Registerd
        /// with the <see cref="Context.Model"/>.
        /// </summary>
        /// <param name="onRegister">Connects the <paramref name="callback"/> with the
        /// <see cref="Context.Model"/>. Ostensibly is a method on the instance itself,
        /// but it need not be the case.</param>
        /// <param name="callback">The Evaluation Callback being registered with the
        /// <see cref="Context.Model"/>.</param>
        /// <returns>The callback index as registered with the <see cref="Context.Model"/>.</returns>
        private int OnRegisterCallback(Func<TCallback, int> onRegister, TCallback callback) =>
            onRegister.Invoke(callback);

        /// <summary>
        /// Registers the <paramref name="callback"/> With the <see cref="Context.Model"/>.
        /// Returns the Index of the Registered <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">The Callback being Registered.</param>
        /// <returns>The Index of the Registered <paramref name="callback"/>.</returns>
        protected virtual int OnRegisterCallbackWithModel(TCallback callback) =>
            this.OnRegisterCallbackWithModel(callback, default);

        /// <summary>
        /// Registers the <paramref name="callback"/> With the <see cref="Context.Model"/>.
        /// Returns the Index of the Registered <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">The Callback being Registered.</param>
        /// <param name="positive">Whether the <paramref name="callback"/>
        /// is considered Positive by the Model.</param>
        /// <returns>The Index of the Registered <paramref name="callback"/>.</returns>
        protected abstract int OnRegisterCallbackWithModel(TCallback callback, bool positive);

        /// <summary>
        /// Gets the EvaluationCallback that will be registered.
        /// </summary>
        protected abstract TCallback EvaluationCallback { get; }

        /// <summary>
        /// Gets the OnZeroTransitCost, however, must override this concerning Unary etc.
        /// </summary>
        protected abstract TCallback OnZeroTransitCost { get; }

        /// <summary>
        /// Gets the <see cref="ICollection{T}"/> of RegisteredCallbackIndexes. Most Dimensions
        /// register a single Callback for an overarching Dimension that applies for all
        /// <see cref="Context.VehicleCount"/> elements. However, sometimes, we want to
        /// allow for multiple possible Vehicle specific callbacks to occur, in which
        /// case, we must allow for a collection of such Registered Callbacks.
        /// </summary>
        /// <remarks>The first <em>Registered Callback</em> will alway sbe the
        /// <see cref="OnZeroTransitCost"/> Callback. Callbacks in addition to that
        /// will start at index <c>1</c>.</remarks>
        protected virtual ICollection<int> RegisteredCallbackIndexes { get; } = new List<int>();

        /// <summary>
        /// Constructs a Dimension given <paramref name="context"/>,
        /// <paramref name="coefficient"/>, and whether <paramref name="shouldRegister"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="coefficient"></param>
        /// <param name="shouldRegister">Most Dimensions will go ahead and Register with the
        /// Model, but not all. There can be corner cases that Register Callbacks in a Vehicle
        /// specific manner, for instance. When <c>true</c>, in this case, we do allow for a
        /// default registration.</param>
        protected Dimension(TContext context, int coefficient, bool shouldRegister = true)
            : base(context, coefficient)
        {
            // We always expect there to be a Zero Callback.
            this.RegisteredCallbackIndexes.Add(this.OnRegisterCallback(this.OnRegisterCallbackWithModel, this.OnZeroTransitCost));

            if (shouldRegister)
            {
                // TODO: TBD: which we could potentially further condition these elements based on Coefficients perhaps...
                this.RegisteredCallbackIndexes.Add(this.OnRegisterCallback(this.OnRegisterCallbackWithModel, this.EvaluationCallback));
            }
        }
    }
}
