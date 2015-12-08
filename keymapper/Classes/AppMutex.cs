using System;

namespace KeyMapper.Classes
{
    internal class AppMutex : IDisposable
	{
        private bool _disposed;
	    private System.Threading.Mutex _appMutex;

		public bool GetMutex()
		{
			bool acquired;
			_appMutex = new System.Threading.Mutex(true, "KeyMapperAppMutex", out acquired);
			
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
			if (_disposed == false)
			{
				if (disposing)
				{
					_appMutex.ReleaseMutex();
					_appMutex.Close();
				}

				_disposed = true;
			}
		}
	}
}
