using System;
using System.Collections.Generic;
using System.Text;

namespace BatMud.BatClientBase
{
	public interface IBatConsole
	{
		void WriteLine(string str);
		void WriteLine(string format, params object[] args);
		void WriteLine(ColorMessage msg);
		void WriteLineLow(string format, params object[] args);

		string ReadLine();

		string Prompt
		{
			get;
			set;
		}

		string InputLine
		{
			get;
			set;
		}
	}

	public static class BatConsole
	{
		#region DefaultConsole
		class DefaultBatConsole : IBatConsole
		{
			public DefaultBatConsole()
			{
			}

			#region IBatConsole Members

			public void WriteLine(string str)
			{
				Console.WriteLine(str);
			}

			public void WriteLine(string format, params object[] args)
			{
				Console.WriteLine(format, args);
			}

			public void WriteLine(ColorMessage msg)
			{
				Console.WriteLine(msg.Text);
			}

			public void WriteLineLow(string format, params object[] args)
			{
				Console.WriteLine(format, args);
			}

			public string ReadLine()
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public string Prompt
			{
				get
				{
					throw new Exception("The method or operation is not implemented.");
				}
				set
				{
					throw new Exception("The method or operation is not implemented.");
				}
			}

			public string InputLine
			{
				get { return ""; }
				set { }
			}

			#endregion
		}
		#endregion

		static IBatConsole s_console = new DefaultBatConsole();

		public static void SetBatConsole(IBatConsole console)
		{
			 s_console = console;
		}

		public static void WriteLine(string str)
		{
			s_console.WriteLine(str);
		}

		public static void WriteLine(string format, params object[] args)
		{
			s_console.WriteLine(format, args);
		}

		public static void WriteLine(ColorMessage msg)
		{
			s_console.WriteLine(msg);
		}

		public static void WriteLineLow(string format, params object[] args)
		{
			s_console.WriteLineLow(format, args);
		}

		public static string ReadLine()
		{
			return s_console.ReadLine();
		}

		public static string Prompt
		{
			get { return s_console.Prompt; }
			set { s_console.Prompt = value; }
		}

		public static void WriteError(string error)
		{
			WriteError(error, null, null);
		}
		
		public static void WriteError(string error, Exception e)
		{
			WriteError(error, e, null);
		}

		public static void WriteError(string error, Exception e, string scriptName)
		{
			s_console.WriteLineLow(error + ":");

			if (e != null)
			{
				if (e.Source == "IronPython")
				{
					if (e is IronPython.Runtime.Exceptions.PythonSyntaxErrorException)
					{
						IronPython.Runtime.Exceptions.PythonSyntaxErrorException ee =
							(IronPython.Runtime.Exceptions.PythonSyntaxErrorException)e;
						s_console.WriteLineLow("{0}({1},{2}): {3}", ee.FileName, ee.Line, ee.Column, ee.Message);
					}
					else
					{
						if (scriptName != null && scriptName.Length > 0)
							s_console.WriteLineLow("{0}: {1}", scriptName, e.Message);
						else
							s_console.WriteLineLow(e.Message);
					}
				}
				else
				{
					s_console.WriteLineLow(e.ToString());
				}
			}
		}

	}
}
