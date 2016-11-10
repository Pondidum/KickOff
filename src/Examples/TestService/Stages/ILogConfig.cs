using System;

namespace TestService.Stages
{
	public interface ILogConfig
	{
		bool EnableKibana { get; }
		Uri LoggingEndpoint { get; }
	}
}
