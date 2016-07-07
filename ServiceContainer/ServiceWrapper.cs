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
		private readonly Container _container;
		private readonly ServiceArgs _serviceArgs;

		public ServiceWrapper(string name, Type entryPoint)
		{
			ServiceName = name;

			_token = new CancellationTokenSource();
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

			_serviceArgs = new ServiceArgs(() => _token.IsCancellationRequested);

			_entryPoint = new Task(() =>
			{
				IStartup startup = null;

				try
				{
					startup = (IStartup)_container.GetInstance(entryPoint);
					startup.Execute(_serviceArgs);
				}
				catch (TaskCanceledException)
				{
				}
				finally
				{
					(startup as IDisposable)?.Dispose();
				}
			}, _token.Token);
		}

		public void Start(string[] args)
		{
			OnStart(args);
		}

		protected override void OnStart(string[] args)
		{
			_serviceArgs.StartArgs = args;
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
