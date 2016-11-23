using System;
using System.Threading;
using Consul;
using KickOff;
using KickOff.Host.Windows;
using KickOff.Stages;
using TestService.Stages;

namespace TestService
{
	class Program
	{
		static void Main(string[] args)
		{

			ServiceHost.Run("TestService", new IStage[]
			{
				new ConfigureContainerStage(),
				new LoggingStage("TestService"),
				new ConsulStage(),
				new AsyncRunnerStage()
			});
		}
	}

	public class Config : ILogConfig, IConsulRegistration
	{
		private readonly AgentService _service;

		public Config()
		{
			_service = new AgentService
			{
				Address = "http://localhost",
				Port = 8005,
				Service = "TestService"
			};
		}

		public bool EnableKibana { get; }
		public Uri LoggingEndpoint { get; }

		public CatalogRegistration CreateRegistration()
		{
			return new CatalogRegistration() { Service = _service };
		}

		public CatalogDeregistration CreateDeregistration()
		{
			return new CatalogDeregistration { ServiceID = _service.ID };
		}
	}

	public class LogWriter : IStartup, IDisposable
	{
		public LogWriter()
		{
			Console.WriteLine("starting up");
		}

		public void Execute(ServiceArgs service)
		{
			Console.WriteLine("Started!");

			while (service.CancelRequested == false)
			{
				Console.Write(".");
				Thread.Sleep(1000);
			}

			Console.WriteLine();
		}

		public void Dispose()
		{
			Console.WriteLine("shutting down");
		}
	}
}
