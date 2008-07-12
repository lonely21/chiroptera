using System;
using System.Collections.Generic;
using System.Text;
using Mono.Unix;
using Mono.Unix.Native;

namespace BatMud.BatClientText
{
	class Program
	{
		static void Main()
		{
			ClientCore clientCore = new ClientCore();

			clientCore.Run();
		}
	}
}
