using System;

namespace KickOff
{
	public interface IStage
	{
		void OnStart(StageArgs args);
		void OnStop(StageArgs args);
	}

	public class StageArgs
	{
		public string[] StartArgs { get; }
		public Func<Type, object> InstanceFactory { get; set; }

		public StageArgs(string[] startArgs)
		{
			StartArgs = startArgs;
		}

		public T TryGetInstance<T>()
		{
			return (T)InstanceFactory(typeof(T));
		}
	}
}
