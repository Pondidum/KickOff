using System;

namespace ServiceContainer
{
	public interface IStartup
	{
		void Execute(ServiceArgs service);
	}

	public class ServiceArgs
	{
		public bool CancelRequested => _checkForCancel();

		private readonly Func<bool> _checkForCancel;

		public ServiceArgs(Func<bool> checkForCancel)
		{
			_checkForCancel = checkForCancel;
		}
	}
}
