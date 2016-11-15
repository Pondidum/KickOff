using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace KickOff
{
	public static class ServiceHost
	{
		public static void Run(string name, IEnumerable<IStage> stages, PipelineCustomisation customiser = null)
		{
			var service = new ServiceWrapper(name, stages, customiser);

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
