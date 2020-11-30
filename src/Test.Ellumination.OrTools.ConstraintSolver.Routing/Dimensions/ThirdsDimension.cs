using System;
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
    public class ThirdsDimension : Dimension
    {
        /// <summary>
        /// Gets the Calculator for internal use.
        /// </summary>
        private FibonacciCalculator Calculator { get; } = new FibonacciCalculator();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="toNode"></param>
        /// <returns></returns>
        private long OnEvaluateEven(int fromNode, int toNode)
        {
            int GetCalculatedCost(int index) => index == 0 ? default : this.Calculator[index];
            var (fromCost, toCost) = (GetCalculatedCost(fromNode), GetCalculatedCost(toNode));

            //// Notwithstanding disjunctions prohibiting even or odd.
            //var result = Math.Min(1, Math.Abs(fromCost - toCost));

            //var isDepot = this.Context.DepotCoordinates.Contains(toNode);

            //var result = isDepot ? default : Math.Abs(fromCost - toCost);

            //return result;

            return Math.Abs(fromCost - toCost);
        }

        /// <summary>
        /// Callback that Evaluates whether the Node is Even.
        /// </summary>
        /// <param name="fromIndex"></param>
        /// <param name="toIndex"></param>
        /// <returns></returns>
        private long OnEvaluateEven(long fromIndex, long toIndex) => this.OnEvaluateEven(
            this.Context.IndexToNode(fromIndex)
            , this.Context.IndexToNode(toIndex)
        );

        /// <summary>
        /// Constructs the Dimension with <paramref name="context"/> and
        /// <paramref name="coefficient"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="coefficient"></param>
        public ThirdsDimension(RoutingContext context, int coefficient)
            : base(context, coefficient)
        {
            if (coefficient > 0)
            {
                //// TODO: TBD: unary might not be evaluating quite like we expect...
                //var evaluatorIndex = this.RegisterUnaryTransitEvaluationCallback(this.OnEvaluateEven);

                //// TODO: TBD: tried different approaches adding dimension to the model to no avail...
                // Estimate Vehicle Capacity given calculated weights averaged by Vehicle Count.
                long nodeCalculatorTotal = this.Calculator.Expand(this.Context.NodeCount).Sum();

                /* Have tried, with seemingly no success, to adjust capacity and so forth, but could not land on an assignment solution, apparently. Unclear at this point how to get a status when that happens. */
                var estimatedCap = nodeCalculatorTotal; // / this.Context.VehicleCount;
                //var estimatedCap = (long)this.Calculator.Expand(this.Context.NodeCount).Sum() / this.Context.VehicleCount;

                /* Based on: https://developers.google.com/optimization/routing/penalties
                 * Unary would probably work as well, but we do go as far as to do a delta calc. */
                var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateEven);

                //// TODO: TBD: not sure arc cost eval is even necessary...
                //this.SetArcCostEvaluators(evaluatorIndex);

                //// Cap literally count of nodes, allow odd scheduling, and divide among the vehicles.
                //var estimatedCap = (this.Context.NodeCount / 2L) / this.Context.VehicleCount;

                var estimatedCaps = Enumerable.Range(0, this.Context.VehicleCount).Select(_ => estimatedCap).ToArray();
                this.AddDimension(evaluatorIndex, estimatedCaps);

                //this.AddDimension(evaluatorIndex, estimatedCap);

                // TODO: TBD: not sure, either approach should work? whether RDorD versus MD?
                this.RoutingDimensionOrDie.SetGlobalSpanCostCoefficient(coefficient);
                //this.MutableDimension.SetGlobalSpanCostCoefficient(coefficient);

#if false // TODO: TBD: tried to add a disjunction, but did not land in a solution...
                //const long evenPenalty = 100;

                ///* Based on: https://developers.google.com/optimization/routing/penalties
                // * NodeToIndex and IndexToNode are key, key, KEY. Cannot overstate this enough. */
                //var oddIndices = this.NodesToIndices(Enumerable.Range(0, this.Context.NodeCount)
                //    .Where(x => x != 0 && x % 2 == 1).ToArray()).ToArray();

                ///* Which, we think, should prohibit scheduling Odd nodes.
                // * https://developers.google.com/optimization/reference/constraint_solver/routing/RoutingModel */
                //oddIndices.Select(x => new long[] { x }).ToList().ForEach(
                //    y => this.AddDisjunction(evenPenalty, y));

                //// Disjunction should be prohibiting odd number scheduling.
                //this.AddDisjunction(evenPenalty, oddIndices);
#endif

#if true // TODO: TBD: whereas "this" approach seemed to "work", but it also seems like a brute force way of describing the problem
                // Sets allowed vehicles for each node.
                var vehicleCount = this.Context.VehicleCount;
                var nodeCount = this.Context.NodeCount;

                // Confirmed this does have the effect we think it should have on solution outcomes.
                for (var vehicleIndex = 0; vehicleIndex < vehicleCount; vehicleIndex++)
                {
                    void OnSetAllowedVehiclesForNode(int nodeIndex) =>
                        this.SetAllowedVehiclesForNode(nodeIndex, vehicleIndex);

                    Enumerable.Range(0, nodeCount)
                        .Where(x => x > 0 && x % vehicleCount == vehicleIndex)
                        .ToList().ForEach(OnSetAllowedVehiclesForNode);
                }
#endif

            }
        }

        //// TODO: TBD: yes, initially probed a unary transit callback...
        //// TODO: TBD: however, I think we can leverage the binary callback instead...
        ///// <summary>
        ///// Handles the actual <see cref="TransitEvaluation"/> Callback. Assuming lower
        ///// arc costs win, allows Transit assuming a Zed Depot, and for Even Stephen nodes.
        ///// Should prohibit transit involving all other nodes.
        ///// </summary>
        ///// <param name="index"></param>
        ///// <returns></returns>
        //// TODO: TBD: from "zero cost" to "prohibit cost"...
        //// TODO: TBD: added Fibonacci calculator for purposes of informing the unary transit costs for sake of example...
        //// TODO: TBD: we think that perhaps a "disjunction" might be required in order to arrive at "even scheduling only" outcome...
        //// TODO: TBD: see: https://developers.google.com/optimization/routing/penalties
        //private long OnTransitEvaluation(long index)
        //{
        //    // TODO: TBD: This is how we make the adjustment from callback index to node...
        //    var i = this.Context.Manager.IndexToNode(index);
        //    return this.Calculator[i];
        //}
    }
}
