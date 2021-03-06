using System;
using System.Linq;

namespace KickOff
{
	public class StageArgs
	{
		public string[] StartArgs { get; }
		public ServiceMetadata Metadata { get; }
		public Func<Type, object> InstanceFactory { get; set; }

		public StageArgs(string[] startArgs)
		{
			StartArgs = startArgs;
			InstanceFactory = DefaultInstanceFactory;
			Metadata = new ServiceMetadata();
		}

		public T TryGetInstance<T>()
		{
			return (T)InstanceFactory(typeof(T));
		}

		private static object DefaultInstanceFactory(Type type)
		{
			if (type != typeof(IStartup))
				return type.GetConstructor(Type.EmptyTypes)?.Invoke(null);

			var assemblies = AppDomain
				.CurrentDomain
				.GetAssemblies();

			var types = assemblies
				.Where(a => a.IsDynamic == false)
				.SelectMany(a => a.GetExportedTypes());

			var implementers = types
				.Where(t => t.IsClass && t.IsAbstract == false && type.IsAssignableFrom(t));

			var withCtor = implementers
				.SingleOrDefault(t => t.GetConstructor(Type.EmptyTypes) != null);

			return withCtor
				.GetConstructor(Type.EmptyTypes)
				?.Invoke(null);
		}
	}
}
