using System;
using System.ServiceProcess;

namespace ServiceContainer
{
	public class ServiceHost
	{
		public static void Run(string name, Action entryPoint)
		{
			if (Environment.UserInteractive)
			{
				Console.WriteLine("Running Console...");
				Console.WriteLine("Press any key to exit");

				entryPoint();

				Console.ReadKey();
			}
			else
			{
				var service = new ServiceWrapper(name, entryPoint);

				ServiceBase.Run(service);
			}
		}
	}

	internal class ServiceWrapper : ServiceBase
	{
		private readonly Action _entryPoint;

		public ServiceWrapper(string name, Action entryPoint)
		{
			_entryPoint = entryPoint;
			ServiceName = name;
		}

		protected override void OnStart(string[] args)
		{
			_entryPoint();
		}
	}
}
