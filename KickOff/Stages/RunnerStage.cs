using System;
using System.Threading;
using System.Threading.Tasks;

namespace KickOff.Stages
{
	public class RunnerStage : Stage
	{
		private readonly CancellationTokenSource _source;
		private readonly ServiceArgs _serviceArgs;
		private readonly Task _runner;
		private IStartup _startup;

		public RunnerStage(string[] startArgs)
		{
			_source = new CancellationTokenSource();

			_serviceArgs = new ServiceArgs(startArgs, () => _source.IsCancellationRequested);

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

		public override void OnStart(StageArgs args)
		{
			_startup = args.TryGetInstance<IStartup>();
			_runner.Start();
		}

		public override void OnStop(StageArgs args)
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
