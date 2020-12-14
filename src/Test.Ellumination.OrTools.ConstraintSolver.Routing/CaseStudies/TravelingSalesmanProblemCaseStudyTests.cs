﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    // TODO: TBD: ditto DistanceMatrix refactoring to collections suite...
    using DistanceMatrix = Distances.DistanceMatrix;

    /// <summary>
    /// Tests involving a <see cref="DefaultRoutingProblemSolver"/>. We do not care about <em>Location</em>, per se, apart from documentation purposes. In a real world scenario, we might promote that concern to an actual <see cref="Distances.LocationDistanceMatrix"/>, however.
    /// 
    /// <para>Our model represents the distances using the following indices, <c>0</c> being the <em>depot</em>.</para>
    /// 
    /// <para><list type="table">
    /// <listheader>
    /// <node>Node</node>
    /// <location>Location</location>
    /// <depot>Depot (default: <c>false</c>)</depot>
    /// </listheader>
    /// <item>
    /// <node>0</node>
    /// <location>New York</location>
    /// <depot>true</depot>
    /// </item>
    /// <item>
    /// <node>1</node>
    /// <location>Los Angeles</location>
    /// </item>
    /// <item>
    /// <node>2</node>
    /// <location>Chicago</location>
    /// </item>
    /// <item>
    /// <node>3</node>
    /// <location>Minneapolis</location>
    /// </item>
    /// <item>
    /// <node>4</node>
    /// <location>Denver</location>
    /// </item>
    /// <item>
    /// <node>5</node>
    /// <location>Dallas</location>
    /// </item>
    /// <item>
    /// <node>6</node>
    /// <location>Seattle</location>
    /// </item>
    /// <item>
    /// <node>7</node>
    /// <location>Boston</location>
    /// </item>
    /// <item>
    /// <node>8</node>
    /// <location>San Francisco</location>
    /// </item>
    /// <item>
    /// <node>9</node>
    /// <location>St. Louis</location>
    /// </item>
    /// <item>
    /// <node>10</node>
    /// <location>Houston</location>
    /// </item>
    /// <item>
    /// <node>11</node>
    /// <location>Phoenix</location>
    /// </item>
    /// <item>
    /// <node>12</node>
    /// <location>Salt Lake City</location>
    /// </item>
    /// </list></para>
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/tsp"/>
    public class TravelingSalesmanProblemCaseStudyTests : OrToolsRoutingCaseStudyTests<DistanceMatrix, RoutingContext
        , DefaultRoutingAssignmentEventArgs, DefaultRoutingProblemSolver, TravelingSalesmanCaseStudyScope>
    {
        public TravelingSalesmanProblemCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <inheritdoc/>
        protected override TravelingSalesmanCaseStudyScope VerifyScope(TravelingSalesmanCaseStudyScope scope)
        {
            scope = base.VerifyScope(scope);

            scope.TotalDistance.AssertEqual(default);

            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Width);
            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Height);

            scope.Context.AssertEqual(1, x => x.VehicleCount);

            scope.SolutionPaths.AssertNotNull().AssertCollectionEmpty();

            return scope;
        }

        /// <summary>
        /// Dimension for private usage.
        /// </summary>
        public class TestDimension : Dimension
        {
            /// <summary>
            /// Gets the Matrix for use throughout the Dimension.
            /// </summary>
            private DistanceMatrix Matrix { get; }

            /// <summary>
            /// Constructs a test dimension.
            /// </summary>
            /// <param name="context"></param>
            /// <param name="matrix"></param>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#arc-cost"/>
            public TestDimension(RoutingContext context, DistanceMatrix matrix)
                : base(context, 100)
            {
                this.Matrix = matrix;

                /* See: https://developers.google.com/optimization/routing/tsp#dist_callback1, the
                 * details of which are also abstracted behind Dimension, etc. */
                var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                /* See: https://developers.google.com/optimization/routing/tsp#arc-cost, the
                 * details of which are purposefully masked by the Dimension implementation. */

                this.SetArcCostEvaluators(evaluatorIndex);
            }

            /// <summary>
            /// Evaluates the Transit between <paramref name="fromNode"/> and
            /// <paramref name="toNode"/>.
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default(long);

            /// <summary>
            /// Evaluates the Transit between <paramref name="fromIndex"/> and
            /// <paramref name="toIndex"/>.
            /// </summary>
            /// <param name="fromIndex"></param>
            /// <param name="toIndex"></param>
            /// <returns></returns>
            /// <see cref="!:https://developers.google.com/optimization/routing/tsp#dist_callback1"/>
            private long OnEvaluateTransit(long fromIndex, long toIndex) => this.OnEvaluateTransit(
                this.Context.IndexToNode(fromIndex)
                , this.Context.IndexToNode(toIndex)
            );
        }

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what we expected.
        /// We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#solution1"/>
        protected override void OnVerifySolution(TravelingSalesmanCaseStudyScope scope)
        {
            const int expectedVehicle = 0;
            scope.SolutionPaths.AssertEqual(1, x => x.Count);
            scope.SolutionPaths.AssertTrue(x => x.ContainsKey(expectedVehicle));
            var expectedPath = Range(0, 7, 2, 3, 4, 12, 6, 8, 1, 11, 10, 5, 9, 0);
            scope.SolutionPaths.AssertEqual(expectedPath, x => x[0]);
        }

        // TODO: TBD: okay, so test "works", output can be verified against expected outcomes...
        // TODO: TBD: the only question is in terms of appropriate factoring...
        // TODO: TBD: would like to be able to rinse and repeat with minimal additional effort...
        // TODO: TBD: bits that need repeating:
        // 1. matrix/values
        // 2. vehicle(s)
        // 3. depot(s)
        // 4. objective value(s)
        // 5. and/or solution accumulator(s)
        // 6. 
        // 7. 
        // 8. 
        /// <summary>
        /// Arranges for the Traveling Salesman Case Study scenarios to be performed.
        /// </summary>
        [Background]
        public void TravelingSalesmanBackground()
        {
            $"Add {nameof(TestDimension)} to this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.Context.AddDefaultDimension<TestDimension>(this.Scope.Matrix)
            );
        }

        // TODO: TBD: assuming that each of the case studies resolve in a sincle "verification" scenario...
        // TODO: TBD: then likely all we need to do is provide that single method...
        /// <summary>
        /// Demonstrates a scenario in which the Solution Agrees with the Optimization Routing
        /// Traveling Salesman Problem illustration.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp"/>
        [Scenario]
        public void Verify_ProblemSolver_Solution()
        {
            /* See: https://developers.google.com/optimization/routing/tsp#main, through which
             * we abstract these are the default solver options, i.e. PATH_CHEAPEST_ARC, etc. */

            // TODO: TBD: this is starting to look like really boilerplate stuff as well...
            $"Solve routing problem given this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.ProblemSolver.Solve(this.Scope.Context)
            );

            void OnVerifySolution() => this.OnVerifySolution(this.Scope);

            $"Verify solution".x(OnVerifySolution);
        }
    }
}
