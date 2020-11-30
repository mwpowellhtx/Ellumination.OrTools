using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ellumination.OrTools.Sat.Parameters
{
    using static Characters;
    using static String;

    /// <summary>
    /// Provides a useful set of <see cref="IParameter"/> extension methods.
    /// </summary>
    internal static class ParameterExtensionMethods
    {
        /// <summary>
        /// Rendering an <see cref="Enum"/> <paramref name="value"/> is a bit of a special case,
        /// but it is not that special of a case given a bit of introspection exposed by the
        /// Parameters API.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string RenderEnumParameterValue(this object value)
        {
            var valueType = value.GetType();

            if (!valueType.IsEnum)
            {
                throw new InvalidOperationException($"Unexpected enum type '{valueType.FullName}'.");
            }

            var defaultRendering = $"{value}";

            var field = valueType.GetField(defaultRendering);

            return field.GetCustomAttribute<ParameterMemberNameAttribute>()?.MemberName ?? defaultRendering;
        }

        /// <summary>
        /// Renders the <see cref="Parameter"/> <see cref="object"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <see cref="object"/>
        /// <see cref="object.ToString"/>
        /// <see cref="string.ToLower"/>
        public static string RenderParameterValue(this object value) =>
            value == null
                ? null : value.GetType().IsEnum
                    ? value.RenderEnumParameterValue() : $"{value}".ToLower();

        /// <summary>
        /// Renders the <see cref="Parameter{T}"/> <see cref="bool"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <see cref="bool"/>
        /// <see cref="bool.ToString"/>
        /// <see cref="string.ToLower"/>
        public static string RenderParameterValue(this bool value) => $"{value}".ToLower();

        /// <summary>
        /// Renders the <see cref="Parameter{T}"/> <see cref="int"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <see cref="int"/>
        /// <see cref="int.ToString"/>
        /// <see cref="string.ToLower"/>
        public static string RenderParameterValue(this int value) => $"{value}".ToLower();

        /// <summary>
        /// Renders the <see cref="Parameter{T}"/> <see cref="long"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <see cref="long"/>
        /// <see cref="long.ToString"/>
        /// <see cref="string.ToLower"/>
        public static string RenderParameterValue(this long value) => $"{value}".ToLower();

        /// <summary>
        /// Renders the <see cref="double"/> <paramref name="value"/>, including cases
        /// where it may be <see cref="double.NaN"/>, <see cref="double.PositiveInfinity"/>,
        /// or <see cref="double.NegativeInfinity"/>. Default <paramref name="format"/> is
        /// &quot;R&quot;.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format">Specify the formatting, defaults to roundtrip, or R.</param>
        /// <returns></returns>
        /// <see cref="double.IsNaN"/>
        /// <see cref="double.IsPositiveInfinity"/>
        /// <see cref="double.IsNegativeInfinity"/>
        /// <see cref="double.ToString(string)"/>
        /// <see cref="string.Substring(int, int)"/>
        /// <see cref="!:https://docs.microsoft.com/en-us/dotnet/api/system.double.tostring"/>
        public static string RenderParameterValue(this double value, string format = "R")
        {
            // Ignoring the IsABC, taking only the ABC in the name.
            string RenderName(string name) => name.Substring(2, 3);

            var infOrValue = !double.IsInfinity(value) ? value.ToString(format)
                : $"{RenderName(nameof(double.IsInfinity))}";

            infOrValue = !double.IsNegativeInfinity(value)
                ? infOrValue : $"-{infOrValue}";

            var nanOrValue = !double.IsNaN(value) ? infOrValue
                : $"{RenderName(nameof(double.IsNaN))}";

            return nanOrValue.ToLower();
        }

        /// <summary>
        /// Renders the <paramref name="parameters"/> assuming a nominal set of
        /// <paramref name="options"/>.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string RenderParameters(this IEnumerable<IParameter> parameters, IParameterValueRenderingOptions options = null)
            => Join($"{SemiColon}", parameters.Select(x => x.ToString(options)));
    }
}
