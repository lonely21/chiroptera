using System;
using System.IO;
using Mono.Unix;

namespace BatMud.BatClientText
{
	static class Program
	{
		public static string ConfigPath;
		
		static void Main()
		{
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			}

			ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			ConfigPath = Path.Combine(ConfigPath, "batclient");
			if(!File.Exists(ConfigPath))
				Directory.CreateDirectory(ConfigPath);			
			
			ClientCore clientCore = new ClientCore();

			clientCore.Run();
		}
		
		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Dbg.WriteLine(e.ExceptionObject.ToString());
			Terminal.UnInit();
			Console.WriteLine(e.ExceptionObject);
		}

	}
}
