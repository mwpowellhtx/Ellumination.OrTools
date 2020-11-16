using System;
using System.Linq;
using System.Reflection;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Distances;

    // TODO: TBD: ditto basically a placeholder at this moment...
    /// <summary>
    /// Dimension naturally supports the Binary version of the Transit Evaluation Callback.
    /// Unless otherwise specified via the
    /// <see cref="UnaryDimension{TNode, TVehicle, TContext}"/>, assume that the Binary
    /// callback will be invoked.
    /// </summary>
    /// <typeparam name="TNode">The type of the Node.</typeparam>
    /// <typeparam name="TVehicle">The type of the Vehicle.</typeparam>
    /// <typeparam name="TContext">The Context.</typeparam>
    /// <typeparam name="TCallback">A Callback <see cref="Delegate"/> type. We do not
    /// need to know specifically what the Delegate is at this level, only that such
    /// a callback is expected to occur when wiring up the registered callback with
    /// the Routing <see cref="Context.Model"/>.</typeparam>
    /// <see cref="Delegate"/>
    /// <see cref="Dimension{TContext}"/>
    /// <see cref="Context{TNode, TVehicle}"/>
    public abstract class Dimension<TNode, TVehicle, TContext, TCallback> : Dimension<TContext>
        where TContext : Context<TNode, TVehicle>
        where TCallback : Delegate
    {
        /// <inheritdoc/>
        protected override DistanceMatrix CreateMatrix(Context context)
        {
            if (context is TContext noviContext && typeof(ILocatable).IsAssignableFrom<TNode>())
            {
                // TODO: TBD: at this moment we might also consider whether a matrix merge is appropriate...
                // TODO: TBD: also involving context...
                var noviContext_Nodes = noviContext.Nodes;
                var locations = noviContext_Nodes.OfType<ILocatable>().Select(x => x.Location).ToArray();
                return new LocationDistanceMatrix(locations);
            }

            return base.CreateMatrix(context);
        }

        /// <summary>
        /// Gets whether Dimension Is considered Positive for transit callback purposes.
        /// </summary>
        protected virtual bool IsPositive { get; }

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
        protected abstract int OnRegisterCallbackWithModel(TCallback callback);

        /// <summary>
        /// Gets the EvaluationCallback that will be registered.
        /// </summary>
        protected abstract TCallback EvaluationCallback { get; }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="positive"></param>
        /// <param name="shouldRegister">Most Dimensions will go ahead and Register with the
        /// Model, but not all. There can be corner cases that Register Callbacks in a Vehicle
        /// specific manner, for instance.</param>
        protected Dimension(TContext context, bool positive, bool shouldRegister = true)
            : base(context)
        {
            this.IsPositive = positive;

            if (shouldRegister)
            {
                // TODO: TBD: which we could potentially further condition these elements based on Coefficients perhaps...
                this.RegisteredCallbackIndex = this.OnRegisterCallback(this.OnRegisterCallbackWithModel, this.EvaluationCallback);
            }
        }
    }
}
