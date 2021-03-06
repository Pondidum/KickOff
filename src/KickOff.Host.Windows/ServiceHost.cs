﻿using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace KickOff.Host.Windows
{
	public static class ServiceHost
	{
		public static void Run(string name, IEnumerable<IStage> stages)
		{
			var service = new ServiceWrapper(name, stages);

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
