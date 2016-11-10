using System.Linq;
using System.Threading;
using KickOff.Stages;
using NSubstitute;
using Shouldly;
using Xunit;

namespace KickOff.Tests.Stages
{
	public class RunnerStageTests
	{
		private readonly RunnerStage _runner;

		public RunnerStageTests()
		{
			_runner = new RunnerStage();
		}

		[Fact]
		public void Execute_returns_immediately()
		{
			var startup = Substitute.For<IStartup>();

			_runner.OnStart(new StageArgs(new string[0])
			{
				InstanceFactory = type => startup
			});

			Thread.Sleep(50);

			startup.Received().Execute(Arg.Any<ServiceArgs>());
		}

		[Fact]
		public void When_an_IStartup_implementation_cannot_be_found()
		{
			var args = new StageArgs(new string[0])
			{
				InstanceFactory = type => null
			};

			Should.Throw<StartupNotFoundException>(() => _runner.OnStart(args));
		}
	}
}
