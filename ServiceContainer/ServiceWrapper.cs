using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceContainer
{
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

		public void Start(string[] args)
		{
			OnStart(args);
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
