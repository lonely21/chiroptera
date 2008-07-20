
using System;
using System.Collections.Generic;
using System.Text;
using BatMud.BatClientBase;
using Mono.Unix.Native;

namespace BatMud.BatClientText
{
	class TextConsole : IBatConsole
	{
		bool m_debugOutput = false;
		bool m_256colors = true;
		
		public TextConsole()
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

		public void ReadChars()
		{
			Terminal.ReadInput();
		}
		
		public string GetLine()
		{
			return Terminal.Pop();
		}
		
		public void HandleSigWinch()
		{
			Terminal.OnScreenResize();
		}

		public void CleanupAfterSigStop()
		{
			Terminal.CleanupAfterSigStop();
		}
		
		public void RestoreAfterSigStop()
		{
			Terminal.RestoreAfterSigStop();
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

			string str = msg.ToAnsiString(m_256colors);

			if(m_debugOutput)
			{
				Terminal.WriteLine("dbg: " + msg.ToDebugString());
				Terminal.WriteLine("esc: " + str.Replace("\x1b", "<esc>"));
			}
			
			Terminal.WriteLine(str);
		}

		public void WriteLineLow(string format, params object[] args)
		{
			string str = String.Format(format, args);

			string[] lines = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			foreach (string line in lines)
			{
				ColorMessage msg = new ColorMessage(line);
				Terminal.WriteLine(msg.ToAnsiString(false));
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
