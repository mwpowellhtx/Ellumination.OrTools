using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using static Matrix;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public abstract class LocationDistanceMatrixTests<M> : DistanceMatrixTests<M>
        where M : LocationDistanceMatrix, new()
    {
        protected LocationDistanceMatrixTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Gets a Name for use within the unit test.
        /// </summary>
        protected static string Name => $"{Guid.NewGuid()}".ToLower();

        /// <summary>
        /// Gets or Sets the ExpectedLocations.
        /// </summary>
        protected virtual IEnumerable<string> ExpectedLocations { get; set; } = Range<string>();

        /// <summary>
        /// Starts out being of Zero Length.
        /// </summary>
        protected override int ExpectedLength { get; } = default;

        /// <inheritdoc/>
        protected override void OnCreateInstance(out M instance) =>
            this.OnCreateInstance(out instance, Range<string>().ToArray());

        /// <summary>
        /// Creates an <paramref name="instance"/> given <paramref name="locations"/>.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="locations"></param>
        protected virtual void OnCreateInstance(out M instance, params string[] locations) =>
            instance = Activator.CreateInstance(typeof(M), (IEnumerable<string>)locations)
                .AssertIsType<M>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onSetLocations"></param>
        /// <param name="instance"></param>
        /// <param name="locations"></param>
        protected virtual void OnInitializeInstance(
            Action<LocationDistanceMatrix, IEnumerable<string>> onSetLocations
            , out M instance, out IEnumerable<string> locations)
        {
            locations = Range(Name, Name, Name).AssertEqual(3, x => x.Count());
            this.OnCreateInstance(out instance, locations.ToArray());
            onSetLocations.Invoke(instance.AssertNotNull(), locations);
        }

        /// <summary>
        /// Verifies the <paramref name="matrix"/> given <paramref name="index"/> and
        /// <paramref name="expected"/> value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="index"></param>
        /// <param name="expected"></param>
        protected virtual void VerifyMatrixValue(M matrix, (string x, string y) index, int? expected = null) =>
            matrix[index.x, index.y].AssertEqual(index.x == index.y ? default(int) : expected);

        /// <inheritdoc/>
        protected override void OnVerifyMatrixBeforeBackground(out M instance, out IEnumerable<(int x, int y)> instanceIndices)
        {
            base.OnVerifyMatrixBeforeBackground(out instance, out instanceIndices);
        }

        /// <inheritdoc/>
        protected override void OnVerifyMatrixAfterBackground(out M instance, out IEnumerable<(int x, int y)> instanceIndices)
        {
            base.OnVerifyMatrixAfterBackground(out instance, out instanceIndices);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="instanceIndices"></param>
        /// <param name="locations"></param>
        [Background]
        public void LocationDistanceMatrixBackground(M instance, IEnumerable<(int x, int y)> instanceIndices, IEnumerable<string> locations)
        {
            $"Zero {nameof(locations)}".x(() => locations = Range<string>());

            $"Zero {nameof(this.ExpectedLocations)}".x(() => this.ExpectedLocations.AssertEqual(locations));

            $"Obtain {nameof(instance)}".x(() => (instance = this.Instance).AssertNotNull());

            $"Verify {nameof(instance)}.{nameof(instance.Length)}".x(() => instance.Length.AssertEqual(default));

            $"Verify {nameof(instance)}.{nameof(instance.Locations)}".x(() => instance.Locations.AssertEqual(locations));
        }
    }
}
