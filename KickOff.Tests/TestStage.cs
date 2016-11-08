using System;

namespace KickOff.Tests
{
	public class TestStage : IStage
	{
		private readonly Action<IStage, StageArgs> _onExecute;
		private readonly Action<IStage, StageArgs> _onDispose;

		public TestStage()
		{
		}

		public TestStage(Action<IStage, StageArgs> onExecute = null, Action<IStage, StageArgs> onDispose = null)
		{
			_onExecute = onExecute;
			_onDispose = onDispose;
		}

		public TestStage(Action<IStage> onExecute = null, Action<IStage> onDispose = null)
		{
			_onExecute = (me, args) => onExecute?.Invoke(me);
			_onDispose = (me, args) => onDispose?.Invoke(me);
		}

		public virtual void OnStart(StageArgs args) => _onExecute?.Invoke(this, args);

		public virtual void OnStop(StageArgs args) => _onDispose?.Invoke(this, args);
	}
}
