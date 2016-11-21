using System;
using System.Reflection;
using KickOff.Stages;
using Shouldly;
using Xunit;

namespace KickOff.Tests.Stages
{
	public class ServiceMetadataStageTests
	{
		private readonly StageArgs _stageArgs;

		public ServiceMetadataStageTests()
		{
			_stageArgs = new StageArgs(new string[0]);
		}

		private ServiceMetadata Metadata => _stageArgs.Metadata;

		[Fact]
		public void When_there_are_no_attributes()
		{
			var stage = new ServiceMetadataStage(new Attribute[] { });

			stage.OnStart(_stageArgs);

			Metadata.ShouldSatisfyAllConditions(
				() => Metadata.Name.ShouldBe(string.Empty),
				() => Metadata.Description.ShouldBe(string.Empty)
			);
		}

		[Fact]
		public void When_there_is_no_name_attribute()
		{
			var stage = new ServiceMetadataStage(new Attribute[]
			{
				new AssemblyDescriptionAttribute("desc"),
			});

			stage.OnStart(_stageArgs);

			Metadata.Description.ShouldBe("desc");
		}

		[Fact]
		public void When_there_is_no_description_attribute()
		{
			var stage = new ServiceMetadataStage(new Attribute[]
			{
				new AssemblyTitleAttribute("title"),
			});

			stage.OnStart(_stageArgs);

			Metadata.Name.ShouldBe("title");
		}

		[Fact]
		public void When_there_are_both_attributes()
		{
			var stage = new ServiceMetadataStage(new Attribute[]
			{
				new AssemblyTitleAttribute("title"),
				new AssemblyDescriptionAttribute("desc"),
			});

			stage.OnStart(_stageArgs);

			Metadata.ShouldSatisfyAllConditions(
				() => Metadata.Name.ShouldBe("title"),
				() => Metadata.Description.ShouldBe("desc")
			);
		}

		[Fact]
		public void When_run_on_an_assembly()
		{
			var stage = new ServiceMetadataStage(typeof(ServiceMetadataStage).Assembly);

			stage.OnStart(_stageArgs);

			Metadata.ShouldSatisfyAllConditions(
				() => Metadata.Name.ShouldBe("KickOff"),
				() => Metadata.Description.ShouldBe("Microservice bootstrapping pipeline")
			);
		}
	}
}
