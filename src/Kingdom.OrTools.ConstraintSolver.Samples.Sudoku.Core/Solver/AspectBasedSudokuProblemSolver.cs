using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    using Google.OrTools.ConstraintSolver;
    using Xunit;
    using static SudokuPuzzle;
    using static IntValueStrategy;
    using static IntVarStrategy;

    public class AspectBasedSudokuProblemSolver : OrProblemSolverBase<SudokuProblemSolverAspect>
    {
        private static IEnumerable<SudokuProblemSolverAspect> GetDefaultAspects()
        {
            yield return new SudokuProblemSolverAspect();
        }

        public AspectBasedSudokuProblemSolver(string modelName)
            : base(modelName, GetDefaultAspects())
        {
        }

        /// <summary>
        /// Gets the Solution.
        /// </summary>
        public ISudokuPuzzle Solution { get; private set; } = new SudokuPuzzle();

        /// <summary>
        /// Begins a New Search corresponding with the <see cref="Solver"/> and
        /// <paramref name="agent"/>.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        /// <inheritdoc />
        protected override ISearchAgent NewSearch(ISearchAgent agent)
        {
            agent.ProcessVariables -= OnProcessVariables;
            agent.ProcessVariables += OnProcessVariables;

            // The simple strategies are okay and yield expected results. But let's keep it interesting.
            return agent.NewSearch(a => a.Solver.MakePhase(a.Variables, ChooseRandom, AssignRandomValue));
        }

        /// <summary>
        /// <see cref="ISearchAgent"/> ProcessVariables event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <inheritdoc />
        protected override void OnProcessVariables(object sender, ProcessVariablesEventArgs e)
        {
            var candidate = new SudokuPuzzle();
            ISudokuPuzzle local = candidate;

            // In this case we know that there is a Single Aspect.
            var aspect = Aspects.SingleOrDefault();
            Assert.NotNull(aspect);

            for (var row = MinSize; row < Size; row++)
            {
                for (var col = MinSize; col < Size; col++)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    local[row, col] = (int) aspect.Cells[row, col].Value();
                }
            }

            /* If we're here processing variables, it should be because we are processing the next
             * solution. However, in the event we still do not have a solution, then simply return. */

            // TODO: TBD: we really should never land here I don't think...
            if (!local.IsSolved)
            {
                return;
            }

            Solution = local;

            // False is the default, so only mark whether ShouldBreak when we have one.
            e.ShouldBreak = true;

            base.OnProcessVariables(sender, e);
        }
    }
}
