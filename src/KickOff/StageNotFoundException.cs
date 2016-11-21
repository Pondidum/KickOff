using System;

namespace KickOff
{
	public class StageNotFoundException : Exception
	{
		public StageNotFoundException(Type stage)
			: base($"Unable to find a stage of type {stage.Name}")
		{
		}
	}
}
