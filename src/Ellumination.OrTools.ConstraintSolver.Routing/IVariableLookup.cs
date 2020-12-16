namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Provides a Variable Lookup. Assumes that the Variable can be indexed
    /// by a <see cref="long"/> Index.
    /// </summary>
    public interface IVariableLookup
    {
        /// <summary>
        /// Returns the <see cref="IntVar"/> Variable corresponding to the
        /// <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IntVar Get(long index);

        /// <summary>
        /// Provides a Get only Variable <see cref="IntVar"/> indexer.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IntVar this[long index] { get; }
    }
}
