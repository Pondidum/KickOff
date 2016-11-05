using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace KickOff.Tests
{
	public class PipelineTests
	{
		[Fact]
		public void When_a_pipeline_is_run()
		{
			var executionOrder = new List<Stage>();
			var disposalOrder = new List<Stage>();

			var first = new TestStage(executionOrder.Add, disposalOrder.Add);
			var second = new TestStage(executionOrder.Add, disposalOrder.Add);

			var stages = new[] { first, second };

			var pipeline = new Pipeline();
			pipeline.Execute(stages);

			pipeline.ShouldSatisfyAllConditions(
				() => executionOrder.ShouldBe(stages),
				() => disposalOrder.ShouldBeEmpty()
			);
		}

		[Fact]
		public void When_a_pipeline_is_disposed()
		{
			var executionOrder = new List<Stage>();
			var disposalOrder = new List<Stage>();

			var first = new TestStage(executionOrder.Add, disposalOrder.Add);
			var second = new TestStage(executionOrder.Add, disposalOrder.Add);

			var stages = new[] { first, second };

			var pipeline = new Pipeline();
			pipeline.Execute(stages);
			pipeline.Dispose();

			pipeline.ShouldSatisfyAllConditions(
				() => executionOrder.ShouldBe(new[] { first, second }),
				() => disposalOrder.ShouldBe(new[] { second, first })
			);
		}

		private class TestStage : Stage
		{
			private readonly Action<Stage> _onExecute;
			private readonly Action<Stage> _onDispose;

			public TestStage(Action<Stage> onExecute, Action<Stage> onDispose)
			{
				_onExecute = onExecute;
				_onDispose = onDispose;
			}

			public override void Execute() => _onExecute(this);

			public override void Dispose() => _onDispose(this);
		}

	}
}