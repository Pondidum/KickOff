using System.Linq;
using KickOff.Tests.TestInfrastructure;
using Shouldly;
using Xunit;

namespace KickOff.Tests
{
	public class EmptyStageBuilderTests
	{
		private readonly StageBuilder _builder;

		public EmptyStageBuilderTests()
		{
			_builder = new StageBuilder();
		}

		[Fact]
		public void When_no_stages_are_registered() => _builder.ToStages().ShouldBeEmpty();

		[Fact]
		public void When_adding()
		{
			var stage = new TestStage();

			_builder.Add(stage);
			_builder.ToStages().ShouldBe(new[] { stage });
		}

		[Fact]
		public void When_starting_with()
		{
			var stage = new TestStage();
			_builder.StartWith(stage);

			_builder.ToStages().ShouldBe(new[] { stage });
		}

		[Fact]
		public void When_ending_with()
		{
			var stage = new TestStage();
			_builder.EndWith(stage);

			_builder.ToStages().ShouldBe(new[] { stage });
		}

		[Fact]
		public void When_adding_before()
		{
			var stage = new TestStage();

			Should.Throw<StageNotFoundException>(() => _builder.AddBefore<TestStage>(stage));
		}

		[Fact]
		public void When_adding_after()
		{
			var stage = new TestStage();

			Should.Throw<StageNotFoundException>(() => _builder.AddAfter<TestStage>(stage));
		}
	}

	public class PopulatedStageBuilderTests
	{
		private readonly StageBuilder _builder;
		private readonly IStage _stageOne;
		private readonly IStage _stageTwo;

		public PopulatedStageBuilderTests()
		{
			_stageOne = new StageOne();
			_stageTwo = new StageTwo();
			_builder = new StageBuilder(new[] { _stageOne, _stageTwo, });
		}


		[Fact]
		public void When_stages_are_registered() => _builder.ToStages().ShouldBe(new[] { _stageOne, _stageTwo });

		[Fact]
		public void When_adding()
		{
			var stage = new TestStage();

			_builder.Add(stage);
			_builder.ToStages().ShouldBe(new[] { _stageOne, _stageTwo, stage });
		}

		[Fact]
		public void When_starting_with()
		{
			var stage = new TestStage();
			_builder.StartWith(stage);

			_builder.ToStages().ShouldBe(new[] { stage, _stageOne, _stageTwo });
		}

		[Fact]
		public void When_ending_with()
		{
			var stage = new TestStage();
			_builder.EndWith(stage);

			_builder.ToStages().ShouldBe(new[] { _stageOne, _stageTwo, stage });
		}

		[Fact]
		public void When_adding_before_first()
		{
			var stage = new TestStage();

			_builder.AddBefore<StageOne>(stage);

			_builder.ToStages().ShouldBe(new[] { stage, _stageOne, _stageTwo });
		}

		[Fact]
		public void When_adding_after_first()
		{
			var stage = new TestStage();

			_builder.AddAfter<StageOne>(stage);

			_builder.ToStages().ShouldBe(new[] { _stageOne, stage, _stageTwo });
		}

		[Fact]
		public void When_adding_before_last()
		{
			var stage = new TestStage();

			_builder.AddBefore<StageTwo>(stage);

			_builder.ToStages().ShouldBe(new[] { _stageOne, stage, _stageTwo });
		}

		[Fact]
		public void When_adding_after_last()
		{
			var stage = new TestStage();

			_builder.AddAfter<StageTwo>(stage);

			_builder.ToStages().ShouldBe(new[] { _stageOne, _stageTwo, stage });
		}

		private class StageOne : IStage
		{
			public void OnStart(StageArgs args)
			{
				throw new System.NotImplementedException();
			}

			public void OnStop(StageArgs args)
			{
				throw new System.NotImplementedException();
			}
		}

		private class StageTwo : IStage
		{
			public void OnStart(StageArgs args)
			{
				throw new System.NotImplementedException();
			}

			public void OnStop(StageArgs args)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
