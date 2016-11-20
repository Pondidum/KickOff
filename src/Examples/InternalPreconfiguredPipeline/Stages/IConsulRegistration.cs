using Consul;

namespace InternalPreconfiguredPipeline.Stages
{
	public interface IConsulRegistration
	{
		CatalogRegistration CreateRegistration();
		CatalogDeregistration CreateDeregistration();
	}
}
