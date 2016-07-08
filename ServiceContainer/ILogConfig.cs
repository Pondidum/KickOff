namespace ServiceContainer
{
	public interface ILogConfig
	{
		string LoggingEndpoint { get; set; }
		bool EnableKibana { get; set; }
	}
}
