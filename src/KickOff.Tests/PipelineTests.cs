﻿using System;
using System.Collections.Generic;
using KickOff.Tests.TestInfrastructure;
using Shouldly;
using Xunit;

namespace KickOff.Tests
{
	public class PipelineTests
	{
		[Fact]
		public void When_a_pipeline_is_run()
		{
			var executionOrder = new List<IStage>();
			var disposalOrder = new List<IStage>();

			var first = new TestStage(executionOrder.Add, disposalOrder.Add);
			var second = new TestStage(executionOrder.Add, disposalOrder.Add);

			var stages = new[] { first, second };

			var pipeline = new Pipeline(stages);
			pipeline.OnStart(new string[0]);

			pipeline.ShouldSatisfyAllConditions(
				() => executionOrder.ShouldBe(stages),
				() => disposalOrder.ShouldBeEmpty()
			);
		}

		[Fact]
		public void When_a_pipeline_is_disposed()
		{
			var executionOrder = new List<IStage>();
			var disposalOrder = new List<IStage>();

			var first = new TestStage(executionOrder.Add, disposalOrder.Add);
			var second = new TestStage(executionOrder.Add, disposalOrder.Add);

			var stages = new[] { first, second };

			var pipeline = new Pipeline(stages);
			pipeline.OnStart(new string[0]);
			pipeline.OnStop();

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

			var pipeline = new Pipeline(new[] { replacer, next });
			pipeline.OnStart(new string[0]);

			instanceFactorySeen.ShouldBe(container);
		}

		[Fact]
		public void Start_args_are_passed_to_all_stages()
		{
			var receivedArgs = new List<string[]>();
			var startArgs = new[] { "a", "b", "c" };

			var first = new TestStage(onExecute: (s, args) => receivedArgs.Add(args.StartArgs));
			var second = new TestStage(onExecute: (s, args) => receivedArgs.Add(args.StartArgs));

			var pipeline = new Pipeline(new[] { first, second });
			pipeline.OnStart(startArgs);

			receivedArgs.ShouldBe(new[] { startArgs, startArgs });
		}

		[Fact]
		public void The_args_used_in_stop_are_same_as_start()
		{
			StageArgs startArgs = null;
			StageArgs stopArgs = null;

			var stage = new TestStage((ts, a) => startArgs = a, (ts, a) => stopArgs = a);

			var pipeline = new Pipeline(new[] { stage });
			pipeline.OnStart(new string[0]);
			pipeline.OnStop();

			stopArgs.ShouldBe(startArgs);
		}
	}
}