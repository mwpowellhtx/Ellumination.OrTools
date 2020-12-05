using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
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

        /// <summary>
        /// Gets the Scope for use throughout the unit tests.
        /// </summary>
        protected virtual TScope Scope { get; private set; }

        /// <summary>
        /// Override in order to Initialize the <paramref name="scope"/>.
        /// </summary>
        /// <param name="scope"></param>
        protected abstract void InitializeScope(out TScope scope);

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

            $"Connect this.{nameof(this.Scope)}.{nameof(CaseStudyScope.Disposing)} event".x(
                () => this.Scope.Disposing += this.OnScopeDisposing
            );
        }

        /// <summary>
        /// Override in order to better handle the <see cref="CaseStudyScope.Disposing"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnScopeDisposing(object sender, EventArgs e)
        {
            sender.AssertNotNull().AssertIsType<TScope>();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                void OnDispose(TScope scope)
                {
                    if (scope != null)
                    {
                        // Unwire the Disposing event.
                        scope.Disposing -= this.OnScopeDisposing;

                        // And...
                        scope.Dispose();
                    }
                }

                OnDispose(this.Scope);
            }

            base.Dispose(disposing);
        }
    }
}
