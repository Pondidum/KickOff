using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContainer;

namespace TestService
{
	class Program
	{
		static void Main(string[] args)
		{

			ServiceHost.Run("TestService", () =>
			{
				File.AppendAllLines(@"D:\dev\test-projects\ServiceContainer\\log.txt", new [] { "boot!" });
			});
		}
	}
}
