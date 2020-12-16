﻿using System;
using System.Collections.Generic;
using System.Linq;

// TODO: TBD: setting up to do some constraint VRP, or CVRP ...
// TODO: TBD: with base class for the VRP, passing in appropriate Scope...
// TODO: TBD: also derived for the CVRP ...
// TODO: TBD: but before we can reasonably do that, we will refactor the scopes to first class file modules...
namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using DistanceMatrix = Distances.DistanceMatrix;
    using static VehicleRoutingCaseStudyScope;

    /// <summary>
    /// Verifies the initial Vehicle Routing Problem. We verify only the first of the
    /// problems, which does pass last time we checked. But moreover, also aligns with
    /// the <see cref="!:https://developers.google.com/optimization/routing/vrp">web
    /// site example</see>.
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/vrp"/>
    /// <remarks>Anything further than this, investigating third party distance matrix
    /// API, for instance, is beyond the scope of the unit test verification.</remarks>
    public abstract class VehicleRoutingCaseStudyTests<TScope> : OrToolsRoutingCaseStudyTests<DistanceMatrix
        , RoutingContext, DefaultRoutingAssignmentEventArgs, DefaultRoutingProblemSolver, TScope>
        where TScope : VehicleRoutingCaseStudyScope
    {
        protected VehicleRoutingCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        private class ArcCostDimension : Dimension
        {
            private DistanceMatrix Matrix { get; }

            public ArcCostDimension(RoutingContext context, DistanceMatrix matrix)
                : base(context, default)
            {
                this.Matrix = matrix;

                var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                this.SetArcCostEvaluators(evaluatorIndex);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromIndex"></param>
            /// <param name="toIndex"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(long fromIndex, long toIndex) => this.OnEvaluateTransit(
                this.Context.IndexToNode(fromIndex), this.Context.IndexToNode(toIndex)
            );
        }

        private class DistanceDimension : Dimension
        {
            /// <summary>
            /// Gets the DefaultCoefficient, defaults to <c>100</c>.
            /// </summary>
            private static int DefaultCoefficient { get; } = 100;

            private DistanceMatrix Matrix { get; }

            public DistanceDimension(RoutingContext context, DistanceMatrix matrix)
                : base(context, DefaultCoefficient)
            {
                this.Matrix = matrix;

                if (this.Coefficient > 0)
                {
                    var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                    this.AddDimension(evaluatorIndex, 3000);

                    var this_MutableDim = this.MutableDimension;

                    this_MutableDim.SetGlobalSpanCostCoefficient(this.Coefficient);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromIndex"></param>
            /// <param name="toIndex"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(long fromIndex, long toIndex) => this.OnEvaluateTransit(
                this.Context.IndexToNode(fromIndex), this.Context.IndexToNode(toIndex)
            );
        }

        /// <inheritdoc/>
        protected override TScope VerifyScope(TScope scope)
        {
            scope = base.VerifyScope(scope);

            scope.TotalDistance.AssertEqual(default);

            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Width);
            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Height);

            scope.Context.AssertEqual(scope.VehicleCount, x => x.VehicleCount);

            return scope;
        }

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what we expected.
        /// We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/vrp#solution"/>
        protected override void OnVerifySolution(TScope scope)
        {
            void OnVerifyExpectedPath((int[] expectedPath, int vehicle) tuple)
            {
                var (expectedPath, vehicle) = tuple;

                var actualPath = scope.SolutionPaths[vehicle];

                actualPath.AssertCollectionEqual(expectedPath);
            }

            scope.ExpectedPaths.Select((x, i) => (x, i)).ToList().ForEach(OnVerifyExpectedPath);
        }

        /// <summary>
        /// Arranges for the Vehicle Routing Case Study scenarios to be performed.
        /// </summary>
        [Background]
        public void VehicleRoutingBackground()
        {
            $"Add {nameof(ArcCostDimension)} to this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.Context.AddDefaultDimension<ArcCostDimension>(this.Scope.Matrix)
            );

            $"Add {nameof(DistanceDimension)} to this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.Context.AddDefaultDimension<DistanceDimension>(this.Scope.Matrix)
            );
        }
    }
}
