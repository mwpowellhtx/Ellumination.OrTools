namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit.Abstractions;

    /// <inheritdoc/>
    public class TimeWindowCaseStudyTests : TimeWindowCaseStudyTests<TimeWindowCaseStudyScope, TimeWindowDimension>
    {
        public TimeWindowCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }
    }
}
