namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Provides a Variable Lookup base class. Assumes that <see cref="Source"/> Variables
    /// are indexed by <see cref="long"/> indexes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VariableLookup<T> : IVariableLookup
    {
        /// <summary>
        /// Gets the Source informing the VariableLookup.
        /// </summary>
        protected T Source { get; }

        /// <summary>
        /// Constructs the Context corresponding to the <paramref name="source"/>.
        /// </summary>
        /// <param name="source"></param>
        protected VariableLookup(T source)
        {
            this.Source = source;
        }

        /// <summary>
        /// Returns the <see cref="IntVar"/> corresponding to the <paramref name="source"/>
        /// at the <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected abstract IntVar Get(T source, long index);

        /// <summary>
        /// Returns the Variable corresponding to the <see cref="Source"/>
        /// at the <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual IntVar Get(long index) => this.Get(this.Source, index);

        /// <summary>
        /// Variable Get only indexer.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <see cref="Get"/>
        public virtual IntVar this[long index] => this.Get(index);
    }
}
