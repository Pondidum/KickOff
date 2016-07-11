using System;
using System.Collections.Generic;
using System.Linq;
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

		public abstract void Execute();
		public abstract void Dispose();
	}

	public class LoggingStage : Stage
	{
		private static readonly ILogger Log = Serilog.Log.ForContext<ServiceWrapper>();

		public override void Execute()
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
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
		public override void Execute()
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
		private readonly CancellationTokenSource _source;
		private readonly ServiceArgs _serviceArgs;
		private readonly Task _entryPoint;

		public EndStage(Type entryPoint, string[] startArgs)
		{

			_source = new CancellationTokenSource();

			_serviceArgs = new ServiceArgs(startArgs, () => _source.IsCancellationRequested);

			_entryPoint = new Task(() =>
			{
				IStartup startup = null;

				try
				{
					startup = (IStartup)Container.GetInstance(entryPoint);
					startup.Execute(_serviceArgs);
				}
				catch (TaskCanceledException)
				{
				}
				finally
				{
					(startup as IDisposable)?.Dispose();
				}
			}, _source.Token);
		}

		public override void Execute()
		{
			_entryPoint.Start();
		}

		public override void Dispose()
		{
			try
			{
				_source.Cancel();
			}
			catch (TaskCanceledException)
			{
			}
		}
	}

	public class Pipeline : IDisposable
	{
		private readonly List<Stage> _stages;
		private readonly IContainer _container;

		public Pipeline(IContainer container)
		{
			_stages = new List<Stage>();
			_container = container;
		}

		public void Execute(Type entryPoint, string[] startArgs)
		{
			var stages = new Stage[] { new LoggingStage(), new ConsulStage(), new EndStage(entryPoint, startArgs) };

			foreach (var stage in stages)
			{
				stage.Container = _container;
				stage.Execute();
			}

			_stages.AddRange(stages);
		}

		public void Dispose()
		{
			_stages.Reverse();
			foreach (var stage in _stages)
			{
				stage.Dispose();
			}
		}
	}


	internal class ServiceWrapper : ServiceBase
	{
		private readonly Type _entryPoint;
		private readonly Pipeline _pipeline;

		public ServiceWrapper(IContainer container, string name, Type entryPoint)
		{
			ServiceName = name;

			_entryPoint = entryPoint;
			_pipeline = new Pipeline(container);
		}

		public void Start(string[] args)
		{
			OnStart(args);
		}

		protected override void OnStart(string[] args)
		{
			_pipeline.Execute(_entryPoint, args);
		}

		protected override void OnStop()
		{
			_pipeline.Dispose();
		}
	}
}
