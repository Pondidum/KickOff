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
*For wrapping the pipeline in other hosting, such as OWIN, TopShelf etc.*

* Install KickOff
```ps
PM> Install-Package KickOff
```
