using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using StructureMap;
using StructureMap.Graph;

namespace ServiceContainer
{
	internal class ServiceWrapper : ServiceBase
	{
		private static readonly ILogger Log = Serilog.Log.ForContext<ServiceWrapper>();

		private readonly Task _entryPoint;
		private readonly CancellationTokenSource _token;
		private readonly Container _container;
		private readonly ServiceArgs _serviceArgs;

		public ServiceWrapper(string name, Type entryPoint)
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			ServiceName = name;

			_token = new CancellationTokenSource();

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
				AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
			}
		}


		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;

			if (ex == null)
				return;

			Log.Error(ex, ex.Message);
		}
	}
}
