using System;

namespace KeyMapper
{
	class AppMutex : IDisposable
	{

		private bool _disposed = false;
		System.Threading.Mutex _appMutex;

		public bool GetMutex()
		{
			bool acquired;
			_appMutex = new System.Threading.Mutex(true, "KeyMapperAppMutex", out acquired);
			
			return acquired;
		}

		#region Finalizer & IDisposable implementation

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

		#endregion

	}
}
