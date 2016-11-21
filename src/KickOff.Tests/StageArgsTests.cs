using KickOff.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;
using Xunit;

namespace KickOff.Tests
{
	public class StageArgsTests
	{
		private readonly StageArgs _args;

		public StageArgsTests()
		{
			_args = new StageArgs(new string[0]);
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

		private class SomeDto
		{
			public int Value { get; set; }
		}
	}
}
