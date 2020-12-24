namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit.Abstractions;

#if false
    // https://developers.google.com/optimization/routing/cvrptw_resources#loading_constraints
    /* The following code adds time windows for loading and unloading the vehicles at the depot.
     * These windows, created by the method <c>FixedDurationIntervalVar</c>, are <em>variable
     * time windows</em>, meaning they do not have fixed start and end times, unlike the time
     * windows at the locations. The widths of the windows are specified by <c>vehicle_load_time</c>
     * and <c>vehicle_unload_time</c>, which happen to be the same in this example. */
    var solver = routing.solver();
    var intervals = new IntervalVar[data.VehicleNumber * 2];
    for (var i = 0; i < data.VehicleNumber; i++)
    {
        // Add load duration at start of routes.
        intervals[2 * i] = solver.MakeFixedDurationIntervalVar(timeDimension.CumulVar(routing.Start(i)), data.VehicleLoadTime, "depot_interval");
        //                                                                            ^^^^^^^^^^^^^      ^^^^^^^^^^^^^^^^^^^^ (?)
        // Add unload duration at end of routes.
        intervals[2 * i + 1] = solver.MakeFixedDurationIntervalVar(timeDimension.CumulVar(routing.End(i)), data.VehicleUnloadTime, "depot_interval");
        //                                                                                ^^^^^^^^^^^      ^^^^^^^^^^^^^^^^^^^^^^ (?)
    }
#endif // false

#if false
    // https://developers.google.com/optimization/routing/cvrptw_resources#add-resource-constraints-at-the-depot
    /* The following code creates the contraint that at most two vehicles can be loaded
     * or unloaded at the same time.
     * 
     * <c>depot_capacity</c> is the maximum number of vehicles that can be loaded or unloaded
     * at the same time, which is 2 in this example.
     * 
     * <c>depot_usage</c> is a vector containing the relative amounts of space required by each
     * vehicle during loading, or unloading, respectively. In this example, we assume that all
     * vehicles require the same amount of space, so <c>depot_usage</c> contains all ones. This
     * means that the maximum number of vehicles that can be loaded at the same time is 2. */
    var depot_usage = Enumerable.Repeat<long>(1, intervals.Length).ToArray();
    solver.Add(solver.MakeCumulative(intervals, depot_usage, data.DepotCapacity, "depot"));
#endif // false

#if false
    /*
     *  Route for vehicle 0:
     *   0 Time(5, 5) -> 8 Time(8, 8) -> 14 Time(11, 11) -> 16 Time(13, 13) -> 0 Time(20, 20)
     *  Time of the route: 20 min
     *
     *  Route for vehicle 1:
     *   0 Time(0, 0) -> 12 Time(4, 4) -> 13 Time(6, 6) -> 15 Time(11, 11) -> 11 Time(14, 14) -> 0 Time(20, 20)
     *  Time of the route: 20 min
     *
     *  Route for vehicle 2:
     *   0 Time(5, 5) -> 7 Time(7, 7) -> 1 Time(11, 11) -> 4 Time(13, 13) -> 3 Time(14, 14) -> 0 Time(25, 25)
     *  Time of the route: 25 min
     *
     *  Route for vehicle 3:
     *   0 Time(0, 0) -> 9 Time(2, 3) -> 5 Time(4, 5) -> 6 Time(6, 9) -> 2 Time(10, 12) -> 10 Time(14, 16) -> 0 Time(25, 25)
     *  Time of the route: 25 min
     *
     *  Total time of all routes: 90 min
     */
#endif // false

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/vrptw"/>
    public class TimeWindowResourceConstraintCaseStudyTests : TimeWindowCaseStudyTests<
        TimeWindowResourceConstraintCaseStudyScope, TimeWindowResourceConstraintDimension>
    {
        public TimeWindowResourceConstraintCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }
    }
}
