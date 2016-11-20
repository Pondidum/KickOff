using System;

namespace InternalPreconfiguredPipeline.Stages
{
	public interface ILogConfig
	{
		bool EnableKibana { get; }
		Uri LoggingEndpoint { get; }
	}
}
