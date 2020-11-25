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

                this.Context.Model.AddDimension(callbackIndex, 0, 3, true, this.Name);

                this.RoutingDimensionOrDie.SetGlobalSpanCostCoefficient(coefficient);
            }
        }

        /// <inheritdoc/>
        protected override OnTransitEvaluationCallback TransitEvaluation => this.OnTransitEvaluation;

        /// <summary>
        /// Handles the actual <see cref="TransitEvaluation"/> Callback. Assuming lower
        /// arc costs win, allows Transit assuming a Zed Depot, and for Even Stephen nodes.
        /// Should prohibit transit involving all other nodes.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        // TODO: TBD: from "zero cost" to "prohibit cost"...
        private long OnTransitEvaluation(long index) => index == 0 ? 0 : index % 2 == 0 ? 0 : 100;
        //                                                                                    ^^^
    }
}
