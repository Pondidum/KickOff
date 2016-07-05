using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

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
		private readonly Task _entryPoint;
		private readonly CancellationTokenSource _token;

		public ServiceWrapper(string name, Action entryPoint)
		{
			ServiceName = name;

			_token = new CancellationTokenSource();
			_entryPoint = new Task(() =>
			{
				try
				{
					entryPoint();
				}
				catch (TaskCanceledException)
				{
				}
			}, _token.Token);
		}

		protected override void OnStart(string[] args)
		{
			_entryPoint.Start();
		}

		protected override void OnStop()
		{
			try
			{
				_token.Cancel();
			}
			catch (TaskCanceledException)
			{
			}
			
		}
	}
}
