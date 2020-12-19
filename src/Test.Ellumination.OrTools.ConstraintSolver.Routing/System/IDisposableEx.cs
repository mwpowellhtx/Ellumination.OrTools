namespace System
{
    /// <inheritdoc/>
    public interface IDisposableEx : IDisposable
    {
        /// <summary>
        /// Event occurs on Disposing.
        /// </summary>
        event EventHandler<EventArgs> Disposing;

        /// <summary>
        /// Gets whether the object IsDisposed.
        /// </summary>
        bool IsDisposed { get; }
    }
}
