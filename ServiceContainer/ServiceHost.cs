using System;
using System.ServiceProcess;

namespace ServiceContainer
{
	public class ServiceHost
	{
		public static void Run(string name, Action entryPoint)
		{
			var service = new ServiceWrapper(name, entryPoint);

			if (Environment.UserInteractive)
			{
				Console.WriteLine("Running Console...");
				Console.WriteLine("Press any key to exit");

				service.Start(new string[0]);

				Console.ReadKey();
			}
			else
			{
				

				ServiceBase.Run(service);
			}
		}
	}
}
