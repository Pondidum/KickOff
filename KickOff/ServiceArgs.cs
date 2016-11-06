using System;

namespace KickOff
{
	public class ServiceArgs
	{
		public bool CancelRequested => _checkForCancel();
		public string[] StartArgs { get; }

		private readonly Func<bool> _checkForCancel;

		public ServiceArgs(string[] startArgs, Func<bool> checkForCancel)
		{
			StartArgs = startArgs;
			_checkForCancel = checkForCancel;
		}
	}
}
