//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Ellumination.OrTools.ConstraintSolver.Routing
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public static class OptionalBooleanExtensionMethods
//    {
//        //
//        private static IDictionary<OptionalBoolean, bool?> OptionalBooleanMap { get; } = new Dictionary<OptionalBoolean, bool?>
//        {
//            { OptionalBoolean.BoolTrue, true }
//            , { OptionalBoolean.BoolFalse, false }
//            , { OptionalBoolean.BoolUnspecified, null }
//        };

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public static bool? AsBoolean(this OptionalBoolean value) =>
//            OptionalBooleanMap.TryGetValue(value, out var result) ? result : null;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public static OptionalBoolean AsOptionalBoolean(this bool? value)
//        {
//            var mapped = OptionalBooleanMap.Where(_ => _.Value == value).ToArray();
//            return mapped.Length == 1 ? mapped.First().Key : OptionalBoolean.BoolUnspecified;
//        }
//    }
//}
