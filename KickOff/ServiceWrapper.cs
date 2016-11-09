using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using KickOff.Stages;

namespace KickOff
{
	public class ServiceWrapper : ServiceBase
	{
		private readonly Pipeline _pipeline;
		private readonly IEnumerable<IStage> _stages;

		public ServiceWrapper(string name, IEnumerable<IStage> stages)
		{
			ServiceName = name;

			_stages = stages;
			_pipeline = new Pipeline();
		}

		public void Start(string[] args)
		{
			OnStart(args);
		}

		protected override void OnStart(string[] args)
		{
			_pipeline.Execute(_stages, args);
		}

		protected override void OnStop()
		{
			_pipeline.Dispose();
		}
	}
}
