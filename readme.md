# KickOff
A Microservice startup and shutdown pipeline

KickOff provides the basis to make a standard Microservice startup across all your systems.

## Usage

There are 3 main ways of using KickOff: To host a Windows Service directly, to manage the startup via other hosting (such as TopShelf), or to wrap to create a standardised bootstrapper.

### Windows Hosting
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
    // StructureMap, SimpleInjector, consul, serilog, etc.
    new AsyncRunnerStage()
});
```


### Usage - Pipeline Only - Hosting
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
            // StructureMap, SimpleInjector, consul, serilog, etc.
            new AsyncRunnerStage(),
        }));
        s.WhenStarted(pipeline => pipeline.OnStart(args));
        s.WhenStopped(pipeline => pipeline.OnStop());
    });
});
```

### Usage - Pipeline Only - Packaging
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

## Authoring Stages

KickOff doesn't come with many built in stages, as their construction and implementation are very diverse.  For example, which IoC Container should a stage use? I have a preference for StructureMap, but not everyone agrees with this, and relying on a container abstraction which hides all the great features of a container doesn't seem like a great idea.

Stages tend to fall into one of three categories; Container Stages, Runner Stages and Other Stages.


### Normal Stages

Stages have a very simple api - an `OnStart` and an `OnStop` method, both of which get a `StageArgs` parameter.

```csharp
public class SerilogStage : IStage
{
    public void OnStart(StageArgs args)
    {
    }

    public void OnStop(StageArgs args)
    {
    }
}
```

The `StageArgs` provides the following abilities:
* `string[] StartArgs` - the arguments the application/pipeline was started with
* `ServiceMetadata Metadata` - The Name and Description of the service, populated by the `ServiceMetadataStage`, which reads the provided Assembly's attributes.
* `Func<Type, object> InstanceFactory` - primarily for overriding the dependency resolver used by the pipeline.  See Container Stages below.
* `T TryGetInstance<T>()` - calls the `InstanceFactory` to create a type.

### Container Stages
Container stages don't differ much from a normal stages, other than they overwrite the `StageArgs.InstanceFactory` property.
You should make sure that the container can resolve the type `IStartup`, as this is what the Runner Stages request to actually launch your service!

There is an example stage in the TestService project using StructureMap:

```csharp
public class ConfigureContainerStage : IStage
{
    private Container _container;

    public virtual void OnStart(StageArgs args)
    {
        _container = new Container(c =>
        {
            c.Scan(a =>
            {
                a.TheCallingAssembly();
                a.LookForRegistries();

                a.Convention<AllInterfacesConvention>();
                a.WithDefaultConventions();
            });
        });

        args.InstanceFactory = _container.TryGetInstance;
    }

    public virtual void OnStop(StageArgs args)
    {
        _container?.Dispose();
    }
}
```

Generally it is a good idea to use some kind of Registry Location feature of your container (SimpleInjector supports this through `SimpleInjector.Packaging` nuget), as service can then have their own implementations without needing anything injected into the pipeline to do customisation.

### Runner Stages

A Runner Stage only differs from a Normal Stage in that it is responsible for calling the `IStartup` implementation.
KickOff comes with two Runners, the `RunnerStage` and `AsyncRunnerStage`.

Generally speaking the `AsyncRunnerStage` is the one you should be using, as this will allow the `Start` method of the pipeline to complete (for example, the start method would be called by the OnStart handler of a Windows Service.)
