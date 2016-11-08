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

		[Fact]
		public void A_replacement_of_the_container_gets_propegated()
		{
			Func<Type, object> container = x => new object();

			var replacer = new TestStage(onExecute: r => r.InstanceFactory = container);
			var next = new TestStage();

			var pipeline = new Pipeline();
			pipeline.Execute(new[] { replacer, next });

			next.InstanceFactory.ShouldBe(container);
		}
	}
}