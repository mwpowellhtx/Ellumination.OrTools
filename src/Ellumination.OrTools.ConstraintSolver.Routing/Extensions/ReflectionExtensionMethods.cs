namespace System.Reflection
{
    /// <summary>
    /// Establishes a handful of useful Reflection extension methods.
    /// </summary>
    internal static class ReflectionExtensionMethods
    {
        /// <summary>
        /// Returns whether <paramref name="root"/> <see cref="Type.IsAssignableFrom"/>
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <returns></returns>
        public static bool IsAssignableFrom<T>(this Type root) => root.IsAssignableFrom(typeof(T));
    }
}
