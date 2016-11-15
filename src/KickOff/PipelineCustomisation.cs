using System;
using System.Collections.Generic;

namespace KickOff
{
	public class PipelineCustomisation
	{
		private readonly Dictionary<Type, Action<object>> _customisations;

		public PipelineCustomisation()
		{
			_customisations = new Dictionary<Type, Action<object>>();
		}

		public void Add<T>(Action<T> customisation)
		{
			_customisations[typeof(T)] = value => customisation((T)value);
		}

		public void Apply<TTarget>(TTarget customisationTarget)
		{
			Action<object> apply;

			if (_customisations.TryGetValue(typeof(TTarget), out apply))
				apply(customisationTarget);
		}
	}
}
