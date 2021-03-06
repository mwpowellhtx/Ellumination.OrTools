﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using DistanceMatrix = Distances.DistanceMatrix;

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
            private TScope Scope { get; }

            private DistanceMatrix Matrix => this.Scope.Matrix;

            public ArcCostDimension(RoutingContext context, TScope scope)
                : base(context, scope.DimensionCoefficient)
            {
                this.Scope = scope;

                if (this.Coefficient > 0)
                {
                    var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                    this.SetArcCostEvaluators(evaluatorIndex);
                }
            }

            /// <summary>
            /// Returns the result after Evaluating the <see cref="Matrix"/> Transit given
            /// <paramref name="fromNode"/> and <paramref name="toNode"/>.
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            protected override long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default;
        }

        /// <summary>
        /// Establishes the basic DistanceDimension given <typeparamref name="TScope"/>.
        /// </summary>
        private class NodeDemandDimension : Dimension
        {
            private TScope Scope { get; }

            private DistanceMatrix Matrix => this.Scope?.Matrix;

            public NodeDemandDimension(RoutingContext context, TScope scope)
                : base(context, scope.DimensionCoefficient)
            {
                this.Scope = scope;

                if (this.Coefficient > 0)
                {
                    var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                    this.AddDimension(evaluatorIndex, this.Scope.VehicleCap);

                    // ? 
                    //this.MutableDimension.SetGlobalSpanCostCoefficient(this.Coefficient);
                }
            }

            /// <summary>
            /// Returns the result after Evaluating the <see cref="Matrix"/> Transit given
            /// <paramref name="fromNode"/> and <paramref name="toNode"/>.
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            protected override long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default;
        }

        /// <inheritdoc/>
        protected override TScope OnVerifyInitialScope(TScope scope)
        {
            scope = base.OnVerifyInitialScope(scope);

            scope.ActualTotalDistance.AssertEqual(default);

            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Width);
            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Height);

            scope.Context.AssertEqual(scope.VehicleCount, x => x.VehicleCount);

            return scope;
        }

        /// <summary>
        /// Arranges for the VRP Case Study scenarios to be performed.
        /// </summary>
        /// <param name="scope"></param>
        [Background]
        public void VehicleRoutingBackground(TScope scope)
        {
            $"Verify {nameof(scope)}".x(() => scope = this.Scope.AssertNotNull());

            $"Add {nameof(ArcCostDimension)} dimension to {nameof(scope)}.{nameof(scope.Context)}".x(
                () => scope.Context.AddDefaultDimension<ArcCostDimension>(scope)
            );

            $"Add {nameof(NodeDemandDimension)} dimension to {nameof(scope)}.{nameof(scope.Context)}".x(
                () => scope.Context.AddDefaultDimension<NodeDemandDimension>(scope)
            );
        }

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what
        /// we expected. We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/vrp#solution"/>
        protected override void OnVerifySolutionScope(TScope scope)
        {
            base.OnVerifySolutionScope(scope);

            void OnVerifyExpectedPath((int vehicle, int[] expectedPath) tuple) =>
                scope.SolutionPaths.ElementAt(tuple.vehicle).AssertNotNull()
                    .AssertCollectionEqual(tuple.expectedPath);

            scope.ExpectedPaths.AssertEqual(scope.VehicleCount, x => x.Count)
                .Select((x, i) => (i, x))
                .ToList().ForEach(OnVerifyExpectedPath);
        }
    }
}
