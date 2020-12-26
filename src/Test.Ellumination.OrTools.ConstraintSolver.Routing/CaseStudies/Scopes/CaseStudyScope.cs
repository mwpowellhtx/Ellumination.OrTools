using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using static String;
    using static TestFixtureBase;

    /// <summary>
    /// Specifies certain bits that are common throughout the case studies.
    /// </summary>
    public abstract class CaseStudyScope : Disposable
    {
        /// <summary>
        /// Gets the Penalty associated with the scope, typically useful with Disjunction specs.
        /// Defaults to <c>null</c>.
        /// </summary>
        /// <value>null</value>
        internal virtual long? Penalty { get; }

        /// <summary>
        /// Gets the DimensionCoefficient, <c>100</c>.
        /// </summary>
        /// <value>100</value>
        internal virtual int DimensionCoefficient { get; } = 100;

        /// <summary>
        /// Gets the VehicleCap. Defaults to zero, <c>0</c>.
        /// </summary>
        /// <value>0</value>
        internal virtual long VehicleCap { get; }

        /// <summary>
        /// Gets the Maximum Slack.
        /// Defaults to <c>null</c>.
        /// </summary>
        internal virtual long? SlackMaximum { get; } = null;

        /// <summary>
        /// Gets the <see cref="SlackMaximum"/> or a <c>default</c> value.
        /// </summary>
        internal virtual long SlackMaximumOrDefault => this.SlackMaximum ?? default;

        /// <summary>
        /// Gets whether to Zero Accumulator when Dimension is registered.
        /// Defaults to <c>null</c>.
        /// </summary>
        internal virtual bool? ZeroAccumulator { get; } = null;

        /// <summary>
        /// Gets the <see cref="ZeroAccumulator"/> or a <c>default</c> value.
        /// </summary>
        internal virtual bool ZeroAccumulatorOrDefault => this.ZeroAccumulator ?? default;

        /// <summary>
        /// Gets the Depot, <c>default</c>.
        /// Assumes Depot is the default, or zero.
        /// </summary>
        internal virtual int Depot { get; } = default;

        /// <summary>
        /// Gets the VehicleCount, <c>1</c>.
        /// Assumes one single vehicle in the Case Study by default.
        /// </summary>
        /// <value>1</value>
        internal virtual int VehicleCount { get; } = 1;

        /// <summary>
        /// Gets a Range of Indices corresponding to the <see cref="VehicleCount"/>.
        /// </summary>
        internal IEnumerable<int> VehicleIndices => Range(0, this.VehicleCount);

        /// <summary>
        /// Gets the <see cref="Matrix"/> Values associated with the Case Study.
        /// </summary>
        protected abstract int?[,] MatrixValues { get; }

        private IList<long?> _routeDistances;
        private IList<IList<int>> _solutionPaths;

        /// <summary>
        /// Gets the RouteDistances for the Case Study, indexed by Vehicle.
        /// </summary>
        internal virtual IList<long?> RouteDistances => (this._routeDistances ?? (
            this._routeDistances = this.VehicleIndices.Select(_ => (long?)null).ToList()
        )).AssertNotNull().AssertEqual(this.VehicleCount, _ => _.Count);

        /// <summary>
        /// Gets a Default <see cref="SolutionPaths"/> Element.
        /// </summary>
        private static IList<int> DefaultSolutionPath => Range<int>().ToList();

        /// <summary>
        /// Gets the SolutionPaths.
        /// </summary>
        internal IList<IList<int>> SolutionPaths => (this._solutionPaths ?? (
            this._solutionPaths = this.VehicleIndices.Select(_ => DefaultSolutionPath).ToList()
        )).AssertNotNull().AssertEqual(this.VehicleCount, _ => _.Count());

        /// <summary>
        /// Gets the <see cref="ActualTotalDistance"/> Unit of Measure.
        /// </summary>
        internal abstract string DistanceUnit { get; }

        /// <summary>
        /// Gets or Sets the ActualTotalDistance for use throughout the tests.
        /// </summary>
        internal long ActualTotalDistance { get; set; } = default;

        /// <summary>
        /// Gets or Sets the ExpectedTotalDistance.
        /// </summary>
        internal virtual long ExpectedTotalDistance { get; set; }

        /// <summary>
        /// Gets a Default <see cref="ExpectedPaths"/> Element.
        /// </summary>
        private static IEnumerable<int> DefaultExpectedPath => Range<int>();

        private IList<int[]> _expectedPaths;

        /// <summary>
        /// Gets or Sets the ExpectedPaths.
        /// </summary>
        internal virtual IList<int[]> ExpectedPaths
        {
            get => this._expectedPaths ?? (this._expectedPaths
                = Range(0, this.VehicleCount).Select(_ => DefaultExpectedPath.ToArray()).ToList()
            );
            set => this._expectedPaths = value.OrEmpty().ToList();
        }

        /// <summary>
        /// Gets the OutputHelper.
        /// </summary>
        protected ITestOutputHelper OutputHelper { get; }

        /// <summary>
        /// Constructs the CaseStudyScope instance.
        /// </summary>
        /// <param name="outputHelper"></param>
        protected CaseStudyScope(ITestOutputHelper outputHelper)
        {
            this.OutputHelper = outputHelper;
        }

        /// <summary>
        /// Verifies and Reports the <see cref="SolutionPaths"/> and <see cref="ExpectedPaths"/>
        /// and related bits.
        /// </summary>
        internal void VerifyAndReportSolution()
        {
            this.ActualTotalDistance.AssertEqual(this.ExpectedTotalDistance);

            this.ExpectedPaths.AssertEqual(this.VehicleCount, x => x.Count);
            this.SolutionPaths.AssertEqual(this.ExpectedPaths.Count, x => x.Count);

            // Zipped for both Report and for Verification.
            var zipped = this.SolutionPaths.Zip(this.ExpectedPaths, (a, e, i) => (a: a.ToArray(), e, i)).ToList();

            void OnReportPaths((int[] a, int[] e, int i) tuple)
            {
                var (a, e, i, aligned) = (
                    tuple.a
                    , tuple.e
                    , tuple.i
                    , tuple.a.SequenceEqual(tuple.e)
                );

                const char comma = ',';
                var delim = $"\r\n    {comma} ";

                var rendered = RenderTupleAssociates(delim
                    , (nameof(a), Render(a))
                    , (nameof(e), Render(e))
                    , (nameof(i), Render(i))
                    , (nameof(aligned), Render(aligned))
                );

                const string squareBrackets = "[]";

                this.OutputHelper.WriteLine($"  {(i == 0 ? Empty : $"{comma} ")}"
                    + $"{Join($"{i}", squareBrackets.ToArray())} {rendered}");
            }

            var scopeType = this.GetType();

            this.OutputHelper.WriteLine($"{scopeType.FullName}.{nameof(this.SolutionPaths)}"
                + $".{nameof(Enumerable.Zip)}({scopeType.FullName}.{nameof(this.ExpectedPaths)}"
                + $", ...) = new[] {{");

            zipped.ForEach(OnReportPaths);

            this.OutputHelper.WriteLine($"}}");

            void OnVerifyPaths((int[] a, int[] e, int i) tuple)
            {
                var (a, e, i) = tuple;
                const int zero = default;
                i.AssertTrue(_ => _ >= zero);
                a.AssertEqual(e.Length, _ => _.Length);
                a.AssertCollectionEqual(e);
            }

            zipped.ForEach(OnVerifyPaths);
        }
    }
}
