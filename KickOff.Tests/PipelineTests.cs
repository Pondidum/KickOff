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
			pipeline.Execute(stages, new string[0]);

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
			pipeline.Execute(stages, new string[0]);
			pipeline.Dispose();

			pipeline.ShouldSatisfyAllConditions(
				() => executionOrder.ShouldBe(new[] { first, second }),
				() => disposalOrder.ShouldBe(new[] { second, first })
			);
		}

		[Fact]
		public void A_replacement_of_the_container_gets_propegated()
		{
			Func<Type, object> container = x => new object();
			Func<Type, object> instanceFactorySeen = null;

			var replacer = new TestStage(onExecute: (ts, a) => a.InstanceFactory = container);
			var next = new TestStage(onExecute: (ts, a) => instanceFactorySeen = a.InstanceFactory);

			var pipeline = new Pipeline();
			pipeline.Execute(new[] { replacer, next }, new string[0]);

			instanceFactorySeen.ShouldBe(container);
		}

		[Fact]
		public void The_args_used_in_stop_are_same_as_start()
		{
			StageArgs startArgs = null;
			StageArgs stopArgs = null;

			var stage = new TestStage((ts, a) => startArgs = a, (ts, a) => stopArgs = a);

			var pipeline = new Pipeline();
			pipeline.Execute(new[] { stage }, new string[0]);
			pipeline.Dispose();

			stopArgs.ShouldBe(startArgs);
		}
	}
}