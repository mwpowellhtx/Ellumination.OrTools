//using Ellumination.OrTools.ConstraintSolver.Routing.Distances;
//using Google.OrTools.ConstraintSolver;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Ellumination.OrTools.ConstraintSolver.Routing
//{
//    /// <summary>
//    /// Represents a BasicContext for unit test purposes.
//    /// </summary>
//    public class BasicContext : Context
//    {
//        /// <inheritdoc/>
//        internal BasicContext(int nodeCount, int vehicleCount)
//            : base(nodeCount, vehicleCount)
//        {
//        }

//        /// <inheritdoc/>
//        internal BasicContext(int nodeCount, int vehicleCount, int depot)
//            : base(nodeCount, vehicleCount, depot)
//        {
//        }

//        /// <inheritdoc/>
//        internal BasicContext(int nodeCount, int vehicleCount, int depot, RoutingModelParameters modelParameters = null)
//            : base(nodeCount, vehicleCount, depot, modelParameters)
//        {
//        }

//        /// <inheritdoc/>
//        internal BasicContext(int nodeCount, int vehicleCount, IEnumerable<int> starts, IEnumerable<int> ends)
//            : base(nodeCount, vehicleCount, starts, ends)
//        {
//        }

//        /// <inheritdoc/>
//        internal BasicContext(int nodeCount, int vehicleCount, IEnumerable<(int start, int end)> eps, RoutingModelParameters modelParameters = null)
//            : base(nodeCount, vehicleCount, eps, modelParameters)
//        {
//        }

//        /// <inheritdoc/>
//        internal BasicContext(int nodeCount, int vehicleCount, IEnumerable<int> starts, IEnumerable<int> ends, RoutingModelParameters modelParameters = null)
//            : base(nodeCount, vehicleCount, starts, ends, modelParameters)
//        {
//        }
//    }
//}
