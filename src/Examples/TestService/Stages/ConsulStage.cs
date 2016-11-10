using Consul;
using KickOff;

namespace TestService.Stages
{
	public class ConsulStage : IStage
	{
		public virtual void OnStart(StageArgs args)
		{
			var registration = args.TryGetInstance<IConsulRegistration>();

			if (registration != null)
			{
				var client = new ConsulClient();
				client.Catalog.Register(registration.CreateRegistration());
			}
		}

		public virtual void OnStop(StageArgs args)
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
