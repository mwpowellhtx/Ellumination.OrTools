namespace Ellumination.OrTools
{
    using Xunit.Abstractions;

    /// <summary>
    /// Establishes a Test base class.
    /// </summary>
    public abstract class TestFixtureBase
    {
        /// <summary>
        /// Gets the OutputHelper.
        /// </summary>
        protected ITestOutputHelper OutputHelper { get; }

        protected TestFixtureBase(ITestOutputHelper outputHelper)
        {
            this.OutputHelper = outputHelper;
        }
    }
}
