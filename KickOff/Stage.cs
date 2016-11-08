using System;

namespace KickOff
{
	public abstract class Stage
	{
		public abstract void OnStart(StageArgs args);
		public abstract void OnStop(StageArgs args);
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
