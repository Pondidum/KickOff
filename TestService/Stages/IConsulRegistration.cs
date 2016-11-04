using Consul;

namespace TestService.Stages
{
	public interface IConsulRegistration
	{
		CatalogRegistration CreateRegistration();
		CatalogDeregistration CreateDeregistration();
	}
}
