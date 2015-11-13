namespace GitTools
{
    using System;
    using Logging;

    /// <summary>
    /// Base class for disposable objects.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        #region Fields
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly object _syncRoot = new object();

        private bool _disposing;
        #endregion

        #region Constructors
        /// <summary>
        /// Finalizes an instance of the <see cref="Disposable"/> class.
        /// </summary>
        ~Disposable()
        {
            Dispose(false);
        }
        #endregion

        #region Properties
        private bool IsDisposed { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Checks whether the object is disposed. If so, it will throw the <see cref="ObjectDisposedException"/>.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">The object is disposed.</exception>
        protected void CheckDisposed()
        {
            lock (_syncRoot)
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
            }
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected virtual void DisposeManaged()
        {
        }

        /// <summary>
        /// Disposes the unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanaged()
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool isDisposing)
        {
            lock (_syncRoot)
            {
                if (!IsDisposed)
                {
                    if (!_disposing)
                    {
                        _disposing = true;

                        if (isDisposing)
                        {
                            try
                            {
                                DisposeManaged();
                            }
                            catch (Exception ex)
                            {
                                //if (ex.IsCritical())
                                //{
                                //    throw;
                                //}

                                Log.ErrorException("Error while disposing managed resources of '{0}'.", ex, GetType().FullName);
                            }
                        }

                        try
                        {
                            DisposeUnmanaged();
                        }
                        catch (Exception ex)
                        {
                            //if (ex.IsCritical())
                            //{
                            //    throw;
                            //}

                            Log.ErrorException("Error while disposing unmanaged resources of '{0}'.", ex, GetType().FullName);
                        }

                        IsDisposed = true;
                        _disposing = false;
                    }
                }
            }
        }
        #endregion
    }
}