using System;

namespace KeyMapper.Classes
{
    internal class AppMutex : IDisposable
	{
        private bool disposed;
	    private System.Threading.Mutex appMutex;

		public bool GetMutex()
		{
            appMutex = new System.Threading.Mutex(true, "KeyMapperAppMutex", out bool acquired);
			
			return acquired;
		}

	    ~AppMutex()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                appMutex.ReleaseMutex();
                appMutex.Close();
            }

            disposed = true;
        }
	}
}
