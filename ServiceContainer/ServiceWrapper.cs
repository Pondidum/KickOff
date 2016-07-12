using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using ServiceContainer.Stages;
using StructureMap;

namespace ServiceContainer
{
	internal class ServiceWrapper : ServiceBase
	{
		private readonly Type _entryPoint;
		private readonly Pipeline _pipeline;
		private readonly IEnumerable<Stage> _stages;

		public ServiceWrapper(IContainer container, string name, Type entryPoint)
		{
			ServiceName = name;

			_entryPoint = entryPoint;
			_pipeline = new Pipeline(container);

			_stages = new Stage[]
			{
				new LoggingStage(name),
				new ConsulStage()
			};
		}

		public void Start(string[] args)
		{
			OnStart(args);
		}

		protected override void OnStart(string[] args)
		{
			_pipeline.Execute(_stages.Concat(new[] { new RunnerStage(_entryPoint, args) }));
		}

		protected override void OnStop()
		{
			_pipeline.Dispose();
		}
	}
}
