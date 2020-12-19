namespace System
{
    /// <inheritdoc/>
    public abstract class Disposable : IDisposableEx
    {
        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc/>
        public event EventHandler<EventArgs> Disposing;

        /// <summary>
        /// <see cref="Disposing"/> Event dispatcher.
        /// </summary>
        /// <param name="e"></param>
        protected void OnDisposing(EventArgs e) => this.Disposing?.Invoke(this, e);

        /// <summary>
        /// <see cref="Disposing"/> Event dispatcher.
        /// </summary>
        protected virtual void OnDisposing() => this.OnDisposing(EventArgs.Empty);

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this.OnDisposing();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            this.IsDisposed = true;
        }
    }
}
