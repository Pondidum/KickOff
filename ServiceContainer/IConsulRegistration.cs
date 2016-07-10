using Consul;

namespace ServiceContainer
{
	public interface IConsulRegistration
	{
		CatalogRegistration CreateRegistration();
		CatalogDeregistration CreateDeregistration();
	}
}
