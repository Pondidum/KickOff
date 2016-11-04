using System;

namespace ServiceContainer
{
	public abstract class Stage : IDisposable
	{
		private Func<Type, object> _factory;

		internal void SetInstanceFactory(Func<Type, object> factory)
		{
			_factory = factory;
		}

		public abstract void Execute();
		public abstract void Dispose();

		protected T TryGetInstance<T>()
		{
			return (T)_factory(typeof(T));
		}

		protected object TryGetInstance(Type type)
		{
			return _factory(type);
		}
	}
}