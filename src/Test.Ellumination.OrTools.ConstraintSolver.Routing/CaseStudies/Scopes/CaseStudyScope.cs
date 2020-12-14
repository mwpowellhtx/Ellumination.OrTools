using System;
using System.Collections.Generic;
using System.Text;
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
        /// Gets the RouteDistances for the Case Study, indexed by Vehicle.
        /// </summary>
        internal virtual IDictionary<int, int?> RouteDistances { get; } = Range<int>().ToDictionary(x => x, _ => (int?)null);

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

        // TODO: TBD: potentially this should be in the base class...
        /// <summary>
        /// Gets the SolutionPath.
        /// </summary>
        internal IDictionary<int, ICollection<int>> SolutionPaths { get; } = new Dictionary<int, ICollection<int>>();

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
