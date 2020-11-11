﻿using System.Linq;

namespace Ellumination.OrTools.Sat.Parameters
{
    using static Ordinals;

    public class BooleanRepeatedParameter : RepeatedParameter<bool>
    {
        public BooleanRepeatedParameter() : this(default)
        {
        }

        public BooleanRepeatedParameter(bool value, params bool[] others) : base(new[] {value}.Concat(others), InternalOrdinal)
        {
        }
    }
}
