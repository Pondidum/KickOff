using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace KickOff.Tests
{
	public class ServiceWrapperTests
	{
		[Fact]
		public void Start_args_get_passed_to_all_stages()
		{
			var receivedArgs = new List<string[]>();

			var first = new TestStage(onExecute: (s) => { });
			var second = new TestStage(onExecute: (s) => { });

			var wrapper = new ServiceWrapper("Test", new[] { first, second });
			var startArgs = new []{ "a", "b", "c" };

			wrapper.Start(startArgs);

			receivedArgs.ShouldBe(new[] { startArgs, startArgs });
		}
	}
}
