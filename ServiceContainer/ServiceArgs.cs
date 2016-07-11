using System;

namespace ServiceContainer
{
	public class ServiceArgs
	{
		public bool CancelRequested => _checkForCancel();
		public string[] StartArgs { get; private set; }

		private readonly Func<bool> _checkForCancel;

		public ServiceArgs(string[] startArgs, Func<bool> checkForCancel)
		{
			StartArgs = startArgs;
			_checkForCancel = checkForCancel;
		}
	}
}
