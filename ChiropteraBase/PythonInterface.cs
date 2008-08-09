using System;
using System.Collections.Generic;
using System.Text;
using IronPython.Hosting;
using System.IO;

namespace Chiroptera.Base
{
	static public class PythonInterface
	{
		public static void Initialize(BaseServicesDispatcher servicesDispatcher,
			TriggerManager triggerManager,
			CommandManager commandManager,
			INetwork network,
			IChiConsole console,
			PythonEngine pythonEngine,
			KeyManager keyManager,
			HiliteManager hiliteManager)
		{
			s_servicesDispatcher = servicesDispatcher;
			s_triggerManager = triggerManager;
			s_commandManager = commandManager;
			s_network = network;
			s_console = console;
			s_pythonEngine = pythonEngine;
			s_keyManager = keyManager;
			s_hiliteManager = hiliteManager;
		}

		static BaseServicesDispatcher s_servicesDispatcher;
		static TriggerManager s_triggerManager;
		static CommandManager s_commandManager;
		static INetwork s_network;
		static IChiConsole s_console;
		static PythonEngine s_pythonEngine;
		static KeyManager s_keyManager;
		static HiliteManager s_hiliteManager;

		public static BaseServicesDispatcher ServicesDispatcher
		{
			get { return s_servicesDispatcher; }
		}

		public static TriggerManager TriggerManager
		{
			get { return s_triggerManager; }
		}

		public static CommandManager CommandManager
		{
			get { return s_commandManager; }
		}

		public static INetwork Network
		{
			get { return s_network; }
		}

		public static IChiConsole Console
		{
			get { return s_console; }
		}

		public static PythonEngine PythonEngine
		{
			get { return s_pythonEngine; }
		}

		public static KeyManager KeyManager
		{
			get { return s_keyManager; }
		}

		public static HiliteManager HiliteManager
		{
			get { return s_hiliteManager; }
		}

		static string ExpandPath(string file)
		{
			string chiPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Chiroptera");
#if DEBUG
			string[] paths = new string[] { chiPath, "../../../scripts/std" };
#else
			string[] paths = new string[] { chiPath, "std" };
#endif

			foreach (string path in paths)
			{
				string fullPath = Path.Combine(path, file);
				if (File.Exists(fullPath))
					return fullPath;

				if (File.Exists(fullPath + ".bc"))
					return fullPath + ".bc";

				if (File.Exists(fullPath + ".py"))
					return fullPath + ".py";
			}

			return null;
		}

		public static void RunScript(string file)
		{
			string filePath = ExpandPath(file);

			if (filePath == null)
			{
				ChiConsole.WriteLine("Script not found: {0}", file);
				return;
			}

			if (Path.GetExtension(filePath) == ".py")
			{
				RunPythonScript(filePath);
			}
			else if (Path.GetExtension(filePath) == ".bc")
			{
				RunChiScript(filePath);
			}
			else
			{
				ChiConsole.WriteLine("Script not found: {0}", file);
			}
		}

		static void RunChiScript(string filePath)
		{
			string file = Path.GetFileName(filePath);

			string[] lines = File.ReadAllLines(filePath);

			int lineNum = 1;
			foreach (string line in lines)
			{
				if (line.Length > 0 && line[0] != '#')
				{
					int res = s_commandManager.RunCommand(line);
					if (res != 0)
					{
						ChiConsole.WriteLine("{0}({1}): Command failed", file, lineNum);
						break;
					}
				}

				lineNum++;
			}
		}

		static void RunPythonScript(string filePath)
		{
			string file = Path.GetFileName(filePath);

			EngineModule module = s_pythonEngine.CreateModule("<" + file + ">", false);

			try
			{
				string wrapCode =
@"import sys
try:
	execfile(r""" + filePath + @""")
except Exception:
	etype, value, tb = sys.exc_info()
	
	print ""%s: %s"" % (etype.__name__, value)
	
	while tb is not None:
		f = tb.tb_frame
		lineno = tb.tb_lineno
		co = f.f_code
		filename = co.co_filename
		name = co.co_name
		print '  File ""%s"", line %d, in %s' % (filename,lineno,name)
		tb = tb.tb_next
";
				s_pythonEngine.Execute(wrapCode, module);
			}
			catch (Exception e)
			{
				ChiConsole.WriteError("Error running script " + file, e, file);
			}
		}


		public static bool IsDebug()
		{
#if DEBUG
			return true;
#else
			return false;
#endif
		}

	}
}
