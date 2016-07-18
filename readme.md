# Service Container

A Microservice Infrstructue project - contains nearly all boilerplate for a microservice in c#.

## Configuration

You can configure some options of the ServiceContainer by implementing a couple of interfaces:

### ElasticSearch/Kibana Logging

```CSharp
public class Config : ILogConfig
{
    public bool EnableKibana { get { return Debugger.IsAttached == false; } }
    public Uri LoggingEndpoint { get { return new Uri("http://localhost:9200"); } }
}
```

### Consul Service Registration/Unregistration

```CSharp
public class Config : IConsulRegistration
{
    public CatalogRegistration CreateRegistration()
    {
        return new CatalogRegistration() { Service = new AgentService
        {
            Address = "http://localhost",
            Port = 8005,
            Service = "TestService"
        }};
    }

    public CatalogDeregistration CreateDeregistration()
    {
        return new CatalogDeregistration { ServiceID = "TestService" };
    }
}
```
You can of course combine both interface implementations into one class if you wish.

### StructureMap Configuration

The inbuilt StructureMap configuration is set to look for [Registries][structuremap-registry], so all you need to do is implement a Registry:

```CSharp
public class MyServiceRegistry: Registry
{
    public MyServiceRegistry()
    {
        For<ISomeDependency>().Use<SomeConcreteImplementation>();
    }
}
```

As the startup class is created by StructureMap, it supports constructor injection, so you can pass your configuration etc straight in:

```CSharp
public class Startup : IStartup
{
    private readonly Configuration _config;
    private readonly ISomeDependency _dependency;

    public Startup(Configuration config, ISomeDependency dependency)
    {
        _config = config;
        _dependency = dependency;
    }

    public void Execute(ServiceArgs service)
    {
        //...
    }
}
```

## Example

```CSharp
public class Program
{
    static void Main(string[] args)
    {
        ServiceHost.Run<Startup>("TestService");
     }
}

public class Startup : IStartup, IDisposable
{
    private static readonly ILogger Log = Serilog.Log.ForContext<Startup>();

    public Startup()
    {
        Log.Debug("Service Starting...")
    }

    public void Execute(ServiceArgs service)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "activity.txt");
        var i = 0;

        while (service.CancelRequested == false)
        {
            File.WriteAllText(path, $"Iteration {i}.");
            Thread.Sleep(500);
        }
    }

    public void Dispose()
    {
        Log.Debug("Service Stopping...")
    }
}
```

# Creating Your Own

1. Fork the project
2. Implement some custom `Stage` implementations
3. Setup with implementations are used in `ServiceWrapper.cs`
4. Pull Request?

I have a [blog post about this project][blog-servicecontainer] which has an outline of why I am not supporting Stage configuration here.


[structuremap-registry]: http://structuremap.github.io/registration/registry-dsl/
[blog-servicecontainer]: http://andydote.co.uk/2016/07/17/preventing-microservice-boilerplate/
