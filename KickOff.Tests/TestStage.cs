using System;

namespace KickOff.Tests
{
	public class TestStage : Stage
	{
		private readonly Action<Stage> _onExecute;
		private readonly Action<Stage> _onDispose;

		public TestStage(Action<Stage> onExecute = null, Action<Stage> onDispose = null)
		{
			_onExecute = onExecute;
			_onDispose = onDispose;
		}

		public override void Execute() => _onExecute?.Invoke(this);

		public override void Dispose() => _onDispose?.Invoke(this);
	}
}
