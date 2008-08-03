
using System;
using System.Collections.Generic;
using System.Text;
using BatMud.BatClientBase;
using BatMud.BatClientText.Term;

namespace BatMud.BatClientText
{
	class TextConsole : IBatConsole
	{
		bool m_256colors = true;
		ParagraphContainer m_paragraphContainer;
		bool m_showOutputDebug = false;
		bool m_escapeOutput = false;

		string m_prompt = "";
		string m_statusLine;

		string m_lastLine;

		int m_lines = 0;
		int m_outputLines; // m_lines - m_editLines - 1
		int m_columns = 0;
		int m_editLines = 4;
		int m_currentLine = 0;

		bool m_visualMode = true;

		bool m_initialized = false;
		
		Queue<string> m_textQueue = new Queue<string>();
		
		static TextConsole s_textConsole;
		
		void D(string format, params object[] args)
		{
			Dbg.WriteLine("TextConsole: " + format, args); 
		}

		public static TextConsole Singleton
		{
			get { return s_textConsole; }
		}
		
		public TextConsole()
		{
			D("Init");
			
			if(s_textConsole != null)
				throw new Exception("Cannot create a new TextConsole");
			
			s_textConsole = this;
			
			m_paragraphContainer = new ParagraphContainer();
			m_paragraphContainer.paragraphAddedEvent += ParagraphAdded;

			if(m_initialized)
				throw new Exception("Terminal already initailized");

			GNUReadLine.rl_initialize();
			GNUReadLine.rl_clear_signals();
			GNUReadLine.mono_rl_set_catch_signals(false);
			GNUReadLine.rl_set_signals();
			GNUReadLine.using_history();
			GNUReadLine.stifle_history(100);

			GNUReadLine.rl_bind_key(12, ClearScreenHandler);
			GNUReadLine.rl_bind_keyseq("\x1b[6~", PageDownHandler);
			GNUReadLine.rl_bind_keyseq("\x1b[5~", PageUpHandler);
			GNUReadLine.rl_bind_keyseq("\x1b[6;5~", PageDownCtrlHandler);
			GNUReadLine.rl_bind_keyseq("\x1b[5;5~", PageUpCtrlHandler);
			
			TermInfo.Init(m_visualMode);
			
			Redraw();

			GNUReadLine.rl_callback_handler_install("", InputHandler);

			Load();

			m_initialized = true;
			
			for(int i = 0; i < 200; i++)
				m_paragraphContainer.Add(String.Format("kala {0} 1234567890 abcdefg hijklmn opqrstu vxyz 1234567890 abcdefg hijklmn opqrstu vxyz foobar {1}", 
				                                       i, i));
		}
		
		void ParagraphAdded(bool historyFull)
		{
			Paragraph p = m_paragraphContainer[m_paragraphContainer.Count-1];

			if(m_currentLine != 0)
			{
				m_currentLine += p.m_lines;
				if(m_currentLine >= m_paragraphContainer.TotalLines)
					m_currentLine = m_paragraphContainer.TotalLines - 1;
					
				return;
			}
			
			string str = p.ToAnsiString(m_256colors);
#if DEBUG
			if (m_escapeOutput)
				str = str.Replace("\x1b", "<esc>");
#endif
			
			OutputLine(str);
		}
		
		public ParagraphContainer ParagraphContainer
		{
			get { return m_paragraphContainer; }
		}

		public void UnInit()
		{
			D("UnInit");
			
			if(!m_initialized)
			{
				Dbg.WriteLine("WARNING Terminal not initialized");
				return;
			}
			
			GNUReadLine.rl_callback_handler_remove();
			TermInfo.UnInit();
			m_initialized = false;
			
			Save();
		}
		
		void CreateStatusLine()
		{
			StringBuilder sb = new StringBuilder(new String('_', m_columns));

			string s = String.Format("{0}x{1}({2})", m_columns, m_lines, m_outputLines);
			InsertStatusLineItem(sb, s, 2);

			s = DateTime.Now.Ticks.ToString();
			InsertStatusLineItem(sb, s, Math.Max(m_columns - s.Length, 0));

			s = String.Format("{0}/{1}", m_currentLine, m_paragraphContainer.TotalLines);
			InsertStatusLineItem(sb, s, 20);
			
			m_statusLine = sb.ToString();
		}
		
		void InsertStatusLineItem(StringBuilder statusLine, string str, int pos)
		{
			if(pos + str.Length > statusLine.Length)
				return;
			
			statusLine.Remove(pos, str.Length);
			statusLine.Insert(pos, str);
		}
		
		static int ClearScreenHandler(int x, int keycode)
		{
			s_textConsole.Redraw();
			return 0;
		}

		static int PageUpHandler(int x, int keycode)
		{
			ScrollUp(1);			
			return 0;
		}
		
		static int PageDownHandler(int x, int keycode)
		{
			ScrollDown(1);
			return 0;
		}
		
		static int PageUpCtrlHandler(int x, int keycode)
		{
			ScrollUp(20);
			return 0;
		}
		
