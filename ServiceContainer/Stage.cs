using System;
using StructureMap;

namespace ServiceContainer
{
	public abstract class Stage : IDisposable
	{
		public IContainer Container { get; set; }

		public abstract void Execute();
		public abstract void Dispose();
	}
}