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
			Stdlib.signal(Signum.SIGINT, SigIntHandler);

			ClientCore clientCore = new ClientCore();

			clientCore.Run();

			TextConsole.RestoreNormal();
		}

		static void SigIntHandler(int signal)
		{
			TextConsole.RestoreNormal();
			
			Stdlib.exit(Stdlib.EXIT_FAILURE);
		}
	}
}
