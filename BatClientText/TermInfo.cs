using System;
using System.Runtime.InteropServices;

namespace BatMud.BatClientText
{
	class TermInfo
	{
		[DllImport("libncurses", CallingConvention = CallingConvention.Cdecl)]
		extern static int setupterm(string term, int filedes, IntPtr errret);

		[DllImport("libncurses", CallingConvention = CallingConvention.Cdecl)]
		extern static int tgetnum(string cap);

		[DllImport("libncurses", CallingConvention = CallingConvention.Cdecl)]
		extern static IntPtr tgetstr(string cap, IntPtr area);

		[DllImport("libncurses", CallingConvention = CallingConvention.Cdecl)]
		extern static IntPtr tparm(string cap, int p1, int p2);

		static bool m_initialized = false;

		static public void Init()
		{
			if(m_initialized == true)
				UnInit();
			
			int ret = setupterm(null, 1, IntPtr.Zero);

			if(ret != 0)
			{
				throw new Exception("Terminfo failed to initialize.");
			}
			
			Console.Write(TGetStr("ti"));
			Console.Write(TermInfo.TGetStr("cl"));
		}

		static public void UnInit()
		{
			Console.Write(TGetStr("te"));
			m_initialized = false;
		}

		static public void Clear()
		{
			Console.Write(TermInfo.TGetStr("cl"));
		}

		static public int GetLines()
		{
			return Console.WindowHeight;
			//return TGetNum("li");
		}

		static public int GetColumns()
		{
			return Console.WindowWidth;
			//return TGetNum("co");
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
            Console.Write(TParm(TGetStr("cm"), row, col));
		}
		
		static public void SetScrollRegion(int startRow, int endRow)
		{
            Console.Write(TParm(TGetStr("cs"), startRow, endRow));
		}


		

		static int TGetNum(string cap)
		{
			int num = tgetnum(cap);

			if(num == -1)
			{
				throw new Exception("tgetnum() failed");
			}
			
			return num;
		}

		static string TGetStr(string cap)
		{
			IntPtr strptr = tgetstr(cap, IntPtr.Zero);
			if(strptr == IntPtr.Zero)
			{
				throw new Exception("tgetstr() failed");
			}
			
			// count # bytes
			int i = 0;
			while(Marshal.ReadByte(strptr, i) != (byte)0)
				++i;
			byte[] buf = new byte [i];
			Marshal.Copy(strptr, buf, 0, buf.Length);
			return System.Text.Encoding.Default.GetString (buf);
		}

		static string TParm(string str, int p1, int p2)
		{
			IntPtr strptr = tparm(str, p1, p2);
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

	}
}
