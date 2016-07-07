using System;
using System.IO;
using System.ServiceProcess;
using Serilog;

namespace ServiceContainer
{
	public class ServiceHost
	{
		public static void Run<TStartup>(string name) where TStartup : IStartup
		{
			ConfigureLogging(name);

			var service = new ServiceWrapper(name, typeof(TStartup));

			if (Environment.UserInteractive)
			{
				Console.WriteLine("Running Console...");
				Console.WriteLine("Press any key to exit");

				service.Start(new string[0]);

				Console.ReadKey();

				service.Stop();
			}
			else
			{
				ServiceBase.Run(service);
			}
		}

		private static void ConfigureLogging(string serviceName)
		{
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			var logs = Path.Combine(baseDirectory, "logs");

			Directory.CreateDirectory(logs);

			Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.Enrich.WithProperty("SoftwareName", serviceName)
				.WriteTo.ColoredConsole()
				.WriteTo.RollingFile(logs)
				.CreateLogger();
		}
	}
}
