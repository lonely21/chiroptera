
using System;
using System.Collections.Generic;
using System.Text;

namespace BatMud.BatClientText
{
	public static class Dbg
	{
		public static void WriteLine(string format, params object[] args)
		{
			string str = String.Format(format, args);

			System.IO.File.AppendAllText("dbg.log", str+"\n");
		}
	}

	public class Terminal
	{
		static string m_prompt = "";

		static int m_lines = 25;
		static int m_columns = 80;
		static int m_editLines = 4;

		static string m_statusLine;

		static bool m_visualMode = true;

		static bool m_initialized = false;
		
		static string s_lastLine;
		
		public static void Init()
		{
			GNUReadLine.rl_clear_signals();
			GNUReadLine.mono_rl_set_catch_signals(false);
			GNUReadLine.stifle_history(100);

			GNUReadLine.rl_bind_key(12, ClearScreenHandler);
			
			Reset();

			GNUReadLine.rl_callback_handler_install("", InputHandler);
			
			m_initialized = true;
		}
		
		public static void UnInit()
		{
			if(m_initialized)
			{
				GNUReadLine.rl_callback_handler_remove();
				TermInfo.UnInit(m_visualMode);
				m_initialized = false;
			}
			
			Save();
		}
		
		public static void Load()
		{
			GNUReadLine.read_history(System.IO.Path.Combine(Program.ConfigPath, "history"));
		}
		
		static void Save()
		{
			GNUReadLine.write_history(System.IO.Path.Combine(Program.ConfigPath, "history"));
		}

		public static void Reset()
		{
			TermInfo.Init(m_visualMode);

			if(m_visualMode)
			{
				TermInfo.GetSize(out m_columns, out m_lines);

				CreateStatusLine();
				
				TermInfo.MoveCursor(m_lines - m_editLines - 1, 0);
				Console.Write(m_statusLine);

				TermInfo.SetScrollRegion(m_lines - m_editLines, m_lines);
				TermInfo.MoveCursor(m_lines - m_editLines, 0);
			}

			GNUReadLine.rl_forced_update_display();
		}


		static void CreateStatusLine()
		{
			StringBuilder sb = new StringBuilder(new String('_', m_columns));

			string s = String.Format("{0}x{1}", m_columns, m_lines);
			sb.Remove(10, s.Length);
			sb.Insert(10, s);

			s = DateTime.Now.ToShortTimeString();
			sb.Remove(m_columns - s.Length, s.Length);
			sb.Insert(m_columns - s.Length, s);
			
			m_statusLine = sb.ToString();
		}

		public static void RestoreNormal()
		{
			TermInfo.UnInit(m_visualMode);
		}

		static void SetOutputMode()
		{
			TermInfo.SaveCursor();
			TermInfo.SetScrollRegion(0, m_lines - m_editLines - 2);
			TermInfo.MoveCursor(m_lines - m_editLines - 2, 0);
		}

		static void SetInputMode()
		{
			TermInfo.SetScrollRegion(m_lines - m_editLines, m_lines);
			TermInfo.RestoreCursor();
		}

		static int ClearScreenHandler(int x, int keycode)
		{
			Reset();
			return 0;
		}
		
		static Queue<string> s_textQueue = new Queue<string>();

		public static string Pop()
		{
			if(s_textQueue.Count == 0)
				return null;

			return s_textQueue.Dequeue();
		}

		static void InputHandler(IntPtr strptr)
		{
			Dbg.WriteLine("Native {0}", strptr);

			//Encoding m_encoding = Encoding.GetEncoding("ISO-8859-1");

			string str; 
			
			//str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(nstr);

			// count # bytes
			int i = 0;
			while(System.Runtime.InteropServices.Marshal.ReadByte(strptr, i) != (byte)0)
				++i;
			Dbg.WriteLine("{0} bytes", i);
			byte[] buf = new byte [i];
			System.Runtime.InteropServices.Marshal.Copy(strptr, buf, 0, buf.Length);
			str = System.Text.Encoding.Default.GetString (buf);
			//str = m_encoding.GetString(buf);


			Dbg.WriteLine("Tuli: '{0}'", str == null ? "<null>" : str);

			if(str == null)
				return;

			if(str.Length > 0 && str != s_lastLine)
				GNUReadLine.add_history(str);

			s_lastLine = str;
			
			s_textQueue.Enqueue(str);
		}

		public static void ReadInput()
		{
			//Dbg.WriteLine("ReadInput");
			GNUReadLine.rl_callback_read_char();
		}

		public static void SigWinchHandler()
		{
			Reset();
			GNUReadLine.rl_resize_terminal();
			//GNUReadLine.rl_reset_line_state();
		}


		public static void SigStopHandler()
		{
			Dbg.WriteLine("STOP");
			//RestoreNormal();
		}

		public static void SigContHandler()
		{
			Dbg.WriteLine("CONT");
			/*return;
			Reset();
			GNUReadLine.rl_resize_terminal();
			GNUReadLine.rl_reset_line_state();*/
		}

		public static string Prompt
		{
			get { return m_prompt; }
			set
			{
				m_prompt = value;
				GNUReadLine.rl_set_prompt(value);
				GNUReadLine.rl_redisplay();
			}
		}
		
		public static string Line
		{
			get 
			{
				IntPtr strptr = GNUReadLine.mono_rl_get_line();
				// count # bytes
				int i = 0;
				while(System.Runtime.InteropServices.Marshal.ReadByte(strptr, i) != (byte)0)
					++i;
				byte[] buf = new byte [i];
				System.Runtime.InteropServices.Marshal.Copy(strptr, buf, 0, buf.Length);
				string str = System.Text.Encoding.Default.GetString (buf);
				//str = m_encoding.GetString(buf);
				return str;
			}
			
			set
			{
				GNUReadLine.mono_rl_set_line(value);
				GNUReadLine.rl_redisplay();
			}
		}
		
		public static void WriteLine(string format, params object[] args)
		{
			if (m_visualMode)
				SetOutputMode();

			string str = String.Format(format, args);
			
			if(m_visualMode)
			{
				// Move to next line
				Console.WriteLine();
				Console.Write(str);
			}
			else
			{
				GNUReadLine.mono_rl_save_and_clear();

				Console.WriteLine(str);
				
				GNUReadLine.rl_on_new_line();
				GNUReadLine.mono_rl_restore();
				//GNUReadLine.rl_redisplay();
			}

			if (m_visualMode)
				SetInputMode();
		}
	}
}