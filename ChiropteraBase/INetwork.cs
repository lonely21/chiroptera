using System;
using System.Collections.Generic;
using System.Text;

namespace Chiroptera.Base
{
	public interface INetwork
	{
		void Connect(string address, int port);
		void Disconnect();

		bool IsConnected
		{
			get;
		}

		void SendLine(string str);
		void SendLine(string format, params object[] args);

		void ReceiveLine(string str);
	}
}
