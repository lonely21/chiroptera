
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

		int m_lines = 25;
		int m_columns = 80;
		int m_editLines = 4;

		bool m_visualMode = false;
		
		public TextConsole()
		{
			s_console = this;

			GNUReadLine.rl_bind_key(12, ClearScreenHandler);

			Stdlib.signal(Signum.SIGWINCH, SigWinchHandler);
			//Stdlib.signal(Signum.SIGTSTP, SigStopHandler);
			//Stdlib.signal(Signum.SIGCONT, SigContHandler);
		}

		public void Reset()
		{
			if(!m_visualMode)
				return;

			TermInfo.Init();

			//			TermInfo.SetScrollRegion(-1, -1);
					
			m_lines = TermInfo.GetLines();
			m_columns = TermInfo.GetColumns();

			TermInfo.MoveCursor(m_lines - m_editLines - 1, 0);
			Console.Write(new String('_', m_columns));

			TermInfo.SetScrollRegion(m_lines - m_editLines, m_lines);
			TermInfo.MoveCursor(m_lines - m_editLines, 0);
		}

		public static void RestoreNormal()
		{
			if(!s_console.m_visualMode)
				return;
			
			TermInfo.UnInit();
		}

		void SetOutputMode()
		{
			TermInfo.SaveCursor();
			TermInfo.SetScrollRegion(0, m_lines - m_editLines - 2);
			TermInfo.MoveCursor(m_lines - m_editLines - 2, 0);
		}

		void SetInputMode()
		{
			TermInfo.SetScrollRegion(m_lines - m_editLines, m_lines);
			TermInfo.RestoreCursor();
		}

		static int ClearScreenHandler(int x, int keycode)
		{
			s_console.Reset();
			GNUReadLine.rl_reset_line_state();
			return 0;
		}
		
		static void SigWinchHandler(int signal)
		{
			s_console.Reset();
			GNUReadLine.rl_resize_terminal();
			GNUReadLine.rl_reset_line_state();
		}

		static void SigStopHandler(int signal)
		{
			Console.WriteLine("STOP");
			//RestoreNormal();
		}
		
		static void SigContHandler(int signal)
		{
			Console.WriteLine("CONT");
			return;
			s_console.Reset();
			GNUReadLine.rl_resize_terminal();
			GNUReadLine.rl_reset_line_state();
		}

		#region IBatConsole Members
		
		public void WriteLine(string str)
		{
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
		}

		public void WriteLine(string format, params object[] args)
		{
			WriteLine(String.Format(format, args));
		}

		public void WriteLine(ColorMessage msg)
		{
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
		}

		public void WriteLineLow(string format, params object[] args)
		{
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
