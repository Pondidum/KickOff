using KickOff.Tests.TestInfrastructure;
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
	}
}
