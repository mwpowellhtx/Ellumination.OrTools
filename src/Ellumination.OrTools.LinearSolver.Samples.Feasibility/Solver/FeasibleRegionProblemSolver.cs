﻿using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.LinearSolver.Samples.Feasibility
{
    using Google.OrTools.LinearSolver;

    /// <summary>
    /// Based on the Feasible Region example provided on the Google Optimization Linear
    /// Programming (LP) web site.
    /// </summary>
    /// <see cref="!:http://developers.google.com/optimization/lp/glop" />
    /// <inheritdoc />
    public class FeasibleRegionProblemSolver
        : OrLinearProblemSolverBase<FeasibleRegionProblemSolver, double>
    {
        /// <inheritdoc />
        public FeasibleRegionProblemSolver()
            : base(@"Feasible Region", p => 3*p.x.SolutionValue() + 4*p.y.SolutionValue())
        {
        }

        private IEnumerable<Variable> _variables;

        protected override IEnumerable<Variable> Variables
        {
            get
            {
                IEnumerable<Variable> GetAll()
                {
                    var solver = Solver;

                    var x = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "x");
                    yield return SetProblemComponent(x, (p, m) => p.x = m);

                    var y = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "y");
                    yield return SetProblemComponent(y, (p, m) => p.y = m);
                }

                return _variables ?? (_variables = GetAll().ToArray());
            }
        }

        protected override void PrepareConstraints(Solver solver)
        {
            {
                // x + 2y <= 14
                var c = solver.MakeConstraint(NegativeInfinity, 14);
                c.SetCoefficient(Problem.x, 1);
                c.SetCoefficient(Problem.y, 2);
                ClrCreatedObjects.Add(c);
                SetProblemComponent(c, (p, c1) => p.c1 = c1);
            }

            {
                // 3x - y >= 0
                var c = solver.MakeConstraint(0, PositiveInfinity);
                c.SetCoefficient(Problem.x, 3);
                c.SetCoefficient(Problem.y, -1);
                ClrCreatedObjects.Add(c);
                SetProblemComponent(c, (p, c2) => p.c2 = c2);
            }

            {
                // x - y <= 2
                var c = solver.MakeConstraint(NegativeInfinity, 2);
                c.SetCoefficient(Problem.x, 1);
                c.SetCoefficient(Problem.y, -1);
                ClrCreatedObjects.Add(c);
                SetProblemComponent(c, (p, c3) => p.c3 = c3);
            }
        }

        protected override void PrepareObjective(Solver solver)
        {
            var obj = solver.Objective();
            obj.SetCoefficient(Problem.x, 3);
            obj.SetCoefficient(Problem.y, 4);
            obj.SetMaximization();
            ClrCreatedObjects.Add(obj);
            SetProblemComponent(obj, (p, o) => p.obj = o);
        }
    }
}
