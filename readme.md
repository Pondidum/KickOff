# KickOff
A Microservice startup and shutdown pipeline

KickOff provides the basis to make a standard Microservice startup across all your systems.


## Usage - Windows Hosting
*For console applications which can also be installed as Windows Services*

* Install KickOff
```
PM> Install-Package KickOff.Host.Windows
```
* Create a new `ConsoleApplication`
* Add a class to be your application's entry point - implement `IStartup`
```csharp
public class LogWriter : IStartup, IDisposable
{
	public LogWriter()
	{
		Console.WriteLine("starting up");
	}

	public void Execute(ServiceArgs service)
	{
		while (service.CancelRequested == false)
		{
			Console.Write(".");
			Thread.Sleep(1000);
		}
	}

	public void Dispose()
	{
		Console.WriteLine("shutting down");
	}
}
```
* Replace the application's `Program`'s `Main()` function with:
```csharp
ServiceHost.Run("TestService", new IStage[]
{
    new ServiceMetadataStage(), //optional, but useful
    // add other stages here, e.g.
    // structuremap, simpleinjector, consul, serilog, etc.
	new AsyncRunnerStage()
});
```


## Usage - Pipeline Only - Hosting
*For wrapping the pipeline in other hosting, such as OWIN, TopShelf etc.*

In this example, we will host the service using TopShelf

* Install KickOff
```ps
PM> Install-Package KickOff
```
* Create a new `ConsoleApplication`
* Add a class to be your application's entry point - implement `IStartup`
```csharp
public class LogWriter : IStartup, IDisposable
{
	public LogWriter()
	{
		Console.WriteLine("starting up");
	}

	public void Execute(ServiceArgs service)
	{
		while (service.CancelRequested == false)
		{
			Console.Write(".");
			Thread.Sleep(1000);
		}
	}

	public void Dispose()
	{
		Console.WriteLine("shutting down");
	}
}
```
* Replace the application's `Program`'s `Main()` function with:
```csharp
HostFactory.Run(x =>
{
	x.Service<Pipeline>(s =>
	{
		s.ConstructUsing(name => new Pipeline(new IStage[]
		{
            new ServiceMetadataStage(), //optional, but useful
            // add other stages here, e.g.
            // structuremap, simpleinjector, consul, serilog, etc.
			new AsyncRunnerStage(),
		}));
		s.WhenStarted(pipeline => pipeline.OnStart(args));
		s.WhenStopped(pipeline => pipeline.OnStop());
	});
});
```

## Usage - Pipeline Only - Packaging
*For creating your own standardised bootstrapper*

* Install KickOff
```ps
PM> Install-Package KickOff
```
* For example, you could create a standardised Microservice startup nuget:
```csharp
public class Microservice
{
    public static void Run<TConfig>(string name, TConfig config)
        where TConfig : ILoggerConfig, IRabbitMqConfig, IConsulConfig
    {
        ServiceHost.Run(name, new IStage[]
        {
            new ServiceMetadataStage(), //optional, but useful
            new StructureMapStage(),
            new ConsulStage(config),
            new SerilogStage(config),
            new RabbitMqStage(config),
            new AsyncRunnerStage()
        });
    }
}

public interface ILoggerConfig
{
    string LogPath { get; }
}

public interface IRabbitMqConfig
{
    string Host { get; }
    string User { get; }
    string Password { get; }
    IEnumerable<string> Queues { get; }
}
```
* Create a Nuget of this, and use it in all your Microservices!
* Looking for a way to load your Strong Typed configuration? [Checkout Stronk](github.com/pondidum/stronk/)!
