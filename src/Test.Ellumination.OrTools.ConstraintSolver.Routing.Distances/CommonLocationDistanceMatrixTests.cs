using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    public class CommonLocationDistanceMatrixTests : LocationDistanceMatrixTests<LocationDistanceMatrix>
    {
        public CommonLocationDistanceMatrixTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Gets or Sets the OtherLocations.
        /// </summary>
        private IEnumerable<string> OtherLocations { get; set; }

        /// <summary>
        /// Gets or Sets the OtherInstance.
        /// </summary>
        private LocationDistanceMatrix OtherInstance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="locations"></param>
        [Background]
        public void CommonLocationBackground(LocationDistanceMatrix instance, IEnumerable<string> locations)
        {
            $"Verify default this.{nameof(this.Instance)}".x(() => this.Instance.AssertNotNull()
                .AssertEqual(this.ExpectedLocations, x => x.Locations)
            );

            $"{nameof(this.OnInitializeInstance)} new {nameof(instance)} with {nameof(locations)}".x(
                () => this.OnInitializeInstance((x, a) => (this.Instance, this.ExpectedLocations) = (x, a)
                    , out instance, out locations)
            );

            $"{nameof(this.OnInitializeInstance)} new {nameof(instance)} with {nameof(locations)}".x(
                () => this.OnInitializeInstance((x, a) => (this.OtherInstance, this.OtherLocations) = (x, a)
                    , out instance, out locations)
            );

            $"Verify this.{nameof(this.ExpectedLocations)}".x(() => this.ExpectedLocations.AssertNotNull()
                .AssertCollectionNotEmpty()
            );

            $"Verify this.{nameof(this.OtherLocations)}".x(() => this.OtherLocations.AssertNotNull()
                .AssertCollectionNotEmpty()
            );
        }

        /// <summary>
        /// Verifies the Matrix Locations. We intentionally postpone the collection ordering,
        /// which has the added benefit of further validating whether we have expected results
        /// from the Matrix.
        /// </summary>
        /// <see cref="OtherInstance"/>
        /// <see cref="OtherLocations"/>
        /// <see cref="MatrixTests{M}.Instance"/>
        /// <see cref="LocationDistanceMatrixTests{M}.ExpectedLocations"/>
        [Scenario]
        public void Verify_Matrix_Locations()
        {
            $"Verify this.{nameof(this.Instance)}".x(() => this.Instance.AssertNotNull()
                .AssertEqual(this.ExpectedLocations.OrderBy(y => y), x => x.Locations)
            );

            $"Verify this.{nameof(this.OtherInstance)}".x(() => this.OtherInstance.AssertNotNull()
                .AssertEqual(this.OtherLocations.OrderBy(y => y), x => x.Locations)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="merged"></param>
        /// <param name="mergeLocations"></param>
        [Scenario]
        public void Can_Merge(LocationDistanceMatrix merged, IEnumerable<string> mergeLocations)
        {
            $"Merge {nameof(mergeLocations)}".x(
                () => mergeLocations = this.ExpectedLocations.Concat(this.OtherLocations).ToArray()
            );

            $"Verify {nameof(merged)}".x(
                () => merged = this.Instance.Merge(this.OtherInstance).AssertNotNull()
            );

            $"Verify {nameof(merged)}.{nameof(merged.Locations)}".x(
                () => merged.Locations.AssertEqual(mergeLocations.OrderBy(x => x))
            );
        }
    }
}
