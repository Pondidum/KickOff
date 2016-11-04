using System;

namespace ServiceContainer
{
	public abstract class Stage : IDisposable
	{
		public Func<Type, object> InstanceFactory { get; set; }

		public abstract void Execute();
		public abstract void Dispose();

		protected T TryGetInstance<T>()
		{
			return (T)InstanceFactory(typeof(T));
		}
	}
}
