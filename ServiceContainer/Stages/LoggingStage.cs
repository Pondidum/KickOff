using System;
using System.IO;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace ServiceContainer.Stages
{
	public class LoggingStage : Stage
	{
		private readonly string _serviceName;

		public LoggingStage(string serviceName)
		{
			_serviceName = serviceName;
		}

		public override void Execute()
		{
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			var logs = Path.Combine(baseDirectory, "logs");

			Directory.CreateDirectory(logs);

			var logConfig = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.Enrich.WithProperty("SoftwareName", _serviceName)
				.WriteTo.ColoredConsole()
				.WriteTo.RollingFile(logs);

			var config = Container.TryGetInstance<ILogConfig>();
			if (config != null && config.EnableKibana)
				logConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(config.LoggingEndpoint) { AutoRegisterTemplate = true });

			Log.Logger = logConfig.CreateLogger();

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;

			if (ex == null)
				return;

			Log.ForContext<ServiceWrapper>().Error(ex, ex.Message);
		}

		public override void Dispose()
		{
			AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
		}
	}
}
