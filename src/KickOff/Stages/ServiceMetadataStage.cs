using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KickOff.Stages
{
	public class ServiceMetadataStage : IStage
	{
		private readonly Attribute[] _attributes;

		public ServiceMetadataStage(Assembly sourceAssembly)
			: this(sourceAssembly.GetCustomAttributes())
		{
		}

		public ServiceMetadataStage(IEnumerable<Attribute> attributes)
		{
			_attributes = attributes.ToArray();
		}

		public void OnStart(StageArgs args)
		{
			var name = _attributes.OfType<AssemblyTitleAttribute>().SingleOrDefault()?.Title;
			var description = _attributes.OfType<AssemblyDescriptionAttribute>().SingleOrDefault()?.Description;

			args.Metadata.Name = name ?? string.Empty;
			args.Metadata.Description = description ?? string.Empty;
		}

		public void OnStop(StageArgs args)
		{
		}
	}
}

