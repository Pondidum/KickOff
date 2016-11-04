using System;
using StructureMap;

namespace ServiceContainer
{
	public abstract class Stage : IDisposable
	{
		public IContainer Container { private get; set; }

		public abstract void Execute();
		public abstract void Dispose();

		protected T TryGetInstance<T>()
		{
			return Container.TryGetInstance<T>();
		}

		protected object TryGetInstance(Type type)
		{
			return Container.TryGetInstance(type);
		}
	}
}