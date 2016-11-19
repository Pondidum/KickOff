using System.Collections.Generic;
using System.Linq;
using KickOff.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;
using Xunit;

namespace KickOff.Tests
{
	public class ServiceWrapperTests
	{
		[Fact]
		public void Start_args_get_passed_to_all_stages()
		{
			var stage = Substitute.For<IStage>();

			var wrapper = new ServiceWrapper("Test", new[] { stage }, new PipelineCustomisation());
			wrapper.Start(new string[0]);

			stage.Received().OnStart(Arg.Any<StageArgs>());
		}

		[Fact]
		public void When_the_service_is_stopped_so_is_the_pipeline()
		{
			var stage = Substitute.For<IStage>();

			var wrapper = new ServiceWrapper("Test", new[] { stage }, new PipelineCustomisation());
			wrapper.Stop();

			stage.Received().OnStop(Arg.Any<StageArgs>());
		}
	}
}
