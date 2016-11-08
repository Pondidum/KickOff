using System;
using System.Collections.Generic;

namespace KickOff
{
	public class Pipeline : IDisposable
	{
		private readonly List<IStage> _stages;
		private StageArgs _stageArgs;

		public Pipeline()
		{
			_stages = new List<IStage>();
		}

		public void Execute(IEnumerable<IStage> stages, string[] startArgs)
		{
			_stageArgs = new StageArgs(startArgs)
			{
				InstanceFactory = type => type.GetConstructor(Type.EmptyTypes)?.Invoke(null)
		};

			foreach (var stage in stages)
			{
				stage.OnStart(_stageArgs);

				_stages.Add(stage);
			}
		}

		public void Dispose()
		{
			for (var i = _stages.Count - 1; i >= 0; i--)
				_stages[i].OnStop(_stageArgs);
		}
	}
}
