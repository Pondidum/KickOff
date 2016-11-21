using System.Collections.Generic;
using System.Linq;

namespace KickOff
{
	public class StageBuilder
	{
		private readonly List<IStage> _stages;

		public StageBuilder() : this(Enumerable.Empty<IStage>())
		{
		}

		public StageBuilder(IEnumerable<IStage> stages)
		{
			_stages = stages.ToList();
		}

		public StageBuilder Add(IStage stage)
		{
			_stages.Add(stage);
			return this;
		}

		public StageBuilder StartWith(IStage stage)
		{
			_stages.Insert(0, stage);
			return this;
		}

		public StageBuilder EndWith(IStage stage)
		{
			_stages.Add(stage);
			return this;
		}

		public StageBuilder AddBefore<TStage>(IStage stage) where TStage : IStage
		{
			var index = _stages.FindIndex(s => s.GetType() == typeof(TStage));

			if (index < 0)
				throw new StageNotFoundException(typeof(TStage));

			_stages.Insert(index, stage);

			return this;
		}

		public StageBuilder AddAfter<TStage>(IStage stage) where TStage : IStage
		{
			var index = _stages.FindIndex(s => s.GetType() == typeof(TStage));

			if (index < 0)
				throw new StageNotFoundException(typeof(TStage));

			_stages.Insert(index + 1, stage);

			return this;
		}

		public IEnumerable<IStage> ToStages()
		{
			return _stages;
		}
	}
}
