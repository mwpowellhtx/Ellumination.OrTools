﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.Sat.Parameters
{
    using static Characters;
    using static String;

    internal class ParameterCollectionRenderingTestCases : ParameterTestCasesBase
    {
        private static IEnumerable<object[]> _privateCases;

        protected override IEnumerable<object[]> Cases
        {
            get
            {
                // ReSharper disable PossibleMultipleEnumeration
                IEnumerable<object[]> GetAll()
                {
                    IEnumerable<TestCaseDescriptor> descriptors = Descriptors.ToArray();

                    /* Seven is a nominal slice here, which yields for a narrow enough slice,
                     * without generating an excessive number of test cases at the same time. */

                    while (descriptors.TrySliceOverCollection(7, out var slice, out descriptors))
                    {
                        var parameters = slice.Select(p => p.Instance).ToArray();

                        yield return GetRange<object>(
                            new ParameterCollection(slice.Select(p => p.Instance))
                            , parameters
                            , Join($"{SemiColon} ", slice.Select(p => p.Rendered))
                        ).ToArray();
                    }
                }
                // ReSharper restore PossibleMultipleEnumeration

                return _privateCases ?? (_privateCases = GetAll().ToArray());
            }
        }
    }
}
