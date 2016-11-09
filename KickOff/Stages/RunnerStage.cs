using System;
using System.Threading;
using System.Threading.Tasks;

namespace KickOff.Stages
{
	public class RunnerStage : IStage
	{
		private ServiceArgs _serviceArgs;
		private IStartup _startup;

		private readonly CancellationTokenSource _source;
		private readonly Task _runner;

		public RunnerStage()
		{
			_source = new CancellationTokenSource();

			_runner = new Task(() =>
			{
				try
				{
					_startup.Execute(_serviceArgs);
				}
				catch (TaskCanceledException)
				{
				}
			}, _source.Token);
		}

		public virtual void OnStart(StageArgs args)
		{
			_startup = args.TryGetInstance<IStartup>();
			_serviceArgs = new ServiceArgs(args.StartArgs, () => _source.IsCancellationRequested);

			_runner.Start();
		}

		public virtual void OnStop(StageArgs args)
		{
			try
			{
				_source.Cancel();
				_runner.Wait();
			}
			catch (TaskCanceledException)
			{
			}

			(_startup as IDisposable)?.Dispose();
		}
	}
}
