// Dbg.cs created with MonoDevelop
// User: tomba at 11:24 AM 7/22/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace Chiroptera.Lin
{
	public static class Dbg
	{
		static Dbg()
		{
			System.IO.File.AppendAllText("dbg.log", "\n\n** START **\n");
		}
		
		public static void WriteLine(string format, params object[] args)
		{
			string str = String.Format(format, args);

			System.IO.File.AppendAllText("dbg.log", str+"\n");
		}
	}

}
