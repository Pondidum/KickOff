using System;
using System.Linq;

namespace KickOff
{
	public class StageArgs
	{
		private readonly PipelineCustomisation _customiser;

		public string[] StartArgs { get; }
		public Func<Type, object> InstanceFactory { get; set; }

		public StageArgs(PipelineCustomisation customiser, string[] startArgs)
		{
			_customiser = customiser;
			StartArgs = startArgs;
			InstanceFactory = DefaultInstanceFactory;
		}

		public T TryGetInstance<T>()
		{
			return (T)InstanceFactory(typeof(T));
		}

		public void ApplyCustomisationTo<TTarget>(TTarget customisationTarget)
		{
			_customiser.Apply(customisationTarget);
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
