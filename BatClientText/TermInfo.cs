using System;
using System.Runtime.InteropServices;
using BatMud.BatClientBase;

public static class TermInfo
{
	[DllImport("libncurses", CallingConvention = CallingConvention.Cdecl)]
		extern static int setupterm(string term, int filedes, IntPtr errret);
/*
	[DllImport("libncurses", CallingConvention = CallingConvention.Cdecl)]
		extern static int tigetnum(string cap);
*/
	[DllImport("libncurses", CallingConvention = CallingConvention.Cdecl)]
		extern static IntPtr tigetstr(string cap);

	[DllImport("libncurses", CallingConvention = CallingConvention.Cdecl)]
		extern static IntPtr tparm(string cap, int p1);
	
	[DllImport("libncurses", CallingConvention = CallingConvention.Cdecl)]
		extern static IntPtr tparm(string cap, int p1, int p2);

	
	static bool m_initialized = false;

	static public void Init(bool fullScreen)
	{
		if(m_initialized == true)
			UnInit(fullScreen);

		int ret = setupterm(null, 1, IntPtr.Zero);

		if(ret != 0)
		{
			throw new Exception("Terminfo failed to initialize.");
		}

		string s;
	
		s = TGetStr("is1");
		if(s != null)
			Console.Write(s);

		s = TGetStr("is2");
		if(s != null)
			Console.Write(s);

		s = TGetStr("is3");
		if(s != null)
			Console.Write(s);
		
		if(fullScreen)
			Console.Write(TGetStr("smcup"));
		Console.Write(TGetStr("clear"));
	}

	static public void UnInit(bool fullScreen)
	{
		if(fullScreen)
			Console.Write(TGetStr("rmcup"));
		
		string s;
		
		s = TGetStr("rs1");
		if(s != null)
			Console.Write(s);

		s = TGetStr("rs2");
		if(s != null)
			Console.Write(s);

		s = TGetStr("rs3");
		if(s != null)
			Console.Write(s);
		
		m_initialized = false;
	}

	static public void Clear()
	{
		Console.Write(TermInfo.TGetStr("cl"));
	}

	static public bool GetSize(out int width, out int height)
	{
		if(GNUReadLine.mono_rl_get_window_size(out width, out height) == false)
		{
			BatConsole.WriteLine("Failed to get win size");
			return false;
		}
		else
		{
			return true;
		}
		//return Console.WindowWidth;
		//return TGetNum("columns");
	}
	
	static public string SetBgColor(int color)
	{
		return TParm(TGetStr("setab"), color);
	}

	static public string SetFgColor(int color)
	{
		return TParm(TGetStr("setaf"), color);
	}
	
	static public void SaveCursor()
	{
		Console.Write(TermInfo.TGetStr("sc"));
	}

	static public void RestoreCursor()
	{
		Console.Write(TermInfo.TGetStr("rc"));
	}

	static public void MoveCursor(int row, int col)
	{
		Console.Write(TParm(TGetStr("cup"), row, col));
	}

	static public void SetScrollRegion(int startRow, int endRow)
	{
		Console.Write(TParm(TGetStr("csr"), startRow, endRow));
	}



#if asd
	static int TGetNum(string cap)
	{
		int num = tigetnum(cap);

		if(num == -1 || num == -2)
		{
			throw new Exception(String.Format("tigetnum({0}) failed: {1}", cap, num));
		}

		return num;
	}
#endif
	static string TGetStr(string cap)
	{
		IntPtr strptr = tigetstr(cap);
		if(strptr == IntPtr.Zero || (int)strptr == -1)
		{
			return null;
			//throw new Exception(String.Format("tigetstr({0}) failed: {1}", cap, strptr));
		}

		// count # bytes
		int i = 0;
		while(Marshal.ReadByte(strptr, i) != (byte)0)
			++i;
		byte[] buf = new byte [i];
		Marshal.Copy(strptr, buf, 0, buf.Length);
		return System.Text.Encoding.Default.GetString (buf);
	}

	static string NativeToString(IntPtr strptr)
	{
		if(strptr == IntPtr.Zero)
		{
			throw new Exception("tparm() failed");
		}
			
		// count # bytes
		int i = 0;
		while(Marshal.ReadByte(strptr, i) != (byte)0)
			++i;
		byte[] buf = new byte [i];
		Marshal.Copy(strptr, buf, 0, buf.Length);
		return System.Text.Encoding.Default.GetString (buf);
	}

	static string TParm(string str, int p1)
	{
		return NativeToString(tparm(str, p1));
	}
		
	static string TParm(string str, int p1, int p2)
	{
		return NativeToString(tparm(str, p1, p2));
	}
}
