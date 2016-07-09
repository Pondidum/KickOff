using System;

namespace ServiceContainer
{
	public interface ILogConfig
	{
		bool EnableKibana { get; }
		Uri LoggingEndpoint { get; }
	}
}
