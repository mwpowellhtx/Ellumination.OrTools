using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Provides Routing Model ProblemSolver capability.
    /// </summary>
    /// <typeparam name="TContext">The Context type.</typeparam>
    /// <typeparam name="TAssign">It is a bit of a Swiss Army knife for events, but
    /// does support all phases of <see cref="Assignment"/> processing.</typeparam>
    /// <see cref="RoutingContext"/>
    /// <see cref="RoutingAssignmentEventArgs{TContext}"/>
    public interface IAssignableRoutingProblemSolver<TContext, TAssign> : IDisposable
        where TContext : RoutingContext
        where TAssign : RoutingAssignmentEventArgs<TContext>
    {
        /// <summary>
        /// Event occurs when Configuring the <see cref="RoutingSearchParameters"/>
        /// approaching the <see cref="Solve"/> invocation.
        /// </summary>
        event EventHandler<RoutingSearchParametersEventArgs> ConfigureSearchParameters;

        /// <summary>
        /// Facilitates the Routing Model problem solution given <paramref name="context"/>.
        /// Supports several phases during the Processing of a given <see cref="Assignment"/>,
        /// Start, End, Vehicles, ForEach Node, etc.
        /// </summary>
        /// <param name="context"></param>
        void Solve(TContext context);

        /// <summary>
        /// Event occurs Before <see cref="Assignment"/> Processing.
        /// </summary>
        event EventHandler<TAssign> BeforeAssignment;

        /// <summary>
        /// Event occurs After <see cref="Assignment"/> Processing.
        /// </summary>
        event EventHandler<TAssign> AfterAssignment;

        /// <summary>
        /// Event occurs Before <see cref="Assignment"/> Vehicle Processing.
        /// </summary>
        event EventHandler<TAssign> BeforeAssignmentVehicle;

        /// <summary>
        /// Event occurs After <see cref="Assignment"/> Vehicle Processing.
        /// </summary>
        event EventHandler<TAssign> AfterAssignmentVehicle;

        /// <summary>
        /// Event occurs ForEach <see cref="Assignment"/> Node Processing.
        /// </summary>
        event EventHandler<TAssign> ForEachAssignmentNode;
    }
}
