using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceContainer.Stages
{
	public class RunnerStage : Stage
	{
		private readonly CancellationTokenSource _source;
		private readonly ServiceArgs _serviceArgs;
		private readonly Task _entryPoint;

		public RunnerStage(Type entryPoint, string[] startArgs)
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
}
