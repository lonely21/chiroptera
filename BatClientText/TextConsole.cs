
using System;
using System.Collections.Generic;
using System.Text;
using BatMud.BatClientBase;
using Mono.Unix.Native;

namespace BatMud.BatClientText
{
	class TextConsole : IBatConsole
	{
		public TextConsole()
		{
			Terminal.Init();
		}

		public void Init()
		{
			Terminal.Init();
		}
		
		public void UnInit()
		{
			Terminal.UnInit();
		}
		
		public void Reset()
		{
			Terminal.Reset();
		}

		public void RestoreNormal()
		{
			Terminal.RestoreNormal();
		}
		
		public void ReadChars()
		{
			Terminal.ReadInput();
		}
		
		public string GetLine()
		{
			return Terminal.Pop();
		}

		#region IBatConsole Members
		
		public void WriteLine(string str)
		{
			string[] lines = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			foreach (string line in lines)
			{
				ColorMessage msg = new ColorMessage(line);
				WriteLine(msg);
			}
		}

		public void WriteLine(string format, params object[] args)
		{
			WriteLine(String.Format(format, args));
		}

		public void WriteLine(ColorMessage msg)
		{
			msg = PythonInterface.ServicesDispatcher.DispatchOutputMessage(msg);
			if (msg == null)
				return;

			string str = msg.ToAnsiString();
			
			Terminal.WriteLine("dbg: " + msg.ToDebugString());
			Terminal.WriteLine("esc: " + str.Replace("\x1b", "<esc>"));
			
			Terminal.WriteLine(str);
		}

		public void WriteLineLow(string format, params object[] args)
		{
			string str = String.Format(format, args);

			string[] lines = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			foreach (string line in lines)
			{
				ColorMessage msg = new ColorMessage(line);
				Terminal.WriteLine(msg.ToAnsiString());
			}
		}
		
		public string Prompt
		{
			get { return Terminal.Prompt; }
			set { Terminal.Prompt = value; }
		}

		public string InputLine
		{
			get { return Terminal.Line; }
			set { Terminal.Line = value;}
		}

		#endregion
	}
}
