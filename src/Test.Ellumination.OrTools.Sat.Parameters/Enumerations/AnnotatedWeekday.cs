//using System;
//using System.Linq;
//using System.Reflection;

namespace Ellumination.OrTools.Sat.Parameters
{
    using static Names;

    public enum AnnotatedWeekday : long
    {
        [ParameterMemberName(MONDAY)]
        Monday = 0L,

        [ParameterMemberName(TUESDAY)]
        Tuesday,

        [ParameterMemberName(WEDNESDAY)]
        Wednesday,

        [ParameterMemberName(THURSDAY)]
        Thursday,

        [ParameterMemberName(FRIDAY)]
        Friday
    }

    //// TODO: TBD: no, we will simply use the Object based RenderParameterValue...
    //internal static class WeekdayExtensionMethods
    //{
    //    /// <summary>
    //    /// Renders the <paramref name="value"/> according to its
    //    /// <see cref="ParameterMemberNameAttribute"/> annotation.
    //    /// </summary>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    public static string RenderAnnotatedWeekdayMemberName(this AnnotatedWeekday value)
    //    {
    //        var annotatedWeekdayType = typeof(AnnotatedWeekday);
    //        var field = annotatedWeekdayType.GetField($"{value}");
    //        return field?.GetCustomAttribute<ParameterMemberNameAttribute>(true)?.MemberName
    //            ?? throw new InvalidOperationException($"'{annotatedWeekdayType.FullName}' value '{value}' is unsupported.");
    //    }
    //}
}
