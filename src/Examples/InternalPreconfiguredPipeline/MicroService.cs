using InternalPreconfiguredPipeline.Stages;
using KickOff;
using KickOff.Host.Windows;
using KickOff.Stages;

namespace InternalPreconfiguredPipeline
{
	public class MicroService
	{
		public static void Run(string name)
		{
			ServiceHost.Run(name, new IStage[]
			{
				new ConfigureContainerStage(),
				new LoggingStage(name),
				new ConsulStage(),
				new AsyncRunnerStage()
			});
		}
	}
}
