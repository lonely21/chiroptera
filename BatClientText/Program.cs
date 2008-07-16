using System;
using System.Collections.Generic;
using System.Text;
using Mono.Unix;
using Mono.Unix.Native;

namespace BatMud.BatClientText
{
	class Program
	{
		static void Main()
		{
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			}
			
			ClientCore clientCore = new ClientCore();

			clientCore.Run();
		}
		
		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Terminal.UnInit();
			Console.WriteLine(e.ExceptionObject);
		}

	}
}
