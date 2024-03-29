﻿namespace BethesdaVoiceLineCharacterCounter.Application.Tests.Fixtures
{
    /// <summary>
    /// This class will function for 
    /// </summary>
    public class ApplicationTest : IDisposable, IAsyncDisposable
    {
        #region [ Variables ]

        private bool disposedValue;

        #endregion

        #region [ Constructors ]

        public ApplicationTest() 
        { 
        
        }

        #endregion

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
