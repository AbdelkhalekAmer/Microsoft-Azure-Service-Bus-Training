using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace training.message.broker.server.Demos
{
    [SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "Ignore")]
    public abstract class Demo : IDisposable
    {
        private bool _disposed = false;

        public abstract Task RunAsync();

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Disposing(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        private void Disposing(bool disposing)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Demo));
            }

            if (disposing)
            {
                OnDemoDisposing();
            }

            _disposed = true;
        }

        protected virtual void OnDemoDisposing()
        {

        }

        ~Demo()
        {
            Disposing(false);
        }
    }
}
