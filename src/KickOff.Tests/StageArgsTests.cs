using KickOff.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;
using Xunit;

namespace KickOff.Tests
{
	public class StageArgsTests
	{
		private readonly StageArgs _args;
		private readonly PipelineCustomisation _customisations;

		public StageArgsTests()
		{
			_customisations = Substitute.For<PipelineCustomisation>();
			_args = new StageArgs(_customisations, new string[0]);
		}

		[Fact]
		public void The_default_instance_factory_can_find_istartup_implementation()
		{
			_args
				.TryGetInstance<IStartup>()
				.ShouldBeOfType<TestApp>();
		}

		[Fact]
		public void The_default_instance_factory_can_create_a_simple_object()
		{
			_args
				.TryGetInstance<SomeDto>()
				.ShouldNotBeNull();
		}

		[Fact]
		public void Applying_Customisiations_call_is_passed_through()
		{
			var dto = new SomeDto();

			_args.ApplyCustomisationTo(dto);

			_customisations.Received().Apply(dto);
		}

		private class SomeDto
		{
			public int Value { get; set; }
		}
	}
}
