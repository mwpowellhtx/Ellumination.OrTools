using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Provides a set of helpful Context extension methods for use when
    /// configuring <see cref="Context.Dimensions"/> among other things.
    /// </summary>
    public static class ContextExtensionMethods
    {
        /// <summary>
        /// Returns an <see cref="Activator.CreateInstance(Type, object[])"/>
        /// <typeparamref name="T"/> instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        private static T GetActivatorCreateInstance<T>(params object[] args) =>
            (T)Activator.CreateInstance(typeof(T), args);

        /// <summary>
        /// Adds the <typeparamref name="TDimension"/> corresponding with the
        /// <typeparamref name="TContext"/> <paramref name="context"/>. Activates the
        /// <typeparamref name="TDimension"/> instance using the public constructor
        /// accepting a <typeparamref name="TContext"/> <paramref name="context"/>
        /// at minimum. We do not advise your Dimension constructors being much more
        /// elaborate than that. At most, Dimension implementations should also require
        /// a Coefficient, but that is about all you should require for an adequate
        /// Dimension constructor.
        /// </summary>
        /// <typeparam name="TContext">The Context type.</typeparam>
        /// <typeparam name="TDimension">The Dimension type.</typeparam>
        /// <param name="context">The Context for which the <see cref="IDimension"/>
        /// should be configured.</param>
        /// <returns></returns>
        /// <remarks>The activity of creating a <typeparamref name="TDimension"/> instance
        /// should route the <paramref name="context"/> through base class constructors, which
        /// subsequently adds the Dimension instance to the <see cref="Context.Dimensions"/>
        /// collection.</remarks>
        public static TContext AddDimension<TContext, TDimension>(this TContext context, params object[] args)
            where TContext : Context
            where TDimension : Dimension
        {
            IEnumerable<object> OnEnumerateArguments()
            {
                yield return context;

                foreach (var arg in args)
                {
                    yield return arg;
                }
            }

            GetActivatorCreateInstance<TDimension>(OnEnumerateArguments().ToArray());

            return context;
        }
    }
}
