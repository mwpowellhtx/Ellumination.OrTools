using System;

namespace Ellumination.OrTools.Sat.CodeGeneration
{
    using Xunit;
    using Xunit.Abstractions;
    using static OrToolsSatGeneratedSyntaxTreeRegistryTests.Expected;
    using static SatParameterCodeGeneratorService;

    public class OrToolsSatGeneratedSyntaxTreeRegistryTests : TestFixtureBase
    {
        /// <summary>
        /// Gets a Registry instance.
        /// </summary>
        private OrToolsSatGeneratedSyntaxTreeRegistry Registry { get; }

        public OrToolsSatGeneratedSyntaxTreeRegistryTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
            Registry = new OrToolsSatGeneratedSyntaxTreeRegistry { };
        }

        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        /// <summary>
        /// Gets a Default instance of <see cref="Version"/>.
        /// </summary>
        private static Version DefaultVersion => new Version { };

        /// <summary>
        /// Provides some known Expected Values.
        /// </summary>
        /// <remarks>Maybe not the greatest idea to include a version specific unit test,
        /// however it is a kind of check and balance that we use to double check our
        /// dependency thinking.</remarks>
        internal static class Expected
        {
            /// <value>7</value>
            internal const int Major = 7;

            /// <value>3</value>
            internal const int Minor = 4;

            /// <value>7083</value>
            internal const int Build = 7247;
        }

        // TODO: TBD: should perhaps have unit tests for this property as well...
        /// <summary>
        /// Gets the Expected <see cref="Version"/>.
        /// </summary>
        /// <remarks>May seem a bit excessive, but we will verify the Version bits with
        /// every request. What this does establish is consistency over the range of
        /// unit tests.</remarks>
        /// <see cref="Major"/>
        /// <see cref="Minor"/>
        /// <see cref="Build"/>
        private static Version ExpectedVersion => GoogleOrToolsVersion
            .AssertNotNull()
            .AssertEqual(Major, x => x.Major)
            .AssertEqual(Minor, x => x.Minor)
            .AssertEqual(Build, x => x.Build);

        [Fact]
        public void Registry_Instance_Valid()
        {
            void VerifyRegistry(OrToolsSatGeneratedSyntaxTreeRegistry registry)
            {
                registry.AssertNotNull();
                registry.AssertEmpty();
                registry.OutputDirectory.AssertNull();
            }

            VerifyRegistry(Registry);
        }

        [Fact]
        public void Google_OrTools_Version_Yields_Different_Instances() => ExpectedVersion.AssertNotSame(ExpectedVersion);

        /// <summary>
        /// Verifies the <paramref name="actual"/> against both <see cref="expected"/>
        /// as well as <paramref name="default"/>.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="default"></param>
        private static void VerifyExpectedVersion(Version actual, Version expected, Version @default) => actual
            .AssertNotNull().AssertNotEqual(@default.AssertNotNull()).AssertEqual(expected).AssertNotSame(expected);

        [Fact]
        public void Version_Is_Correct() => VerifyExpectedVersion(Registry.AssertNotNull().GoogleOrToolsVersion, ExpectedVersion, DefaultVersion);
    }
}
