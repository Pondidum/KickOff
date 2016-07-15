using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using ServiceContainer.Stages;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace ServiceContainer
{
	internal class ServiceWrapper : ServiceBase
	{
		private readonly Type _entryPoint;
		private readonly Pipeline _pipeline;
		private readonly IEnumerable<Stage> _stages;

		public ServiceWrapper(string name, Type entryPoint)
		{
			ServiceName = name;

			var container = CreateContainer(entryPoint);

			_entryPoint = entryPoint;
			_pipeline = new Pipeline(container);

			_stages = new Stage[]
			{
				new LoggingStage(name),
				new ConsulStage()
			};
		}

		public void Start(string[] args)
		{
			OnStart(args);
		}

		protected override void OnStart(string[] args)
		{
			_pipeline.Execute(_stages.Concat(new[] { new RunnerStage(_entryPoint, args) }));
		}

		protected override void OnStop()
		{
			_pipeline.Dispose();
		}

		private static IContainer CreateContainer(Type entryPoint)
		{
			return new Container(c =>
			{
				c.Scan(a =>
				{
					a.TheCallingAssembly();
					a.AssemblyContainingType(entryPoint);

					a.LookForRegistries();

					a.Convention<AllInterfacesConvention>();
					a.WithDefaultConventions();
				});

			});
		}
	}

	internal class AllInterfacesConvention : IRegistrationConvention
	{
		public void ScanTypes(TypeSet types, Registry registry)
		{
			// Only work on concrete types
			var classes = types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed);

			foreach (var type in classes)
				foreach (var @interface in type.GetInterfaces())
					registry.For(@interface).Use(type);
		}
	}
}
