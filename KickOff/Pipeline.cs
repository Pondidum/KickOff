using System;
using System.Collections.Generic;
using System.Linq;

namespace KickOff
{
	public class Pipeline : IDisposable
	{
		private readonly IStage[] _stages;
		private StageArgs _stageArgs;

		public Pipeline(IEnumerable<IStage> stages)
		{
			_stages = stages.ToArray();
		}

		public void Execute(string[] startArgs)
		{
			_stageArgs = new StageArgs(startArgs)
			{
				InstanceFactory = type => type.GetConstructor(Type.EmptyTypes)?.Invoke(null)
			};

			foreach (var stage in _stages)
			{
				stage.OnStart(_stageArgs);
			}
		}

		public void Dispose()
		{
			for (var i = _stages.Length - 1; i >= 0; i--)
				_stages[i].OnStop(_stageArgs);
		}
	}
}
