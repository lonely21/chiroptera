
using System;
using System.Collections.Generic;
using System.Text;
using BatMud.BatClientBase;
using Mono.Unix.Native;

namespace BatMud.BatClientText
{
	class TextConsole : IBatConsole
	{
		static TextConsole s_console;

		string m_prompt = "";
		
		public TextConsole()
		{
			s_console = this;
			
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
			Terminal.WriteLine(str);
			/*
			if(m_visualMode)
				SetOutputMode();

			string[] lines = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			foreach (string line in lines)
			{
				ColorMessage msg = new ColorMessage(line);

				msg = PythonInterface.ServicesDispatcher.DispatchOutputMessage(msg);
				if (msg == null)
					continue;

				if(m_visualMode)
				{
					// Move to next line
					Console.WriteLine();
					Console.Write(msg.ToAnsiString());
				}
				else
				{
					Console.WriteLine(msg.ToAnsiString());
				}
			}

			if(m_visualMode)
				SetInputMode();
				*/
		}

		public void WriteLine(string format, params object[] args)
		{
			WriteLine(String.Format(format, args));
		}

		public void WriteLine(ColorMessage msg)
		{
			string str = msg.ToAnsiString();
			Terminal.WriteLine(str);
			/*
			
			if(m_visualMode)
				SetOutputMode();
			
			msg = PythonInterface.ServicesDispatcher.DispatchOutputMessage(msg);
			if (msg == null)
				return;
			
			if(m_visualMode)
			{
				// Move to next line
				Console.WriteLine();
				Console.Write(msg.ToAnsiString());
			}
			else
			{
				Console.WriteLine(msg.ToAnsiString());
			}
			
			if(m_visualMode)
				SetInputMode();
				*/
		}

		public void WriteLineLow(string format, params object[] args)
		{
			Terminal.WriteLine(format, args);
			/*
			if (m_visualMode)
				SetOutputMode();

			string str = String.Format(format, args);

			string[] lines = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			foreach (string line in lines)
			{
				ColorMessage msg = new ColorMessage(line);

				if (m_visualMode)
				{
					// Move to next line
					Console.WriteLine();
					Console.Write(msg.ToAnsiString());
				}
				else
				{
					Console.WriteLine(msg.ToAnsiString());
				}
			}

			if (m_visualMode)
				SetInputMode();
				*/
		}
		public string ReadLine()
		{
			throw new NotImplementedException();
			//return GNUReadLine.ReadLine(m_prompt);
		}
		
		public string Prompt
		{
			get { return m_prompt; }
			set
			{
				m_prompt = value;
				GNUReadLine.SetPrompt(s_console.m_prompt);
			}
		}

		public string InputLine
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

		#endregion
	}
}
