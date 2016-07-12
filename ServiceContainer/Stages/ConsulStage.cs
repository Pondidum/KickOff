using Consul;

namespace ServiceContainer.Stages
{
	public class ConsulStage : Stage
	{
		public override void Execute()
		{
			var registration = Container.TryGetInstance<IConsulRegistration>();

			if (registration != null)
			{
				var client = new ConsulClient();
				client.Catalog.Register(registration.CreateRegistration());
			}
		}

		public override void Dispose()
		{
			var registration = Container.TryGetInstance<IConsulRegistration>();

			if (registration != null)
			{
				var client = new ConsulClient();
				client.Catalog.Deregister(registration.CreateDeregistration());
			}
		}
	}
}
