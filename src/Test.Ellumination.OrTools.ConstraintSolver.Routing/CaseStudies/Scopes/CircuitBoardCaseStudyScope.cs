﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using DistanceMatrix = Distances.DistanceMatrix;
    using static TestFixtureBase;

    /// <inheritdoc/>
    public class CircuitBoardCaseStudyScope : CaseStudyScope<DistanceMatrix
        , RoutingContext, DefaultRoutingAssignmentEventArgs, DefaultRoutingProblemSolver>
    {
        /// <summary>
        /// Gets or Sets the ExpectedTotalDistance, <c>2790</c>.
        /// </summary>
        /// <value>2790</value>
        internal override long ExpectedTotalDistance { get; set; } = 2790;

        /// <summary>
        /// Gets or Sets the ExpectedPaths.
        /// </summary>
        internal override IList<int[]> ExpectedPaths { get; set; } = Range<int[]>(
            Range(0, 1, 279, 2, 278, 277, 247, 248, 249, 246, 244, 243, 242, 241, 240
                , 239, 238, 237, 236, 235, 234, 233, 232, 231, 230, 245, 250, 229, 228
                , 227, 226, 225, 224, 223, 222, 221, 220, 219, 218, 217, 216, 215, 214
                , 213, 212, 211, 210, 209, 208, 251, 254, 255, 257, 256, 253, 252, 207
                , 206, 205, 204, 203, 202, 142, 141, 146, 147, 140, 139, 265, 136, 137
                , 138, 148, 149, 177, 176, 175, 178, 179, 180, 181, 182, 183, 184, 186
                , 185, 192, 196, 197, 198, 144, 145, 143, 199, 201, 200, 195, 194, 193
                , 191, 190, 189, 188, 187, 163, 164, 165, 166, 167, 168, 169, 171, 170
                , 172, 105, 106, 104, 103, 107, 109, 110, 113, 114, 116, 117, 61, 62
                , 63, 65, 64, 84, 85, 115, 112, 86, 83, 82, 87, 111, 108, 89, 90, 91
                , 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 92, 79, 88, 81, 80, 78
                , 77, 76, 74, 75, 73, 72, 71, 70, 69, 66, 68, 67, 57, 56, 55, 54
                , 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 58, 60, 59, 42, 41
                , 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 124, 123, 122, 121
                , 120, 119, 118, 156, 157, 158, 173, 162, 161, 160, 174, 159, 150, 151
                , 155, 152, 154, 153, 128, 129, 130, 131, 18, 19, 20, 127, 126, 125, 28
                , 27, 26, 25, 21, 24, 22, 23, 13, 12, 14, 11, 10, 9, 7, 8, 6, 5
                , 275, 274, 273, 272, 271, 270, 15, 16, 17, 132, 133, 269, 268, 134
                , 135, 267, 266, 264, 263, 262, 261, 260, 258, 259, 276, 3, 4, 0).ToArray()
        ).ToList();

        /// <summary>
        /// The data for the problem consist of 280 points in the plane, shown in
        /// the scatter chart above. The program creates the data in an array of
        /// ordered pairs corresponding to the points in the plane, as shown below.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#data3"/>
        internal (int x, int y)[] CircuitBoardCoordinates { get; } = {
            (288, 149), (288, 129), (270, 133), (256, 141), (256, 157), (246, 157)
            , (236, 169), (228, 169), (228, 161), (220, 169), (212, 169), (204, 169)
            , (196, 169), (188, 169), (196, 161), (188, 145), (172, 145), (164, 145)
            , (156, 145), (148, 145), (140, 145), (148, 169), (164, 169), (172, 169)
            , (156, 169), (140, 169), (132, 169), (124, 169), (116, 161), (104, 153)
            , (104, 161), (104, 169), (90, 165), (80, 157), (64, 157), (64, 165)
            , (56, 169), (56, 161), (56, 153), (56, 145), (56, 137), (56, 129)
            , (56, 121), (40, 121), (40, 129), (40, 137), (40, 145), (40, 153)
            , (40, 161), (40, 169), (32, 169), (32, 161), (32, 153), (32, 145)
            , (32, 137), (32, 129), (32, 121), (32, 113), (40, 113), (56, 113)
            , (56, 105), (48, 99), (40, 99), (32, 97), (32, 89), (24, 89), (16, 97)
            , (16, 109), (8, 109), (8, 97), (8, 89), (8, 81), (8, 73), (8, 65)
            , (8, 57), (16, 57), (8, 49), (8, 41), (24, 45), (32, 41), (32, 49)
            , (32, 57), (32, 65), (32, 73), (32, 81), (40, 83), (40, 73), (40, 63)
            , (40, 51), (44, 43), (44, 35), (44, 27), (32, 25), (24, 25), (16, 25)
            , (16, 17), (24, 17), (32, 17), (44, 11), (56, 9), (56, 17), (56, 25)
            , (56, 33), (56, 41), (64, 41), (72, 41), (72, 49), (56, 49), (48, 51)
            , (56, 57), (56, 65), (48, 63), (48, 73), (56, 73), (56, 81), (48, 83)
            , (56, 89), (56, 97), (104, 97), (104, 105), (104, 113), (104, 121)
            , (104, 129), (104, 137), (104, 145), (116, 145), (124, 145), (132, 145)
            , (132, 137), (140, 137), (148, 137), (156, 137), (164, 137), (172, 125)
            , (172, 117), (172, 109), (172, 101), (172, 93), (172, 85), (180, 85)
            , (180, 77), (180, 69), (180, 61), (180, 53), (172, 53), (172, 61)
            , (172, 69), (172, 77), (164, 81), (148, 85), (124, 85), (124, 93)
            , (124, 109), (124, 125), (124, 117), (124, 101), (104, 89), (104, 81)
            , (104, 73), (104, 65), (104, 49), (104, 41), (104, 33), (104, 25)
            , (104, 17), (92, 9), (80, 9), (72, 9), (64, 21), (72, 25), (80, 25)
            , (80, 25), (80, 41), (88, 49), (104, 57), (124, 69), (124, 77), (132, 81)
            , (140, 65), (132, 61), (124, 61), (124, 53), (124, 45), (124, 37)
            , (124, 29), (132, 21), (124, 21), (120, 9), (128, 9), (136, 9), (148, 9)
            , (162, 9), (156, 25), (172, 21), (180, 21), (180, 29), (172, 29)
            , (172, 37), (172, 45), (180, 45), (180, 37), (188, 41), (196, 49)
            , (204, 57), (212, 65), (220, 73), (228, 69), (228, 77), (236, 77)
            , (236, 69), (236, 61), (228, 61), (228, 53), (236, 53), (236, 45)
            , (228, 45), (228, 37), (236, 37), (236, 29), (228, 29), (228, 21)
            , (236, 21), (252, 21), (260, 29), (260, 37), (260, 45), (260, 53)
            , (260, 61), (260, 69), (260, 77), (276, 77), (276, 69), (276, 61)
            , (276, 53), (284, 53), (284, 61), (284, 69), (284, 77), (284, 85)
            , (284, 93), (284, 101), (288, 109), (280, 109), (276, 101), (276, 93)
            , (276, 85), (268, 97), (260, 109), (252, 101), (260, 93), (260, 85)
            , (236, 85), (228, 85), (228, 93), (236, 93), (236, 101), (228, 101)
            , (228, 109), (228, 117), (228, 125), (220, 125), (212, 117), (204, 109)
            , (196, 101), (188, 93), (180, 93), (180, 101), (180, 109), (180, 117)
            , (180, 125), (196, 145), (204, 145), (212, 145), (220, 145), (228, 145)
            , (236, 145), (246, 141), (252, 125), (260, 129), (280, 133)
        };

        /// <summary>
        /// Returns the value <paramref name="x"/> Squared.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double Squared(double x) => Math.Pow(x, 2d);

        /// <summary>
        /// Returns the Square Root of <paramref name="d"/>.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static int Sqrt(double d) => (int)Math.Sqrt(d);

        /// <summary>
        /// Returns the Calculated Euclidian <see cref="Distances.Matrix.Values"/>
        /// of the <see cref="DistanceMatrix"/>.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        /// <see cref="!:https://en.wikipedia.org/wiki/Euclidean_distance"/>
        /// <see cref="!:https://en.wikipedia.org/wiki/Euclidean_distance#Two_dimensions"/>
        private static int?[,] CalculateEuclidianDistanceMatrix((int x, int y)[] coords)
        {
            var coords_Length = coords.Length;
            var matrixValues = new int?[coords_Length, coords_Length];

            // Calculate the distance matrix using two dimensional Euclidean distance.
            void OnCalculateDistance((int fromCoord, int toCoord) coord)
            {
                var (toCoordX, toCoordY) = coords[coord.toCoord];
                var (fromCoordX, fromCoordY) = coords[coord.fromCoord];

                matrixValues[coord.fromCoord, coord.toCoord]
                    = Sqrt(Squared(toCoordX - fromCoordX) + Squared(toCoordY - fromCoordY));
            }

            var dim = Enumerable.Range(0, coords_Length).ToArray();

            // Ignoring the matrix diagonal, which zeroing is handled by the matrix itself.
            dim.AsCoordinates().Where(coord => coord.x != coord.y).ToList().ForEach(OnCalculateDistance);

            return matrixValues;

        }

        private int?[,] _matrixValues;

        /// <inheritdoc/>
        protected override int?[,] MatrixValues => this._matrixValues ?? (
            this._matrixValues = CalculateEuclidianDistanceMatrix(this.CircuitBoardCoordinates)
        );

        /// <inheritdoc/>
        protected override DistanceMatrix CreateMatrix() => new DistanceMatrix(this.MatrixValues);

        /// <inheritdoc/>
        protected override RoutingContext CreateContext() => new RoutingContext(this.Matrix.Length, this.VehicleCount, this.Depot);

        /// <summary>
        /// Gets the Circuit Board Drill Press unit of measure, <c>&quot;m&quot;</c>.
        /// </summary>
        /// <value>m</value>
        internal override string DistanceUnit { get; } = "m";

        /// <inheritdoc/>
        public CircuitBoardCaseStudyScope(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        ///// <summary>
        ///// Event handler occurs On
        ///// <see cref="AssignableRoutingProblemSolver{TContext, TAssign}.Assign"/> event.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected override void OnProblemSolverAssign(object sender, DefaultRoutingAssignmentEventArgs e)
        //{
        //    var (vehicle, node, previousNode) = (e.AssertNotNull().VehicleIndex, e.NodeIndex, e.PreviousNodeIndex);
        //    // Vehicle should always be this.
        //    vehicle.AssertEqual(0);
        //    this.SolutionPaths[vehicle].AssertNotNull().Add(node);
        //    if (previousNode.HasValue)
        //    {
        //        this.ActualTotalDistance += (int)this.Context.GetArcCostForVehicle(previousNode ?? default, node, vehicle);
        //    }
        //    base.OnProblemSolverAssign(sender, e);
        //}

        // TODO: TBD: refactor verification to the tests...
        ///// <inheritdoc/>
        //protected override void OnVerifySolutionScope(CircuitBoardDrillPressCaseStudyScope scope)
        //{
        //    base.OnVerifySolutionScope(scope);
        //    /* https://developers.google.com/optimization/routing/tsp#print_solution2 */
        //    void OnReportTotalDistance(int totalDistance) =>
        //        this.OutputHelper.WriteLine($"Objective: {totalDistance} {this.DistanceUnit}");
        //    void OnReportEachVehiclePath((int vehicle, IEnumerable<int> path) item) =>
        //        this.OutputHelper.WriteLine(
        //            $"Route for vehicle {item.vehicle}: {string.Join(" -> ", item.path)}"
        //        );
        //    OnReportTotalDistance(this.ActualTotalDistance);
        //    Range(0, this.VehicleCount)
        //        .Select(_ => (_, (IEnumerable<int>)this.SolutionPaths[_]))
        //            .ToList().ForEach(OnReportEachVehiclePath);
        //}
    }
}
