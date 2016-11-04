using StructureMap;
using StructureMap.Graph;

namespace ServiceContainer.Stages
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
	}
}
