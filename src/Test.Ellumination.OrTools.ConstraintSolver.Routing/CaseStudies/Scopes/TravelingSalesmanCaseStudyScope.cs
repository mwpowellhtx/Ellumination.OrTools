using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using DistanceMatrix = Distances.DistanceMatrix;
    using static TestFixtureBase;

    /// <inheritdoc/>
    public class TravelingSalesmanCaseStudyScope : CaseStudyScope<DistanceMatrix
        , RoutingContext, DefaultRoutingAssignmentEventArgs, DefaultRoutingProblemSolver>
    {
        /// <summary>
        /// Gets the Traveling Salesman Distance Unit, &quot;mi&quot;.
        /// </summary>
        protected override string DistanceUnit { get; } = "mi";

        public TravelingSalesmanCaseStudyScope(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Gets the MatrixValues for use with the <see cref="Matrix"/> property.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#dist_matrix_data"/>
        protected override int?[,] MatrixValues { get; } = {
                {0, 2451, 713, 1018, 1631, 1374, 2408, 213, 2571, 875, 1420, 2145, 1972}
                , {2451, 0, 1745, 1524, 831, 1240, 959, 2596, 403, 1589, 1374, 357, 579}
                , {713, 1745, 0, 355, 920, 803, 1737, 851, 1858, 262, 940, 1453, 1260}
                , {1018, 1524, 355, 0, 700, 862, 1395, 1123, 1584, 466, 1056, 1280, 987}
                , {1631, 831, 920, 700, 0, 663, 1021, 1769, 949, 796, 879, 586, 371}
                , {1374, 1240, 803, 862, 663, 0, 1681, 1551, 1765, 547, 225, 887, 999}
                , {2408, 959, 1737, 1395, 1021, 1681, 0, 2493, 678, 1724, 1891, 1114, 701}
                , {213, 2596, 851, 1123, 1769, 1551, 2493, 0, 2699, 1038, 1605, 2300, 2099}
                , {2571, 403, 1858, 1584, 949, 1765, 678, 2699, 0, 1744, 1645, 653, 600}
                , {875, 1589, 262, 466, 796, 547, 1724, 1038, 1744, 0, 679, 1272, 1162}
                , {1420, 1374, 940, 1056, 879, 225, 1891, 1605, 1645, 679, 0, 1017, 1200}
                , {2145, 357, 1453, 1280, 586, 887, 1114, 2300, 653, 1272, 1017, 0, 504}
                , {1972, 579, 1260, 987, 371, 999, 701, 2099, 600, 1162, 1200, 504, 0}
            };

        private DistanceMatrix _matrix;

        private RoutingContext _context;

        /// <inheritdoc/>
        internal override DistanceMatrix Matrix => this._matrix ?? (this._matrix = new DistanceMatrix(this.MatrixValues));

        /// <inheritdoc/>
        internal override RoutingContext Context => this._context ?? (
            this._context = new RoutingContext(this.Matrix.Width, 1)
            );

        /// <summary>
        /// Event handler occurs On
        /// <see cref="AssignableRoutingProblemSolver{TContext, TAssign}.Assign"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnProblemSolverAssign(object sender, DefaultRoutingAssignmentEventArgs e)
        {
            var (vehicle, node, previousNode) = (e.AssertNotNull().VehicleIndex, e.NodeIndex, e.PreviousNodeIndex);

            // Vehicle should always be this.
            vehicle.AssertEqual(0);

            this.SolutionPaths[vehicle] = this.SolutionPaths.TryGetValue(vehicle, out var path)
                ? path
                : new List<int>();

            this.SolutionPaths[vehicle].AssertNotNull().Add(node);

            if (previousNode.HasValue)
            {
                this.TotalDistance += this.Matrix[previousNode.Value, node] ?? default;
            }

            base.OnProblemSolverAssign(sender, e);
        }

        /// <summary>
        /// Verifies the Solution vis a vis the TSP solution.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#solution1"/>
        internal override void VerifySolution()
        {
            base.VerifySolution();

            this.SolutionPaths.AssertEqual(1, x => x.Count);

            // TODO: TBD: which we could probably refactor the "0" vehicle, to "all" vehicles, etc...
            if (this.SolutionPaths.TryGetValue(0, out var actualPath).AssertTrue())
            {
                // Pretty much verbatim, https://developers.google.com/optimization/routing/tsp#solution1.
                this.AssertEqual(7293, x => x.TotalDistance);

                ICollection<int> expectedPath = Range(0, 7, 2, 3, 4, 12, 6, 8, 1, 11, 10, 5, 9, 0).ToList();

                actualPath.AssertCollectionEqual(expectedPath);
            }

            /* See: https://developers.google.com/optimization/routing/tsp#printer, which
             * we should have the solution in hand approaching test disposal. */

            void OnReportTotalDistance(int totalDistance) =>
                this.OutputHelper.WriteLine($"Objective: {totalDistance} {this.DistanceUnit}");

            void OnReportEachVehiclePath((int vehicle, IEnumerable<int> path) item) =>
                this.OutputHelper.WriteLine(
                    $"Route for vehicle {item.vehicle}: {string.Join(" , ", item.path)}"
                );

            OnReportTotalDistance(this.TotalDistance);

            this.SolutionPaths.Keys.OrderBy(key => key)
                .Select(key => (key, (IEnumerable<int>)this.SolutionPaths[key]))
                    .ToList().ForEach(OnReportEachVehiclePath);
        }
    }
}
