using System;
using System.Collections.Generic;

namespace KickOff
{
	public class Pipeline : IDisposable
	{
		private readonly List<Stage> _stages;

		public Pipeline()
		{
			_stages = new List<Stage>();
		}

		public void Execute(IEnumerable<Stage> stages)
		{
			Func<Type, object> factory = type => type.GetConstructor(Type.EmptyTypes).Invoke(null);

			foreach (var stage in stages)
			{
				stage.InstanceFactory = factory;
				stage.Execute();

				factory = stage.InstanceFactory;
				_stages.Add(stage);
			}
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
