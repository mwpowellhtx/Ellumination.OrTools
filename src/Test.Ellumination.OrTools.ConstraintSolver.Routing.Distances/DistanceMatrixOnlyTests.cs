using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    /// <summary>
    /// 
    /// </summary>
    public abstract class DistanceMatrixOnlyTests : DistanceMatrixTests<DistanceMatrix>
    {
        protected DistanceMatrixOnlyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Override in order to specify how to create the <see cref="Instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        protected override void OnCreateInstance(out DistanceMatrix instance) =>
            instance = new DistanceMatrix(this.ExpectedLength);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceIndices"></param>
        [Background]
        public void DistanceMatrixOnlyBackground(IEnumerable<(int x, int y)> instanceIndices)
        {
            $"Verify the {nameof(instanceIndices)}".x(() => instanceIndices
                = this.InstanceIndices.AssertCollectionNotEmpty().ToArray()
            );
        }
    }
}
