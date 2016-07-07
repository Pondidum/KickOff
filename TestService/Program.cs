using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceContainer;

namespace TestService
{
	class Program
	{
		static void Main(string[] args)
		{

			ServiceHost.Run<LogWriter>("TestService");

			Thread.Sleep(5000);
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


			while(service.CancelRequested == false)
				Thread.Sleep(500);
		}

		public void Dispose()
		{
			Console.WriteLine("shutting down");
		}
	}
}
