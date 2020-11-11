using System;
using System.Linq;

namespace Ellumination.OrTools.Sat.Parameters
{
    using static Month;

    public enum Month : long
    {
        January = 0L,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    internal static class MonthExtensionMethods
    {
        /// <summary>
        /// Renders the <see cref="Month"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RenderParameterValue(this Month value) =>
            Enum.GetValues(typeof(Month)).OfType<Month>().Contains(value)
                ? $"{value}"
                : throw new InvalidOperationException($"'{typeof(Month).FullName}' value '{value}' is unsupported.");
    }
}
