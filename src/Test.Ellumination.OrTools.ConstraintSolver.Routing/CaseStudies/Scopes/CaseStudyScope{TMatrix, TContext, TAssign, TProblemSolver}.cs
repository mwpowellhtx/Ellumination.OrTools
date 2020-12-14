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
    public abstract class CaseStudyScope<TMatrix, TContext, TAssign, TProblemSolver> : CaseStudyScope
        where TMatrix : Distances.Matrix
        where TContext : RoutingContext
        where TAssign : RoutingAssignmentEventArgs<TContext>
        where TProblemSolver : AssignableRoutingProblemSolver<TContext, TAssign>
    {
        protected CaseStudyScope(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Gets the Matrix associated with the Case Study.
        /// </summary>
        internal abstract TMatrix Matrix { get; }

        /// <summary>
        /// Gets the Context associated with the Case Study.
        /// </summary>
        internal abstract TContext Context { get; }

        protected virtual TProblemSolver CreateProblemSolver()
        {
            var problemSolver = Activator.CreateInstance<TProblemSolver>();
            problemSolver.Assign += this.OnProblemSolverAssign;
            return problemSolver;
        }

        private TProblemSolver _problemSolver;

        // TODO: TBD: may be able to install this in the base class..
        /// <summary>
        /// Gets or Sets the ProblemSolver.
        /// </summary>
        internal TProblemSolver ProblemSolver => this._problemSolver ?? (
            this._problemSolver = CreateProblemSolver()
        );

        /// <summary>
        /// Event handler occurs On
        /// <see cref="AssignableRoutingProblemSolver{TContext, TAssign}.Assign"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProblemSolverAssign(object sender, TAssign e)
        {
        }

        /// <summary>
        /// Override in order to dispose the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this.OnDisposing();

                void OnDisposeProblemSolver(TProblemSolver problemSolver)
                {
                    if (problemSolver != null)
                    {
                        problemSolver.Assign -= this.OnProblemSolverAssign;
                        problemSolver.Dispose();
                    }
                }

                OnDisposeProblemSolver(this.ProblemSolver);

                this.Context?.Dispose();
            }
        }
    }
}
