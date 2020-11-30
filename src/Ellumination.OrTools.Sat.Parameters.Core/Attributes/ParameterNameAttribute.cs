using System;

namespace Ellumination.OrTools.Sat.Parameters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ParameterNameAttribute : Attribute
    {
        public string Name { get; }

        public ParameterNameAttribute(string name) => Name = name;
    }
}
