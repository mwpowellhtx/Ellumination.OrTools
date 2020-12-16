using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using static TestFixtureBase;

    /// <summary>
    /// Specifies certain bits that are common throughout the case studies.
    /// </summary>
    public abstract class CaseStudyScope : IDisposable
    {
        /// <summary>
        /// Gets or Sets the TotalDistance for use throughout the tests.
        /// </summary>
        internal int TotalDistance { get; set; } = default;

        /// <summary>
        /// Gets the VehicleCount.
        /// </summary>
        internal virtual int VehicleCount { get; } = default;

        private IList<int?> _routeDistances;

        /// <summary>
        /// Gets the RouteDistances for the Case Study, indexed by Vehicle.
        /// </summary>
        internal virtual IList<int?> RouteDistances => (this._routeDistances ?? (
            this._routeDistances = Range(0, this.VehicleCount).Select(_ => (int?)null).ToList()
        )).AssertNotNull().AssertEqual(this.VehicleCount, _ => _.Count);

        /// <summary>
        /// Gets the <see cref="TotalDistance"/> Unit of Measure.
        /// </summary>
        protected abstract string DistanceUnit { get; }

        /// <summary>
        /// Gets the OutputHelper.
        /// </summary>
        protected ITestOutputHelper OutputHelper { get; }

        protected CaseStudyScope(ITestOutputHelper outputHelper)
        {
            this.OutputHelper = outputHelper;
        }

        /// <summary>
        /// Gets the <see cref="Matrix"/> Values associated with the Case Study.
        /// </summary>
        protected abstract int?[,] MatrixValues { get; }

        /// <summary>
        /// 
        /// </summary>
        private static IList<int> DefaultSolutionPath => Range<int>().ToList();

        private IList<IList<int>> _solutionPaths;

        /// <summary>
        /// Gets the SolutionPaths.
        /// </summary>
        internal IList<IList<int>> SolutionPaths => (this._solutionPaths ?? (
            this._solutionPaths = Range(0, this.VehicleCount).Select(_ => DefaultSolutionPath).ToList()
        )).AssertNotNull().AssertEqual(this.VehicleCount, _ => _.Count());

        /// <summary>
        /// Event handler occurs On
        /// <see cref="AssignableRoutingProblemSolver{TContext, TAssign}.Assign"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProblemSolverAssign(object sender, DefaultRoutingAssignmentEventArgs e)
        {
        }

        /// <summary>
        /// Override in order to Verify the Solution.
        /// </summary>
        internal virtual void VerifySolution()
        {
        }

        /// <summary>
        /// Event occurs On <see cref="Dispose(bool)"/>.
        /// </summary>
        internal event EventHandler<EventArgs> Disposing;

        /// <summary>
        /// Callback invokes the <see cref="Disposing"/> event.
        /// </summary>
        protected virtual void OnDisposing() => this.Disposing?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Gets whether the object isDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Override in order to dispose the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this.OnDisposing();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            this.IsDisposed = true;
        }
    }
}
