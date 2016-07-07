using System;

namespace ServiceContainer
{
	public class ServiceArgs
	{
		public bool CancelRequested => _checkForCancel();
		public string[] StartArgs { get; internal set; }

		private readonly Func<bool> _checkForCancel;

		public ServiceArgs(Func<bool> checkForCancel)
		{
			_checkForCancel = checkForCancel;
		}
	}
}
