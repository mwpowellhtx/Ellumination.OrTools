using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools
{
    /// <summary>
    /// FibonacciCalculator calculates the sequence of numbers in the Fibonacci Sequence.
    /// </summary>
    public class FibonacciCalculator : IEnumerable<int>
    {
        /// <summary>
        /// Default constructor with the starting <c>{0, 1}</c> sequence.
        /// </summary>
        public FibonacciCalculator()
        {
        }

        private void Initialize(int count)
        {
            // Which should calculate the sequence out of the gate.
            this.Get(count - 1);
        }

        /// <summary>
        /// Constructs the Fibonacci Sequence with the initial <paramref name="count"/>.
        /// </summary>
        /// <param name="count"></param>
        public FibonacciCalculator(int count) => this.Initialize(count);

        /// <summary>
        /// Gets the current set of Fibonacci Values.
        /// </summary>
        private List<int> Values { get; } = new List<int> { 0, 1 };

        /// <summary>
        /// Calculates the next segment in the Fibonacci Sequence given
        /// a <paramref name="pair"/> and the <paramref name="count"/>
        /// of requested <see cref="Values"/>.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        private IEnumerable<int> Calculate(int count, params int[] pair)
        {
            // TODO: TBD: could add some parameter validation here...
            // TODO: TBD: should be exactly TWO elements.
            // TODO: TBD: could be more, and we just take the first two, that would be acceptable as well...
            var (a, b) = (pair[0], pair[1]);

            while (count-- > 0)
            {
                (a, b) = (b, a + b);
                yield return b;
            }
        }

        /// <summary>
        /// Returns the Sequence <see cref="Enumerable.ElementAt"/> the <paramref name="index"/>.
        /// Has the side effect of Adding the next Segment in the Sequence when it must be
        /// calculated for the first time.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <see cref="Values"/>
        /// <see cref="Calculate"/>
        /// <see cref="Enumerable.Count"/>
        /// <see cref="Enumerable.Skip"/>
        /// <see cref="Enumerable.ElementAt"/>
        /// <see cref="Enumerable.ToArray"/>
        /// <see cref="List{T}.AddRange"/>
        public int Get(int index)
        {
            var this_Values = this.Values;
            var this_Values_Count = this_Values.Count();
            // Add the difference in the Sequence, no need to recalculate any others.
            this_Values.AddRange(this.Calculate(
                (index + 1) - this_Values_Count
                , this_Values.Skip(this_Values_Count - 2).ToArray()
            ));
            return this_Values.ElementAt(index);
        }

        /// <summary>
        /// Indexer to <see cref="Get"/> the specified <paramref name="index"/> in the sequence.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[int index] => this.Get(index);

        /// <summary>
        /// Expands the Fibonacci Sequence to the <paramref name="requestedCount"/>.
        /// If there is nothing to <see cref="Calculate"/> during the <see cref="Get"/>
        /// operation, then effectively it is a no-op. Only calculates what is required
        /// to calculate to match the requested sequence.
        /// </summary>
        /// <param name="requestedCount">Expands the sequence to the RequestedCount.</param>
        /// <returns>The Calculator instance in order for fluent requests to happen.</returns>
        public FibonacciCalculator Expand(int requestedCount)
        {
            this.Get(requestedCount - 1);
            return this;
        }

        /// <inheritdoc/>
        public IEnumerator<int> GetEnumerator() => this.Values.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
