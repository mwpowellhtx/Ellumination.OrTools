﻿namespace Ellumination.OrTools.Sat.Parameters
{
    using static Names;

    [ParameterName(annotated_integer_repeated)]
    public class AnnotatedIntegerRepeatedParameter : IntegerRepeatedParameter
    {
        public AnnotatedIntegerRepeatedParameter()
        {
        }

        public AnnotatedIntegerRepeatedParameter(int value, params int[] others) : base(value, others)
        {
        }
    }
}
