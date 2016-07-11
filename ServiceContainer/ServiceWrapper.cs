using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Serilog;
using StructureMap;
using StructureMap.Graph;

namespace ServiceContainer
{
	public abstract class Stage : IDisposable
	{
		public IContainer Container { get; set; }

		public abstract void Execute(Action next);
		public abstract void Dispose();
	}

	public class LoggingStage : Stage
	{
		private static readonly ILogger Log = Serilog.Log.ForContext<ServiceWrapper>();

		public override void Execute(Action next)
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			next();
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;

			if (ex == null)
				return;

			Log.Error(ex, ex.Message);
		}

		public override void Dispose()
		{
			AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
		}
	}

	public class ConsulStage : Stage
	{
		public override void Execute(Action next)
		{
			var registration = Container.TryGetInstance<IConsulRegistration>();

			if (registration != null)
			{
				var client = new ConsulClient();
				client.Catalog.Register(registration.CreateRegistration());
			}
		}

		public override void Dispose()
		{
			var registration = Container.TryGetInstance<IConsulRegistration>();

			if (registration != null)
			{
				var client = new ConsulClient();
				client.Catalog.Deregister(registration.CreateDeregistration());
			}
		}
	}

	public class EndStage : Stage
	{
		public EndStage(Action entryPoint)
		{
			throw new NotImplementedException();
		}

		public override void Execute(Action next)
		{
		}

		public override void Dispose()
		{
		}
	}

	public class Pipeline : IDisposable
	{
		private readonly Stack<Stage> _stages;

		public Pipeline()
		{
			_stages = new Stack<Stage>();
		} 

		public void Execute(IContainer container, Action entryPoint)
		{
			var stages = new Stage[] { new LoggingStage(), new ConsulStage(), new EndStage(entryPoint) };

			foreach (var stage in stages)
			{
				// implement! 

				//stage.Execute(...)
				_stages.Push(stage);
			}
		}

		public void Dispose()
		{
			Stage stage;
			while ((stage = _stages.Pop()) != null)
			{
				stage.Dispose();
			}
		}
	}


	internal class ServiceWrapper : ServiceBase
	{
		private static readonly ILogger Log = Serilog.Log.ForContext<ServiceWrapper>();

		private readonly Task _entryPoint;
		private readonly CancellationTokenSource _token;
		private readonly IContainer _container;
		private readonly ServiceArgs _serviceArgs;

		public ServiceWrapper(IContainer container, string name, Type entryPoint)
		{
			_container = container;
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
			var registration = _container.TryGetInstance<IConsulRegistration>();

			if (registration != null)
			{
				var client = new ConsulClient();
				client.Catalog.Register(registration.CreateRegistration());
			}

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

				var registration = _container.TryGetInstance<IConsulRegistration>();

				if (registration != null)
				{
					var client = new ConsulClient();
					client.Catalog.Deregister(registration.CreateDeregistration());
				}

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
