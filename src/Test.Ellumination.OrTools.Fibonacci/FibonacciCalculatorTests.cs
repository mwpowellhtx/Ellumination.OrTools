using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    public class FibonacciCalculatorTests : TestFixtureBase
    {
        /// <summary>
        /// Constructs the fixture given parameters.
        /// </summary>
        /// <param name="outputHelper"></param>
        public FibonacciCalculatorTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Gets the ExpectedSequences.
        /// </summary>
        private IDictionary<int, IEnumerable<int>> ExpectedSequences { get; set; }

        [Background]
        public void CalculatorBackground(int count)
        {
            $"Initialize the {nameof(ExpectedSequences)}".x(() =>
            {
                // 5, 10, 15 elements...
                var sequence = Range(0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377);

                this.ExpectedSequences = new Dictionary<int, IEnumerable<int>>();

                Range(5, 10, 15).ToList().ForEach(key => this.ExpectedSequences[key] = sequence.Take(key).ToArray());
            });

            $"Fibonacci {nameof(count)} is 5".x(() => count = 5);

            $"Initialize this.{nameof(Calculator)}".x(() => this.Calculator[count - 1].AssertEqual(
                this.ExpectedSequences[count].Last()
            ));

            $"Verify this.{nameof(Calculator)}".x(() => this.Calculator.AssertEqual(
                this.ExpectedSequences[count]
            ));
        }

        [Scenario]
        public void Sequence_Does_Not_Recalculate(int count, int middle)
        {
            $"Fibonacci {nameof(count)} is 5".x(() => count = 5);

            $"Fibonacci {nameof(middle)} is 3".x(() => middle = 3);

            $"Verify element at {middle - 1}".x(() => this.Calculator[middle - 1].AssertEqual(
                this.ExpectedSequences[count].ElementAt(middle - 1)
            ));

            $"Verify this.{nameof(Calculator)}".x(() => this.Calculator.AssertEqual(
                this.ExpectedSequences[count]
            ));
        }

        [Scenario]
        public void Sequence_Calculates(int count)
        {
            $"Fibonacci {nameof(count)} is 10".x(() => count = 10);

            $"Request the element at {count - 1}".x(() => this.Calculator[count - 1].AssertEqual(
                this.ExpectedSequences[count].Last()
            ));

            $"And verify the sequence itself".x(() => this.Calculator.AssertEqual(
                this.ExpectedSequences[count]
            ));
        }

        [Scenario]
        public void Sequence_Can_Expand(int count)
        {
            $"Fibonacci {nameof(count)} is 15".x(() => count = 15);

            $"Starting sequence count was not {count}".x(() => this.Calculator.AssertTrue(x => x.Count() < count));

            $"Expand to {count} and verify".x(() => this.Calculator.Expand(count).AssertEqual(
                this.ExpectedSequences[count]
            ));
        }
    }
}
