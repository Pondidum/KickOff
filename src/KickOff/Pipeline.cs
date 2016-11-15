using System.Collections.Generic;
using System.Linq;

namespace KickOff
{
	public class Pipeline
	{
		private StageArgs _stageArgs;

		private readonly PipelineCustomisation _customiser;
		private readonly IStage[] _stages;

		public Pipeline(IEnumerable<IStage> stages, PipelineCustomisation customiser = null)
		{
			_customiser = customiser ?? new PipelineCustomisation();
			_stages = stages.ToArray();
		}

		public void OnStart(string[] startArgs)
		{
			_stageArgs = new StageArgs(_customiser, startArgs);

			foreach (var stage in _stages)
			{
				stage.OnStart(_stageArgs);
			}
		}

		public void OnStop()
		{
			for (var i = _stages.Length - 1; i >= 0; i--)
				_stages[i].OnStop(_stageArgs);
		}
	}
}
