using KickOff;
using ServiceContainer;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace TestService.Stages
{
	public class ConfigureContainerStage : Stage
	{
		private Container _container;

		public override void Execute()
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

			InstanceFactory = _container.TryGetInstance;
		}

		public override void Dispose()
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
