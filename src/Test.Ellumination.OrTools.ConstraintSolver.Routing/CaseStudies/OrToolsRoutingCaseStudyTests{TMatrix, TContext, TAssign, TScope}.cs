using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    /// <summary>
    /// Override in order to specify further bits concerning the Case Study.
    /// </summary>
    /// <typeparam name="TMatrix">A <see cref="Distances.Matrix"/> type.</typeparam>
    /// <typeparam name="TContext">A <see cref="RoutingContext"/> type.</typeparam>
    /// <typeparam name="TAssign">A <see cref="RoutingAssignmentEventArgs{TContext}"/> type.</typeparam>
    /// <typeparam name="TProblemSolver">A <see cref="AssignableRoutingProblemSolver{TContext, TAssign}"/> type.</typeparam>
    /// <typeparam name="TScope">A <see cref="CaseStudyScope{TMatrix, TContext, TAssign, TProblemSolver}"/> type.</typeparam>
    public abstract class OrToolsRoutingCaseStudyTests<TMatrix, TContext, TAssign, TProblemSolver, TScope> : TestFixtureBase
        where TMatrix : Distances.Matrix
        where TContext : RoutingContext
        where TAssign : RoutingAssignmentEventArgs<TContext>
        where TProblemSolver : AssignableRoutingProblemSolver<TContext, TAssign>
        where TScope : CaseStudyScope<TMatrix, TContext, TAssign, TProblemSolver>
    {
        protected OrToolsRoutingCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
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
        protected virtual TScope OnVerifyInitialScope(TScope scope)
        {
            scope.AssertNotNull();
            scope.Context.AssertNotNull();
            scope.Matrix.AssertNotNull();
            scope.ProblemSolver.AssertNotNull();
            return scope;
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
                this.Scope = this.OnVerifyInitialScope(scope);
            }

            void OnRollbackScope()
            {
                this.Scope?.Dispose();
                this.Scope = null;
            }

            // We cannot Rollback here, because that falls outside the scope of the actual test context.
            $"Initialize this.{nameof(this.Scope)}".x(OnInitializeScope)
                .Rollback(OnRollbackScope);
        }

        /// <summary>
        /// Override in order to perform test and scope specific verification just prior
        /// to tear down.
        /// </summary>
        /// <param name="scope"></param>
        protected virtual void OnVerifySolutionScope(TScope scope) =>
            scope.AssertNotNull();

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
                this.OnVerifySolutionScope(this.Scope);
                this.Scope?.Dispose();
            }

            $"Tears down this.{nameof(this.Scope)}".x(OnScopeTearDown);
        }
    }
}
