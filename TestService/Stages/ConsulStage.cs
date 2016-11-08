using Consul;
using KickOff;

namespace TestService.Stages
{
	public class ConsulStage : Stage
	{
		public override void Execute(StageArgs args)
		{
			var registration = args.TryGetInstance<IConsulRegistration>();

			if (registration != null)
			{
				var client = new ConsulClient();
				client.Catalog.Register(registration.CreateRegistration());
			}
		}

		public override void Dispose(StageArgs args)
		{
			var registration = args.TryGetInstance<IConsulRegistration>();

			if (registration != null)
			{
				var client = new ConsulClient();
				client.Catalog.Deregister(registration.CreateDeregistration());
			}
		}
	}
}
