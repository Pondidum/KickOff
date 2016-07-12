using System;
using System.Linq;
using System.ServiceProcess;
using StructureMap;
using StructureMap.Graph;

namespace ServiceContainer
{
	internal class ServiceWrapper : ServiceBase
	{
		private readonly Type _entryPoint;
		private readonly Pipeline _pipeline;

		public ServiceWrapper(IContainer container, string name, Type entryPoint)
		{
			_entryPoint = entryPoint;
			ServiceName = name;
			
			_pipeline = new Pipeline(container);
		}

		public void Start(string[] args)
		{
			OnStart(args);
		}

		protected override void OnStart(string[] args)
		{
			_pipeline.Execute(_entryPoint, args);
		}

		protected override void OnStop()
		{
			_pipeline.Dispose();
		}
	}
}
