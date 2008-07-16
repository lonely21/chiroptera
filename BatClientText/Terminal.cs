
using System;
using System.Collections.Generic;
using System.Text;

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
	static int m_editLines = 30;

	static string m_statusLine;

	static bool m_visualMode = true;

	static bool m_initialized = false;
	
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
			if(m_visualMode)
				TermInfo.UnInit();
			m_initialized = false;
		}
	}

	public static void Reset()
	{
		if(m_visualMode)
		{
			TermInfo.Init();

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

		m_statusLine = sb.ToString();
	}

	public static void RestoreNormal()
	{
		if(!m_visualMode)
			return;

		TermInfo.UnInit();
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

		if(str.Length > 0)
			GNUReadLine.add_history(str);

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
			GNUReadLine.SetPrompt(m_prompt);
		}
	}
	
	public static void WriteLine(string format, params object[] args)
	{
		if (m_visualMode)
			SetOutputMode();

		string str = String.Format(format, args);
		
		const char ESC = '\x1b';
		string estr = str.Replace(ESC.ToString(), "<esc>"); 

		str = estr;		
		
		Dbg.WriteLine("Output: '{0}'", estr);

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

#region IBatConsole Members
#if asd
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

#endif
#endregion
}
