using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using OnTransitEvaluationCallback = Google.OrTools.ConstraintSolver.LongToLong;

    // TODO: TBD: we think that this is probably a naive over simplified example...
    // TODO: TBD: maybe there is a better use case example that we could unit test...
    // https://developers.google.com/optimization/routing/vrp
    // TODO: TBD: might look to "disjunctions" as a possible approach...
    // TODO: TBD: although it is not "purely" a cost model then, is it?
    // https://developers.google.com/optimization/routing/penalties
    // https://github.com/google/or-tools/tree/stable/ortools/constraint_solver
    // https://github.com/google/or-tools/blob/stable/ortools/constraint_solver/routing_flags.h
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="!:https://github.com/google/or-tools/blob/stable/ortools/constraint_solver/routing_flags.cc#L137"/>
    public class DefaultEvenDimension : DefaultUnaryDimension
    {
        public DefaultEvenDimension(Context context, int coefficient)
            : base(context, coefficient)
        {
            if (coefficient > 0)
            {
                var callbackIndex = this.RegisteredCallbackIndexes.Last();

                // Estimate Vehicle Capacity given calculated weights averaged by Vehicle Count.
                var estimatedCap = this.Calculator.Expand(this.Context.NodeCount).Sum();

                this.Context.Model.AddDimension(callbackIndex, 0, estimatedCap, true, this.Name);

                this.RoutingDimensionOrDie.SetGlobalSpanCostCoefficient(coefficient);

                const long evenPenalty = 10000;

                // Which, we think, should prohibit scheduling Odd nodes.
                var odds = Enumerable.Range(0, this.Context.NodeCount).Where(x => x != 0 && x % 2 != 0).Select(x => (long)x).ToArray();

                this.Context.Model.AddDisjunction(odds, evenPenalty);
            }
        }

        /// <inheritdoc/>
        protected override OnTransitEvaluationCallback TransitEvaluation => this.OnTransitEvaluation;

        /// <summary>
        /// Gets the Calculator for internal use.
        /// </summary>
        private FibonacciCalculator Calculator { get; } = new FibonacciCalculator();

        /// <summary>
        /// Handles the actual <see cref="TransitEvaluation"/> Callback. Assuming lower
        /// arc costs win, allows Transit assuming a Zed Depot, and for Even Stephen nodes.
        /// Should prohibit transit involving all other nodes.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        // TODO: TBD: from "zero cost" to "prohibit cost"...
        // TODO: TBD: added Fibonacci calculator for purposes of informing the unary transit costs for sake of example...
        // TODO: TBD: we think that perhaps a "disjunction" might be required in order to arrive at "even scheduling only" outcome...
        // TODO: TBD: see: https://developers.google.com/optimization/routing/penalties
        private long OnTransitEvaluation(long index)
        {
            // TODO: TBD: This is how we make the adjustment from callback index to node...
            var i = this.Context.Manager.IndexToNode(index);
            return this.Calculator[i];
        }
    }
}
