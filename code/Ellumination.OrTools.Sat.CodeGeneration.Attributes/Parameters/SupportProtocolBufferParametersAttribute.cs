using System;
using System.Diagnostics;

namespace Ellumination.OrTools.Sat.CodeGeneration.Attributes
{
    using Code.Generation.Roslyn;

    /// <summary>
    /// The Code Generator must be named CpSatParametersGenerator in the
    /// Ellumination.OrTools.Sat.CodeGeneration.Generators namespace furnished from a separate
    /// assembly.
    /// </summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Assembly)
     , CodeGenerationAttribute("Ellumination.OrTools.Sat.CodeGeneration.Generators.CpSatParametersAssemblyCodeGenerator, Ellumination.OrTools.Sat.CodeGeneration")
     , Conditional("CodeGeneration")]
    public class SupportProtocolBufferParametersAttribute : Attribute
    {
    }
}
