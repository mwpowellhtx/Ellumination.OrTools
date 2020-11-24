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
    public abstract class Dimension<TContext, TCallback> : Dimension
        where TContext : Context
        where TCallback : Delegate
    {
        /// <summary>
        /// Gets the Context in terms of <typeparamref name="TContext"/>.
        /// </summary>
        public new virtual TContext Context { get; }

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
        /// Gets the OnZeroTransitCost, however, must override this concerning Unary etc.
        /// </summary>
        protected abstract TCallback OnZeroTransitCost { get; }

        /// <summary>
        /// Gets the TransitEvaluation that will be registered. Defaults to
        /// <see cref="OnZeroTransitCost"/>. Override in order to perform a
        /// different, <see cref="Dimension"/> specific <em>TransitEvaluation</em>.
        /// </summary>
        protected virtual TCallback TransitEvaluation => this.OnZeroTransitCost;

        /// <summary>
        /// Constructs a Dimension given <paramref name="context"/> and
        /// <paramref name="coefficient"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="coefficient"></param>
        /// <param name="shouldRegister"></param>
        protected Dimension(TContext context, int coefficient, bool shouldRegister = true)
            : base(context, coefficient)
        {
            // Saves having to convert the base class property every time.
            this.Context = context;

            // We always expect there to be a Zero Callback.
            this.RegisteredCallbackIndexes.Add(this.OnRegisterCallback(this.OnRegisterCallbackWithModel, this.OnZeroTransitCost));

            if (shouldRegister)
            {
                // TODO: TBD: which we could potentially further condition these elements based on Coefficients perhaps...
                this.RegisteredCallbackIndexes.Add(this.OnRegisterCallback(this.OnRegisterCallbackWithModel, this.TransitEvaluation));
            }
        }
    }
}
