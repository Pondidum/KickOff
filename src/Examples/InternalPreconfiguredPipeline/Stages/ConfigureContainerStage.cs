using KickOff;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace InternalPreconfiguredPipeline.Stages
{
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

		private class AllInterfacesConvention : IRegistrationConvention
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
}
