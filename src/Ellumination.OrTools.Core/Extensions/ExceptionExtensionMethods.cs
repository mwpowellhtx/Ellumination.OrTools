using System;
using System.Linq;

namespace Ellumination.OrTools
{
    internal static class ExceptionExtensionMethods
    {
        /// <summary>
        /// Returns an instance of <typeparamref name="TException"/> given
        /// <paramref name="args"/>.
        /// </summary>
        /// <typeparam name="TException">The <see cref="Exception"/> to be Thrown.</typeparam>
        /// <param name="_">The root object anchoring the extension method.</param>
        /// <param name="args">The Arguments being relayed to the <typeparamref name="TException"/>.</param>
        /// <returns></returns>
        public static TException ThrowException<TException>(this object _, params object[] args)
            where TException : Exception =>
                !args.Any()
                    ? Activator.CreateInstance<TException>()
                    : (TException)Activator.CreateInstance(typeof(TException), args);
    }
}
