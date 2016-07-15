using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using ServiceContainer;

namespace TestService
{
	class Program
	{
		static void Main(string[] args)
		{

			ServiceHost.Run<LogWriter>("TestService");
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
			File.AppendAllLines(@"D:\dev\test-projects\ServiceContainer\\log.txt", new[] { "boot!" });


			while (service.CancelRequested == false)
				Thread.Sleep(500);
		}

		public void Dispose()
		{
			Console.WriteLine("shutting down");
		}
	}
}
