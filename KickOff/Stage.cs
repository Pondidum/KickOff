using System;

namespace KickOff
{
	public abstract class Stage : IDisposable
	{
		public Func<Type, object> InstanceFactory { get; set; }

		public abstract void Execute(StageArgs args);
		public abstract void Dispose();

		protected T TryGetInstance<T>()
		{
			return (T)InstanceFactory(typeof(T));
		}
	}

	public class StageArgs
	{
		public string[] StartArgs { get; }

		public StageArgs(string[] startArgs)
		{
			StartArgs = startArgs;
		}
	}
}
