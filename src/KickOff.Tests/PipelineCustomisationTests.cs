using Shouldly;
using Xunit;

namespace KickOff.Tests
{
	public class PipelineCustomisationTests
	{
		private readonly PipelineCustomisation _pipe;
		private readonly Dto _target;

		public PipelineCustomisationTests()
		{
			_pipe = new PipelineCustomisation();
			_target = new Dto();
		}

		[Fact]
		public void When_applying_a_non_existing_customisation()
		{
			Should.NotThrow(() => _pipe.Apply(_target));
		}

		[Fact]
		public void When_applying_an_added_customisation()
		{
			_pipe.Add<Dto>(dto => dto.Value = 123);

			_pipe.Apply(_target);

			_target.Value.ShouldBe(123);
		}

		[Fact]
		public void When_applying_a_replaced_customisation()
		{
			_pipe.Add<Dto>(dto => dto.Value = 123);
			_pipe.Add<Dto>(dto => dto.Value = 987);

			_pipe.Apply(_target);

			_target.Value.ShouldBe(987);
		}

		[Fact]
		public void When_applying_an_only_appeneded_customisation()
		{
			_pipe.Append<Dto>(dto => dto.Value = 123);

			_pipe.Apply(_target);

			_target.Value.ShouldBe(123);
		}

		[Fact]
		public void When_applying_an_added_and_appended_customisation()
		{
			_pipe.Add<Dto>(dto => dto.Value = 123);
			_pipe.Append<Dto>(dto => dto.Value = dto.Value + 15);

			_pipe.Apply(_target);

			_target.Value.ShouldBe(138);
		}

		[Fact]
		public void When_checking_for_a_non_existant_registration_by_generic()
		{
			_pipe
				.HasCustomisationFor<Dto>()
				.ShouldBe(false);
		}

		[Fact]
		public void When_checking_for_a_non_existant_registration_by_type()
		{
			_pipe
				.HasCustomisationFor(typeof(Dto))
				.ShouldBe(false);
		}

		[Fact]
		public void When_checking_for_an_existing_registration_by_generic()
		{
			_pipe.Add<Dto>(dto => dto.Value = 123);

			_pipe
				.HasCustomisationFor<Dto>()
				.ShouldBe(true);
		}

		[Fact]
		public void When_checking_for_an_existing_registration_by_type()
		{
			_pipe.Add<Dto>(dto => dto.Value = 123);

			_pipe
				.HasCustomisationFor(typeof(Dto))
				.ShouldBe(true);
		}

		private class Dto
		{
			public int Value { get; set; }
		}
	}
}
