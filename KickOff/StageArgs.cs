using System;

namespace KickOff
{
	public class StageArgs
	{
		public string[] StartArgs { get; }
		public Func<Type, object> InstanceFactory { get; set; }

		public StageArgs(string[] startArgs)
		{
			StartArgs = startArgs;
			InstanceFactory = type => type.GetConstructor(Type.EmptyTypes)?.Invoke(null);
		}

		public T TryGetInstance<T>()
		{
			return (T)InstanceFactory(typeof(T));
		}
	}
}
