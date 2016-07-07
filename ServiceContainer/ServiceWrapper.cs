using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using StructureMap;
using StructureMap.Graph;

namespace ServiceContainer
{
	internal class ServiceWrapper : ServiceBase
	{
		private readonly Task _entryPoint;
		private readonly CancellationTokenSource _token;
		private Container _container;

		public ServiceWrapper(string name, Type entryPoint)
		{
			ServiceName = name;

			_container = new Container(c =>
			{
				c.Scan(a =>
				{
					a.TheCallingAssembly();
					a.AssemblyContainingType(entryPoint);

					a.LookForRegistries();
					a.WithDefaultConventions();
				});
			});

			_token = new CancellationTokenSource();
			_entryPoint = new Task(() =>
			{
				try
				{
					startup = (IStartup)_container.GetInstance(entryPoint);
					startup.Execute(args);
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
			finally
			{
				_container.Dispose();
			}

		}
	}
}
