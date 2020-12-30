//using System;
//using System.Collections.Generic;
//using System.Text;

//// TODO: TBD: given our "Core" assembly does not make a reference to Google.OrTools intentionally, this may not be the best factoring...
//// TODO: TBD: we may reconsider whether such a dependency is warranted after all...
//namespace Ellumination.OrTools.Enumerations
//{
//    using OptionalBooleanType = Google.OrTools
//    /// <summary>
//    /// A &quot;three-way&quot; or <em>tri-state</em> boolean: <c>unspecified</c>, <c>false</c>
//    /// or <c>true</c>. We do not use the value of <c>1</c> to increase the chance to catch bugs.
//    /// <br/>
//    /// <br/>
//    /// In <em>python</em>, a user may set a proto field of this type enum to a <c>boolean</c>
//    /// value without type checks, if they set it to <c>True</c>, the proto validity code will
//    /// catch it, because it will be cast to <c>1</c>, which is an invalid enum value. Note that
//    /// if the user sets if to <c>False</c>, i.e. <c>0</c>, it will be caught by the routing
//    /// library parameter validity check as well.
//    /// </summary>
//    /// <see cref="!:https://python.org"/>
//    public enum OptionalBoolean
//    {
//        /// <summary>
//        /// <c>0</c>
//        /// </summary>
//        BoolUnspecified = 0,

//        /// <summary>
//        /// <c>2</c>
//        /// </summary>
//        BoolFalse = 2,

//        /// <summary>
//        /// <c>3</c>
//        /// </summary>
//        BoolTrue = 3
//    }
//}
