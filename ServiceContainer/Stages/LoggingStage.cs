using System;
using Serilog;

namespace ServiceContainer.Stages
{
	public class LoggingStage : Stage
	{
		private static readonly ILogger Log = Serilog.Log.ForContext<ServiceWrapper>();

		public override void Execute()
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;

			if (ex == null)
				return;

			Log.Error(ex, ex.Message);
		}

		public override void Dispose()
		{
			AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
		}
	}
}
