using System.Collections.Generic;
using System.ServiceProcess;

namespace KickOff
{
	public class ServiceWrapper : ServiceBase
	{
		private readonly Pipeline _pipeline;

		public ServiceWrapper(string name, IEnumerable<IStage> stages, PipelineCustomisation customiser)
		{
			ServiceName = name;

			_pipeline = new Pipeline(stages, customiser);
		}

		public void Start(string[] args)
		{
			OnStart(args);
		}

		protected override void OnStart(string[] args)
		{
			_pipeline.OnStart(args);
		}

		protected override void OnStop()
		{
			_pipeline.OnStop();
		}
	}
}
