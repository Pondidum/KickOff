using System.Threading;
using KickOff.Stages;
using NSubstitute;
using Shouldly;
using Xunit;

namespace KickOff.Tests.Stages
{
	public class AsyncRunnerStageTests
	{
		private readonly AsyncRunnerStage _asyncRunner;

		public AsyncRunnerStageTests()
		{
			_asyncRunner = new AsyncRunnerStage();
		}

		[Fact]
		public void Execute_returns_immediately()
		{
			var startup = Substitute.For<IStartup>();

			_asyncRunner.OnStart(new StageArgs(new PipelineCustomisation(), new string[0])
			{
				InstanceFactory = type => startup
			});

			Thread.Sleep(50);

			startup.Received().Execute(Arg.Any<ServiceArgs>());
		}

		[Fact]
		public void When_an_IStartup_implementation_cannot_be_found()
		{
			var args = new StageArgs(new PipelineCustomisation(), new string[0])
			{
				InstanceFactory = type => null
			};

			Should.Throw<StartupNotFoundException>(() => _asyncRunner.OnStart(args));
		}
	}
}