		static int PageDownCtrlHandler(int x, int keycode)
		{
			ScrollDown(20);
			return 0;
		}
		
		static void ScrollUp(int lines)
		{
			s_textConsole.m_currentLine += lines;
			
			if(s_textConsole.m_currentLine >= s_textConsole.m_paragraphContainer.TotalLines)
				s_textConsole.m_currentLine = s_textConsole.m_paragraphContainer.TotalLines - 1;
			
			s_textConsole.Redraw();
		}
		
		static void ScrollDown(int lines)
		{
			s_textConsole.m_currentLine -= lines;
			
			if(s_textConsole.m_currentLine < 0)
				s_textConsole.m_currentLine = 0;

			s_textConsole.Redraw();
		}

		public void ReadChars()
		{
			//Dbg.WriteLine("ReadInput");
			GNUReadLine.rl_callback_read_char();
		}
		
		public string GetLine()
		{
			if(m_textQueue.Count == 0)
				return null;

			return m_textQueue.Dequeue();
		}
		
		public void HandleSigWinch()
		{
			Redraw();
		}

		public void CleanupAfterSigStop()
		{
			D("CleanupAfterSigStop");
			
			if(m_visualMode)
			{
				TermInfo.SetScrollRegion(0, m_lines);
				TermInfo.Clear();
			}
			
			GNUReadLine.rl_cleanup_after_signal();
		}
		
		public void RestoreAfterSigStop()
		{
			D("RestoreAfterSigStop");
			GNUReadLine.rl_reset_after_signal();
			Redraw();
		}
		
		
		void Redraw()
		{
			int columns, lines;
			TermInfo.GetSize(out columns, out lines);

			if(columns != m_columns)
				m_paragraphContainer.SetColumns(columns);

			m_columns = columns;
			m_lines = lines;
			m_outputLines = m_lines - m_editLines - 1;
			
			if(m_visualMode)
			{
				TermInfo.Clear();
			
				RedrawStatusLine();
				RedrawOutputLines();
				// move to top of input area
				TermInfo.MoveCursor(m_lines - m_editLines, 0);

				GNUReadLine.rl_on_new_line();
			}
			
			GNUReadLine.rl_resize_terminal();
		}
		
		void RedrawStatusLine()
		{
			if(m_visualMode)
			{
				CreateStatusLine();
				// move to status line
				TermInfo.MoveCursor(m_lines - m_editLines - 1, 0);
				Console.Write(m_statusLine);
			}
		}

		void RedrawOutputLines()
		{
			if(m_paragraphContainer.Count == 0)
				return;

			int p = m_paragraphContainer.Count - 1;
			int l = 0;
			
			/* find the bottom paragraph */
			while(p > 0 && l < m_currentLine)
			{
				l += m_paragraphContainer[p].m_lines;
				p--;
			}
			
			int bottomPara = p;		// bottom full para
			int bottomLines = l - m_currentLine;	// lines shown of the bottom partial para
			
			Dbg.WriteLine("Bottom paragraph {0}, lines {1}", bottomPara, bottomLines);
			
			l = bottomLines;
			
			/* find the top paragraph */
			while(p >= 0 && l < m_outputLines)
			{
				l += m_paragraphContainer[p].m_lines;
				p--;
			}
			
			p++;
			
			int topPara = p;	// top full or partial para
			int topLines = 0;	// lines hidden of the top partial para
			int emptyLines = 0;
			
			if(l - m_outputLines < 0)
				emptyLines = Math.Abs(l - m_outputLines);
			else
				topLines = l - m_outputLines;

			Dbg.WriteLine("Top paragraph {0}, lines {1}, emptyLines {2}", topPara, topLines, emptyLines);
			
			RedrawOutputLines2();
		}
		
