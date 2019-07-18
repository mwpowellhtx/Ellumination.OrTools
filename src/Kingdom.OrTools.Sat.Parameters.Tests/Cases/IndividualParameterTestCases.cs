﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Double;
    using static Names;
    using static AnnotatedWeekday;

    internal class IndividualParameterTestCases : ParameterTestCasesBase
    {
        private static Type VerifyTypeIsEnum<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException($"Type `{type.FullName}´ is not an enum.");
            }

            return type;
        }

        private static IEnumerable<T> GetEnumValues<T>()
            where T : struct
        {
            foreach (T x in Enum.GetValues(VerifyTypeIsEnum<T>()))
            {
                yield return x;
            }
        }

        private delegate IParameter<T> CreateParameterCallback<T>(T value);

        // ReSharper disable once UnusedTypeParameter
        private delegate string RenderParameterValueCallback<TParameter, in T>(T value)
            where TParameter : IParameter<T>;

        private static RenderParameterValueCallback<TParameter, T> GetDefaultRenderParameterValueCallback<TParameter, T>(Func<T, string> render = null, string parameterName = null)
            where TParameter : IParameter<T>
            => x => $"{parameterName ?? typeof(TParameter).Name}={(render ?? (y => $"{y}")).Invoke(x)}";

        private static IEnumerable<object> GetParameterTestCase<TParameter, T>(T value, CreateParameterCallback<T> create
            , RenderParameterValueCallback<TParameter, T> render, int? precision = null)
            where TParameter : IParameter<T>
            => GetRange<object>(create(value), typeof(T), value, render(value), precision).ToArray();

        private static IEnumerable<object[]> _privateCases;

        private static IEnumerable<object[]> PrivateCases
        {
            get
            {
                string RenderDoubleValue(double x) => ParameterValueRenderingOptions.RenderDoubleValue(x);

                string RenderBoolean(bool x) => $"{x}".ToLower();

                // We do it this way because we are not here to use the code under test to inform the test case.
                string RenderAnnotatedWeekday(AnnotatedWeekday x)
                {
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (x)
                    {
                        case Monday: return MONDAY;
                        case Tuesday: return TUESDAY;
                        case Wednesday: return WEDNESDAY;
                        case Thursday: return THURSDAY;
                        case Friday: return FRIDAY;
                    }

                    throw new InvalidOperationException($"Unexpected value `{x}´.");
                }

                string RenderAnnotatedEnumValue<T>(T x) => x is AnnotatedWeekday y ? RenderAnnotatedWeekday(y) : $"{x}";

                IEnumerable<object[]> GetAll()
                {
                    // Default Boolean is False, correct...
                    yield return GetParameterTestCase(default, _ => new BooleanParameter(), GetDefaultRenderParameterValueCallback<BooleanParameter, bool>(RenderBoolean)).ToArray();
                    yield return GetParameterTestCase(true, y => new BooleanParameter(y), GetDefaultRenderParameterValueCallback<BooleanParameter, bool>(RenderBoolean)).ToArray();

                    yield return GetParameterTestCase(default, _ => new AnnotatedBooleanParameter(), GetDefaultRenderParameterValueCallback<AnnotatedBooleanParameter, bool>(RenderBoolean, annotated_boolean)).ToArray();
                    yield return GetParameterTestCase(true, y => new AnnotatedBooleanParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedBooleanParameter, bool>(RenderBoolean, annotated_boolean)).ToArray();

                    yield return GetParameterTestCase(default, _ => new IntegerParameter(), GetDefaultRenderParameterValueCallback<IntegerParameter, int>()).ToArray();
                    yield return GetParameterTestCase(default, _ => new AnnotatedIntegerParameter(), GetDefaultRenderParameterValueCallback<AnnotatedIntegerParameter, int>(null, annotated_integer)).ToArray();

                    foreach (var x in GetRange(1, 2))
                    {
                        yield return GetParameterTestCase(x, y => new IntegerParameter(y), GetDefaultRenderParameterValueCallback<IntegerParameter, int>()).ToArray();
                        yield return GetParameterTestCase(-x, y => new IntegerParameter(y), GetDefaultRenderParameterValueCallback<IntegerParameter, int>()).ToArray();

                        yield return GetParameterTestCase(x, y => new AnnotatedIntegerParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedIntegerParameter, int>(null, annotated_integer)).ToArray();
                        yield return GetParameterTestCase(-x, y => new AnnotatedIntegerParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedIntegerParameter, int>(null, annotated_integer)).ToArray();
                    }

                    yield return GetParameterTestCase(default, _ => new LongParameter(), GetDefaultRenderParameterValueCallback<LongParameter, long>()).ToArray();
                    yield return GetParameterTestCase(default, _ => new AnnotatedLongParameter(), GetDefaultRenderParameterValueCallback<AnnotatedLongParameter, long>(null, annotated_long)).ToArray();

                    foreach (var x in GetRange<long>(1, 2))
                    {
                        yield return GetParameterTestCase(x, y => new LongParameter(y), GetDefaultRenderParameterValueCallback<LongParameter, long>()).ToArray();
                        yield return GetParameterTestCase(-x, y => new LongParameter(y), GetDefaultRenderParameterValueCallback<LongParameter, long>()).ToArray();

                        yield return GetParameterTestCase(x, y => new AnnotatedLongParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedLongParameter, long>(null, annotated_long)).ToArray();
                        yield return GetParameterTestCase(-x, y => new AnnotatedLongParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedLongParameter, long>(null, annotated_long)).ToArray();
                    }

                    yield return GetParameterTestCase(default, _ => new MonthParameter(), GetDefaultRenderParameterValueCallback<MonthParameter, Month>(RenderAnnotatedEnumValue)).ToArray();
                    yield return GetParameterTestCase(default, _ => new AnnotatedMonthParameter(), GetDefaultRenderParameterValueCallback<AnnotatedMonthParameter, Month>(RenderAnnotatedEnumValue, annotated_month)).ToArray();

                    // Ostensibly Skipping the Default Value.
                    foreach (var x in GetEnumValues<Month>().Skip(1))
                    {
                        yield return GetParameterTestCase(x, y => new MonthParameter(y), GetDefaultRenderParameterValueCallback<MonthParameter, Month>(RenderAnnotatedEnumValue)).ToArray();
                        yield return GetParameterTestCase(x, y => new AnnotatedMonthParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedMonthParameter, Month>(RenderAnnotatedEnumValue, annotated_month)).ToArray();
                    }

                    yield return GetParameterTestCase(default, _ => new WeekdayParameter(), GetDefaultRenderParameterValueCallback<WeekdayParameter, AnnotatedWeekday>(RenderAnnotatedEnumValue)).ToArray();
                    yield return GetParameterTestCase(default, _ => new AnnotatedWeekdayParameter(), GetDefaultRenderParameterValueCallback<AnnotatedWeekdayParameter, AnnotatedWeekday>(RenderAnnotatedEnumValue, annotated_weekday)).ToArray();

                    // Ditto Enumerated Default Values...
                    foreach (var x in GetEnumValues<AnnotatedWeekday>().Skip(1))
                    {
                        yield return GetParameterTestCase(x, y => new WeekdayParameter(y), GetDefaultRenderParameterValueCallback<WeekdayParameter, AnnotatedWeekday>(RenderAnnotatedEnumValue)).ToArray();
                        yield return GetParameterTestCase(x, y => new AnnotatedWeekdayParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedWeekdayParameter, AnnotatedWeekday>(RenderAnnotatedEnumValue, annotated_weekday)).ToArray();
                    }

                    const int precision = 3;

                    yield return GetParameterTestCase(default, _ => new DoubleParameter(), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();
                    yield return GetParameterTestCase(default, _ => new AnnotatedDoubleParameter(), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();

                    yield return GetParameterTestCase(NegativeInfinity, y => new DoubleParameter(y), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();
                    yield return GetParameterTestCase(PositiveInfinity, y => new DoubleParameter(y), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();
                    yield return GetParameterTestCase(NaN, y => new DoubleParameter(y), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();

                    yield return GetParameterTestCase(NegativeInfinity, y => new AnnotatedDoubleParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();
                    yield return GetParameterTestCase(PositiveInfinity, y => new AnnotatedDoubleParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();
                    yield return GetParameterTestCase(NaN, y => new AnnotatedDoubleParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();

                    foreach (var x in GetRange<double>(1, 2))
                    {
                        yield return GetParameterTestCase(x, y => new DoubleParameter(y), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();
                        yield return GetParameterTestCase(-x, y => new DoubleParameter(y), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();

                        yield return GetParameterTestCase(x, y => new AnnotatedDoubleParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();
                        yield return GetParameterTestCase(-x, y => new AnnotatedDoubleParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();
                    }
                }

                return _privateCases ?? (_privateCases = GetAll().ToArray());
            }
        }

        protected override IEnumerable<object[]> Cases => PrivateCases;
    }
}
