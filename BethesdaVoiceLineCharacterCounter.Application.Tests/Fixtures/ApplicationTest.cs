using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethesdaVoiceLineCharacterCounter.Application.Tests.Fixtures
{
    /// <summary>
    /// This class will function for 
    /// </summary>
    public class ApplicationTest : IDisposable, IAsyncDisposable
    {
        #region [ Variables ]

        private bool disposedValue;

        #endregion

        public ApplicationTest() 
        { 
        
        }

        #region [ Destructor ]

        ~ApplicationTest()
        {
            Dispose(false);
        }

        #endregion

        #region [ IDisposable Implementation ]

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ApplicationTest()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }



        #endregion

        #region [ IAsyncDisposable Implementation ]

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);

            GC.SuppressFinalize(this);
        }

        protected virtual ValueTask DisposeAsyncCore()
        {
            return ValueTask.CompletedTask;
        }

        #endregion
    }
}
