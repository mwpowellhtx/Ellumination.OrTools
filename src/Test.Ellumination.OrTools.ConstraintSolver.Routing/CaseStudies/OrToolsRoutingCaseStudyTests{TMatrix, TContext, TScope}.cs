using System;
using System.Collections.Generic;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    /// <summary>
    /// Override in order to specify further bits concerning the Case Study.
    /// </summary>
    /// <typeparam name="TMatrix"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TScope"></typeparam>
    public abstract class OrToolsRoutingCaseStudyTests<TMatrix, TContext, TScope> : TestFixtureBase
        where TMatrix : Distances.Matrix
        where TContext : Context
        where TScope : OrToolsRoutingCaseStudyTests<TMatrix, TContext, TScope>.CaseStudyScope
    {
        protected OrToolsRoutingCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

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
            /// Gets the Matrix associated with the Case Study.
            /// </summary>
            internal abstract TMatrix Matrix { get; }

            /// <summary>
            /// Gets the Context associated with the Case Study.
            /// </summary>
            internal abstract TContext Context { get; }

            protected virtual DefaultRoutingProblemSolver CreateProblemSolver()
            {
                var problemSolver = new DefaultRoutingProblemSolver();
                problemSolver.Assign += this.OnProblemSolverAssign;
                return problemSolver;
            }

            private DefaultRoutingProblemSolver _problemSolver;

            // TODO: TBD: may be able to install this in the base class..
            /// <summary>
            /// Gets or Sets the ProblemSolver.
            /// </summary>
            internal DefaultRoutingProblemSolver ProblemSolver => this._problemSolver ?? (
                this._problemSolver = CreateProblemSolver()
            );

            // TODO: TBD: potentially this should be in the base class...
            /// <summary>
            /// Gets the SolutionPath.
            /// </summary>
            internal IDictionary<int, ICollection<int>> SolutionPaths { get; }
                = new Dictionary<int, ICollection<int>>();

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

                    void OnDisposeProblemSolver(DefaultRoutingProblemSolver problemSolver)
                    {
                        if (problemSolver != null)
                        {
                            this.ProblemSolver.Assign -= this.OnProblemSolverAssign;
                            this.ProblemSolver.Dispose();
                        }
                    }

                    this.Context?.Dispose();
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                this.Dispose(true);
                this.IsDisposed = true;
            }
        }

        private TScope _scope;

        /// <summary>
        /// Gets the Scope for use throughout the unit tests.
        /// </summary>
        protected virtual TScope Scope
        {
            get => this._scope;
            private set => this._scope = value;
        }

        /// <summary>
        /// Override in order to Initialize the <paramref name="scope"/>.
        /// </summary>
        /// <param name="scope"></param>
        protected virtual void InitializeScope(out TScope scope) =>
            scope = Activator.CreateInstance(typeof(TScope), this.OutputHelper).AssertIsType<TScope>();

        /// <summary>
        /// Override in order to <see cref="VerifyScope"/> in a more specialized manner.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        protected virtual TScope VerifyScope(TScope scope)
        {
            scope.AssertNotNull();
            scope.Context.AssertNotNull();
            scope.Matrix.AssertNotNull();
            scope.ProblemSolver.AssertNotNull();
            return scope;
        }

        /// <summary>
        /// Override in order to Verify the <paramref name="scope"/> Solution.
        /// </summary>
        /// <param name="scope"></param>
        protected virtual void OnVerifySolution(TScope scope)
        {
        }

        /// <summary>
        /// Performs some Background arrangement concerning the Case Study.
        /// </summary>
        [Background]
        public void CaseStudyBackground()
        {
            void OnInitializeScope()
            {
                this.InitializeScope(out var scope);
                this.Scope = this.VerifyScope(scope);
            }

            $"Initialize this.{nameof(this.Scope)}".x(OnInitializeScope);
        }

        /// <summary>
        /// Event handler occurs On <paramref name="scope"/> Tear Down.
        /// </summary>
        /// <param name="scope"></param>
        protected virtual void OnScopeTearDown(ref TScope scope)
        {
            if (scope != null)
            {
                scope?.Dispose();
                scope = null;
            }
        }

        /// <summary>
        /// Tear down the Case Study unit tests, which in particular, affords us an opportunity
        /// to <see cref="IDisposable.Dispose"/> of the <see cref="Scope"/> prior to the unit
        /// test execution context having gone out of scope. This is critical in order for proper
        /// reporting to occur through the <see cref="TestFixtureBase.OutputHelper"/> instance.
        /// </summary>
        [TearDown]
        public void CaseStudyTearDown()
        {
            void OnScopeTearDown()
            {
                this.OnScopeTearDown(ref this._scope);
                this.Scope.AssertNull();
            }

            IDisposable o = null;

            $"Tears down this.{nameof(this.Scope)}".x(OnScopeTearDown);
        }
    }
}