		void RedrawOutputLines2()
		{
			if(m_paragraphContainer.Count == 0)
				return;
			
			//TermInfo.SaveCursor();
			//TermInfo.SetScrollRegion(0, m_outputLines + 1);

			int l = 0;
			int p = m_paragraphContainer.Count - 1;
			
			for(; p >= 0; p--)
			{
				if(l >= m_currentLine)
					break;
				
				l += m_paragraphContainer[p].m_lines;
			}
			
			//Dbg.WriteLine("bottom para {0}, lines {1}", p, l - m_currentLine);
			
			int linesLastPara = l - m_currentLine;

			l = linesLastPara;
			
			for(; p >= 0; p--)
			{
				l += m_paragraphContainer[p].m_lines;
				
				if(l >= m_outputLines)
					break;
			}
			
			//Dbg.WriteLine("top p {0}, total l {1}", p, l);
			
			if(p < 0)
				p = 0;

			//Dbg.WriteLine("upmost paragraph {0}", p);
			
			l = m_outputLines - l;

			if(l < 0)
			{
				// top paragraph is partial
				int start = Math.Abs(l) * m_columns;
				Paragraph para = m_paragraphContainer[p];
				string str = para.ToAnsiString(m_256colors, start, para.m_text.Length - start);
				//Dbg.WriteLine("topmost ({1},{2}) '{0}'", str, start, para.m_text.Length - start);

				TermInfo.MoveCursor(0, 0);
				Console.Write(str);
				
				l = m_paragraphContainer[p].m_lines - Math.Abs(l);
				
				p++;
			}
			
			
			//Dbg.WriteLine("starting to draw from line {0}, para {1}", l, p);
			
			for(; p <= m_paragraphContainer.Count - 1; p++)
			{
				string str = m_paragraphContainer[p].ToAnsiString(m_256colors);

				TermInfo.MoveCursor(l, 0);
				Console.Write(str);
				
				l += m_paragraphContainer[p].m_lines;
				if(l >= m_outputLines)
					break;
			}

			//TermInfo.SetScrollRegion(m_lines - m_editLines, m_lines);
			//TermInfo.RestoreCursor();
		}
		
		
		void Load()
		{
			GNUReadLine.read_history(System.IO.Path.Combine(Program.ConfigPath, "history"));
			int l = GNUReadLine.mono_history_get_length();
			if(l > 0)
			{
				GNUReadLine.history_set_pos(l);
				
				IntPtr strptr = GNUReadLine.mono_history_get(l);
				int i = 0;
				while(System.Runtime.InteropServices.Marshal.ReadByte(strptr, i) != (byte)0)
					++i;
				byte[] buf = new byte [i];
				System.Runtime.InteropServices.Marshal.Copy(strptr, buf, 0, buf.Length);
				string str = System.Text.Encoding.Default.GetString (buf);
				
				m_lastLine = str;
				Dbg.WriteLine("lastline was '{0}'", str);
			}
		}
		
		void Save()
		{
			GNUReadLine.write_history(System.IO.Path.Combine(Program.ConfigPath, "history"));
		}
		
		static void InputHandler(IntPtr strptr)
		{
			//Dbg.WriteLine("Native {0}", strptr);

			//Encoding m_encoding = Encoding.GetEncoding("ISO-8859-1");

			string str; 
			
			//str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(nstr);

			// count # bytes
			int i = 0;
			while(System.Runtime.InteropServices.Marshal.ReadByte(strptr, i) != (byte)0)
				++i;
			//Dbg.WriteLine("{0} bytes", i);
			byte[] buf = new byte [i];
			System.Runtime.InteropServices.Marshal.Copy(strptr, buf, 0, buf.Length);
			str = System.Text.Encoding.Default.GetString (buf);
			//str = m_encoding.GetString(buf);


			//Dbg.WriteLine("Tuli: '{0}'", str == null ? "<null>" : str);

			if(str == null)
				return;

			if(str.Length > 0 && str != s_textConsole.m_lastLine)
				GNUReadLine.add_history(str);
			else
				Dbg.WriteLine("skipping {0}", str);

			s_textConsole.m_lastLine = str;
			
			s_textConsole.m_textQueue.Enqueue(str);
		}

		void OutputLine(string str)
		{
			if(m_visualMode)
			{
				TermInfo.SaveCursor();
				TermInfo.SetScrollRegion(0, m_lines - m_editLines - 2);
				TermInfo.MoveCursor(m_lines - m_editLines - 2, 0);
				
				// Move to next line (and possibly scroll the screen)
				Console.WriteLine();
				Console.Write(str);
				
				TermInfo.SetScrollRegion(m_lines - m_editLines, m_lines);
				TermInfo.RestoreCursor();
			}
			else
			{
				GNUReadLine.mono_rl_save_and_clear();

				Console.WriteLine(str);
				
				GNUReadLine.rl_on_new_line();
				GNUReadLine.mono_rl_restore();
			}
		}
		
		#region IBatConsole Members
		
		public void WriteLine(string str)
		{
			string[] lines = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			TextStyle style = new TextStyle();
			foreach (string line in lines)
			{
				ColorMessage msg = ColorMessage.CreateFromAnsi(line, style);
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

#if DEBUG
			if (m_showOutputDebug)
				m_paragraphContainer.Add("dbg: " + msg.ToDebugString());
#endif

			Paragraph p = new Paragraph(msg);

#if DEBUG
			if (m_showOutputDebug)
				m_paragraphContainer.Add("esc: " + p.ToAnsiString(m_256colors).Replace("\x1b", "<esc>"));
#endif
			
			m_paragraphContainer.Add(p);
		}

		public void WriteLineLow(string format, params object[] args)
		{
			string str = String.Format(format, args);

			string[] lines = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			TextStyle style = new TextStyle();
			foreach (string line in lines)
			{
				ColorMessage msg = ColorMessage.CreateFromAnsi(line, style);
				m_paragraphContainer.Add(new Paragraph(msg));
			}
		}
		
		public string Prompt
		{
			get { return m_prompt; }
			set
			{
				m_prompt = value;
				GNUReadLine.rl_set_prompt(value);
				GNUReadLine.rl_redisplay();
			}
		}
		
		public string InputLine
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

		#endregion
	}
}
