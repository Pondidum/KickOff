using System;

namespace KickOff.Tests
{
	public class TestStage : Stage
	{
		private readonly Action<Stage, StageArgs> _onExecute;
		private readonly Action<Stage, StageArgs> _onDispose;

		public TestStage()
		{
		}

		public TestStage(Action<Stage, StageArgs> onExecute = null, Action<Stage, StageArgs> onDispose = null)
		{
			_onExecute = onExecute;
			_onDispose = onDispose;
		}

		public TestStage(Action<Stage> onExecute = null, Action<Stage> onDispose = null)
		{
			_onExecute = (me, args) => onExecute?.Invoke(me);
			_onDispose = (me, args) => onDispose?.Invoke(me);
		}

		public override void Execute(StageArgs args) => _onExecute?.Invoke(this, args);

		public override void Dispose(StageArgs args) => _onDispose?.Invoke(this, args);
	}
}
