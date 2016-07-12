using System;
using System.IO;
using System.ServiceProcess;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using StructureMap;
using StructureMap.Graph;

namespace ServiceContainer
{
	public static class ServiceHost
	{
		public static void Run<TStartup>(string name) where TStartup : IStartup
		{
			var container = new Container(c =>
			{
				c.Scan(a =>
				{
					a.TheCallingAssembly();
					a.AssemblyContainingType<TStartup>();

					a.LookForRegistries();
					a.WithDefaultConventions();
				});
			});

			var config = container.TryGetInstance<ILogConfig>();

			var service = new ServiceWrapper(container, name, typeof(TStartup));

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
	}
}
