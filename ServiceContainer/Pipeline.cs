using System;
using System.Collections.Generic;
using ServiceContainer.Stages;
using StructureMap;

namespace ServiceContainer
{
	public class Pipeline : IDisposable
	{
		private readonly List<Stage> _stages;
		private readonly IContainer _container;

		public Pipeline(IContainer container)
		{
			_stages = new List<Stage>();
			_container = container;
		}

		public void Execute(IEnumerable<Stage> stages)
		{
			foreach (var stage in stages)
			{
				stage.Container = _container;
				stage.Execute();
			}

			_stages.AddRange(stages);
		}

		public void Dispose()
		{
			_stages.Reverse();
			foreach (var stage in _stages)
			{
				stage.Dispose();
			}
		}
	}
}