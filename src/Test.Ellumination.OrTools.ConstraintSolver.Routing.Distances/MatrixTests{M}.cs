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
    public abstract class MatrixTests<M> : TestFixtureBase
        where M : Matrix, new()
    {
        /// <summary>
        /// Gets or Sets the Instance.
        /// </summary>
        protected virtual M Instance { get; set; }

        /// <summary>
        /// Gets the ExpectedWidth.
        /// </summary>
        protected virtual int ExpectedWidth { get; }

        /// <summary>
        /// Gets the ExpectedHeight.
        /// </summary>
        protected virtual int ExpectedHeight { get; }

        /// <summary>
        /// Gets whether ExpectedMirrored.
        /// </summary>
        protected virtual bool ExpectedMirrored { get; } = default;

        /// <summary>
        /// Gets whether ExpectedSquare.
        /// </summary>
        /// <see cref="ExpectedWidth"/>
        /// <see cref="ExpectedHeight"/>
        protected virtual bool ExpectedSquare => this.ExpectedWidth == this.ExpectedHeight;

        /// <summary>
        /// Gets the ExpectedCount.
        /// </summary>
        /// <see cref="ExpectedWidth"/>
        /// <see cref="ExpectedHeight"/>
        protected virtual int ExpectedCount => this.ExpectedWidth * this.ExpectedHeight;

        /// <summary>
        /// Gets <see cref="Instance"/> Indices used throughout testing.
        /// </summary>
        protected virtual IEnumerable<(int x, int y)> InstanceIndices
        {
            get
            {
                IEnumerable<(int x, int y)> GetAll(M instance)
                {
                    if (instance != null)
                    {
                        foreach (var index in Enumerable.Range(0, instance.Width).SelectMany(
                            x => Enumerable.Range(0, instance.Height).Select(y => (x, y))
                        ))
                        {
                            yield return index;
                        }
                    }
                }

                return GetAll(this.Instance).AssertNotNull();
            }
        }

        protected MatrixTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Override in order to specify how to create the <see cref="Instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        protected abstract void OnCreateInstance(out M instance);

        /// <summary>
        /// Provides a Default <paramref name="matrix"/>, <paramref name="x"/>,
        /// <paramref name="y"/> selector.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected static int? DefaultMatrixSelector(M matrix, int x, int y) => matrix[x, y];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="indices"></param>
        protected virtual void VerifyNullMatrix(M actual, IEnumerable<(int x, int y)> indices)
        {
            actual.AssertNull();
            // TODO: TBD: dubious the value these two lines actually offers...
            actual.AssertEqual(this.ExpectedWidth, _ => _?.Width ?? this.ExpectedWidth);
            actual.AssertEqual(this.ExpectedHeight, _ => _?.Height ?? this.ExpectedHeight);
            indices.AssertCollectionEmpty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected delegate int? MatrixSelector(M matrix, int x, int y);

        /// <summary>
        /// Verifies the <paramref name="matrix"/> value at the <paramref name="index"/>.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="index"></param>
        /// <param name="expected"></param>
        protected virtual void VerifyMatrixValue(M matrix, (int x, int y) index, int? expected = null) =>
            matrix[index.x, index.y].AssertEqual(expected);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="indices"></param>
        /// <param name="selector"></param>
        protected virtual void VerifyMatrix(M actual, IEnumerable<(int x, int y)> indices, MatrixSelector selector = null)
        {
            selector = selector ?? DefaultMatrixSelector;
            actual.AssertNotNull();
            actual.AssertEqual(this.ExpectedWidth, _ => _.Width);
            actual.AssertEqual(this.ExpectedHeight, _ => _.Height);
            actual.AssertEqual(this.ExpectedSquare, _ => _.IsSquare);
            actual.AssertEqual(this.ExpectedMirrored, _ => _.IsMirrored);
            indices.AssertEqual(this.ExpectedCount, _ => _.Count());
            void OnVerifyMatrixValueAtIndex((int x, int y) index) => VerifyMatrixValue(actual, index);
            indices.ToList().ForEach(OnVerifyMatrixValueAtIndex);
        }

        protected virtual void OnVerifyMatrix(M actual, IEnumerable<(int x, int y)> indices
            , (int? width, int? height)? expectedDim = null
        )
        {
            expectedDim = expectedDim ?? ((null, null));

            if (actual == null)
            {
                this.VerifyNullMatrix(actual, indices);
                return;
            }

            this.VerifyMatrix(actual, indices);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="instanceIndices"></param>
        protected virtual void OnVerifyMatrixBeforeBackground(out M instance, out IEnumerable<(int x, int y)> instanceIndices) =>
            this.OnVerifyMatrix(instance = this.Instance, instanceIndices = this.InstanceIndices);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="instanceIndices"></param>
        protected virtual void OnVerifyMatrixAfterBackground(out M instance, out IEnumerable<(int x, int y)> instanceIndices) =>
            this.OnVerifyMatrix(instance = this.Instance, instanceIndices = this.InstanceIndices, (DefaultLength, DefaultLength));

        /// <summary>
        /// Initializes the test background.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="instanceIndices"></param>
        [Background]
        public void MatrixBackground(M instance, IEnumerable<(int x, int y)> instanceIndices)
        {
            $"Verify default this.{nameof(this.Instance)}".x(() => OnVerifyMatrixBeforeBackground(out instance, out instanceIndices));

            void OnCreateInstance()
            {
                this.OnCreateInstance(out var matrix);
                if (matrix != null)
                {
                    this.Instance = matrix;
                }
            }

            $"Install new this.{nameof(this.Instance)}".x(OnCreateInstance);

            $"Verify this.{nameof(this.Instance)}".x(() => OnVerifyMatrixAfterBackground(out instance, out instanceIndices));
        }
    }
}
