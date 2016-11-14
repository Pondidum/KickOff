using System;

namespace KickOff.Stages
{
	public class RunnerStage : IStage
	{
		private IStartup _startup;

		public void OnStart(StageArgs args)
		{
			_startup = args.TryGetInstance<IStartup>();

			if (_startup == null)
				throw new StartupNotFoundException();

			_startup.Execute(new ServiceArgs(args.StartArgs, () => false));
		}

		public void OnStop(StageArgs args)
		{
			(_startup as IDisposable)?.Dispose();
		}
	}
}
