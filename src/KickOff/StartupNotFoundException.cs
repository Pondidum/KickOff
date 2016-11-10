using System;

namespace KickOff
{
	public class StartupNotFoundException : Exception
	{
		public StartupNotFoundException() : base("Could not find an implementation of IStartup to run")
		{
		}
	}
}
