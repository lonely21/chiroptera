//#define TELNET_DEBUG

using System;
using System.Text;

namespace Chiroptera.Base
{
	/// <summary>
	/// Summary description for Telnet.
	/// </summary>

	public class Telnet
	{
		#region Telnet Codes
		public enum TelnetCodes : byte
		{
			IAC = 255,		/* interpret as command: */
			DONT = 254,		/* you are not to use option */
			DO = 253,		/* please, you use option */
			WONT = 252,		/* I won't use option */
			WILL = 251,		/* I will use option */
			SB = 250,		/* interpret as subnegotiation */
			GA = 249,		/* you may reverse the line */
			EL = 248,		/* erase the current line */
			EC = 247,		/* erase the current character */
			AYT = 246,		/* are you there */
			AO = 245,		/* abort output--but let prog finish */
			IP = 244,		/* interrupt process--permanently */
			BREAK = 243,	/* break */
			DM = 242,		/* data mark--for connect. cleaning */
			NOP = 241,		/* nop */
			SE = 240,		/* end sub negotiation */
			EOR = 239,		/* end of record (transparent mode) */
			ABORT = 238,	/* Abort process */
			SUSP = 237,		/* Suspend process */
			xEOF = 236		/* End of file: EOF is already used... */

			//			SYNCH = 242		/* for telfunc calls */
		}

		public enum TelnetOpts : byte
		{
			TELOPT_BINARY = 0, /* 8-bit data path */
			TELOPT_ECHO = 1,   /* echo */
			TELOPT_RCP = 2,    /* prepare to reconnect */
			TELOPT_SGA = 3,        /* suppress go ahead */
			TELOPT_NAMS = 4,       /* approximate message size */
			TELOPT_STATUS = 5,     /* give status */
			TELOPT_TM = 6,         /* timing mark */
			TELOPT_RCTE = 7,   /* remote controlled transmission and echo */
			TELOPT_NAOL = 8,   /* negotiate about output line width */
			TELOPT_NAOP = 9,   /* negotiate about output page size */
			TELOPT_NAOCRD = 10,    /* negotiate about CR disposition */
			TELOPT_NAOHTS = 11,    /* negotiate about horizontal tabstops */
			TELOPT_NAOHTD = 12,    /* negotiate about horizontal tab disposition */
			TELOPT_NAOFFD = 13,    /* negotiate about formfeed disposition */
			TELOPT_NAOVTS = 14,    /* negotiate about vertical tab stops */
			TELOPT_NAOVTD = 15,    /* negotiate about vertical tab disposition */
			TELOPT_NAOLFD = 16,    /* negotiate about output LF disposition */
			TELOPT_XASCII = 17,    /* extended ascic character set */
			TELOPT_LOGOUT = 18,    /* force logout */
			TELOPT_BM = 19,    /* byte macro */
			TELOPT_DET = 20,   /* data entry terminal */
			TELOPT_SUPDUP = 21,    /* supdup protocol */
			TELOPT_SUPDUPOUTPUT = 22,  /* supdup output */
			TELOPT_SNDLOC = 23,    /* send location */
			TELOPT_TTYPE = 24, /* terminal type */
			TELOPT_EOR = 25,   /* end or record */
			TELOPT_TUID = 26,  /* TACACS user identification */
			TELOPT_OUTMRK = 27,    /* output marking */
			TELOPT_TTYLOC = 28,    /* terminal location number */
			TELOPT_3270REGIME = 29,    /* 3270 regime */
			TELOPT_X3PAD = 30, /* X.3 PAD */
			TELOPT_NAWS = 31,  /* window size */
			TELOPT_TSPEED = 32,    /* terminal speed */
			TELOPT_LFLOW = 33, /* remote flow control */
			TELOPT_LINEMODE = 34,  /* Linemode option */
			TELOPT_XDISPLOC = 35,  /* X Display Location */
			TELOPT_OLD_ENVIRON = 36,   /* Old - Environment variables */
			TELOPT_AUTHENTICATION = 37, /* Authenticate */
			TELOPT_ENCRYPT = 38,   /* Encryption option */
			TELOPT_NEW_ENVIRON = 39,   /* New - Environment variables */
			TELOPT_EXOPL = 255,    /* extended-options-list */
		}
		#endregion

		public delegate void ConnectDelegate(Exception exception, string address, int port);
		public delegate void DisconnectDelegate(string address, int port);
		public delegate void ReceiveDelegate(string data);
		public delegate void TelnetDelegate(TelnetCodes code, TelnetOpts opt);
		public delegate void PromptDelegate(string data);
		
		public event ConnectDelegate connectEvent;
		public event DisconnectDelegate disconnectEvent;
		public event PromptDelegate promptEvent;
		public event ReceiveDelegate receiveEvent;
		public event TelnetDelegate telnetEvent;

		Encoding m_encoding = Encoding.GetEncoding("ISO-8859-1");

		StringBuilder m_stringBuilder = new StringBuilder();

		ClientSocket m_socket;

		public Telnet()
		{
		}

		public bool IsConnected
		{
			get { return (m_socket != null) && m_socket.IsConnected; }
		}

		public string Address
		{
			get { return m_socket.Address; }
		}

		public int Port
		{
			get { return m_socket.Port; }
		}

		public void Connect(string address, int port)
		{
			if (IsConnected)
				throw new Exception("Already connected");

			Disconnect();

			m_socket = new ClientSocket();

			m_socket.connectEvent += ConnectEvent;
			m_socket.disconnectEvent += DisconnectEvent;
			m_socket.receiveEvent += ProcessData;

			m_socket.Connect(address, port);
		}
		
		public void Disconnect()
		{
			if (IsConnected)
				DisconnectEvent(m_socket.Address, m_socket.Port);

			if (m_socket != null)
			{
				m_socket.connectEvent -= ConnectEvent;
				m_socket.disconnectEvent -= DisconnectEvent;
				m_socket.receiveEvent -= ProcessData;

				m_socket.Disconnect();
				m_socket = null;
			}
		}

		public void Send(string data)
		{
			if (!IsConnected)
				throw new Exception("Not connected");

			m_socket.Send(m_encoding.GetBytes(data));
		}

		public void SendLine(string data)
		{
			if (!IsConnected)
				throw new Exception("Not connected");

			m_socket.Send(m_encoding.GetBytes(data + "\n"));
		}


		private void ConnectEvent(Exception exception, string address, int port)
		{
			if (connectEvent != null)
				connectEvent(exception, address, port);
		}

		private void DisconnectEvent(string address, int port)
		{
			if (disconnectEvent != null)
				disconnectEvent(address, port);
		}

		private int ProcessData(byte[] data, int len)
		{
			int scanned = 0;
			int pos = 0;
			while(pos < len)
			{
				switch(data[pos])
				{
					case (byte)TelnetCodes.IAC:
					{
						m_stringBuilder.Append(m_encoding.GetString(data, scanned, pos - scanned));
						scanned = pos;

						HandleIAC(data, len, ref pos, ref scanned);

						if(pos == len)
						{
							// We are at end. Incomplete/broken/whole sequence ended the buffer.
							// In any case, nothing more to receive
							return scanned;
						}

						break;
					}

					case (byte)'\r':
					{
						pos++; // skip \r
						if(pos == len)
						{
							return scanned;
						}

						m_stringBuilder.Append(m_encoding.GetString(data, scanned, pos - scanned - 1));

						if(data[pos] == (byte)'\n')
						{
							pos++; // skip \n
						}

						scanned = pos;

						string str = m_stringBuilder.ToString();

						if(receiveEvent != null)
							receiveEvent(str);

						m_stringBuilder = new StringBuilder();

						break;
					}

					case (byte)'\n':
					{
						// add the already scanned part
						m_stringBuilder.Append(m_encoding.GetString(data, scanned, pos - scanned));
						pos++;
						scanned = pos;

						string str = m_stringBuilder.ToString();

						if(receiveEvent != null)
							receiveEvent(str);

						m_stringBuilder = new StringBuilder();

						break;
					}

					case (byte)'\t':
					{
						// add the already scanned part
						m_stringBuilder.Append(m_encoding.GetString(data, scanned, pos - scanned));

						pos++;
						scanned = pos;

						int tabPos = m_stringBuilder.Length/8;
						tabPos = tabPos * 8 + 8;

						string str = new String(' ', tabPos - m_stringBuilder.Length);

						m_stringBuilder.Append(str);

						break;
					}

					case 7:
					{
						Console.Beep();

						pos++;
						scanned = pos;

						break;
					}

					default:
					{
						pos++;
						break;
					}
				}
			}

			m_stringBuilder.Append(m_encoding.GetString(data, scanned, pos - scanned));
			scanned = pos;

			return scanned;
		}

		private void HandleIAC(byte[] data, int len, ref int pos, ref int scanned)
		{
#if TELNET_DEBUG
			ChiConsole.WriteLine(((TelnetCodes)data[pos]).ToString() + " ");
#endif
			if(pos+1 >= len)
			{
#if TELNET_DEBUG
				ChiConsole.WriteLine("Uncomplete telnet sequence");
#endif
				// skip IAC. We will try again when we receive more data
				pos++;
				return;
			}

			if(System.Enum.IsDefined(TelnetCodes.ABORT.GetType(), data[pos + 1]) == false)
			{
				ChiConsole.WriteLine("Unknown telnet code " + data[pos + 1]);
				// consume IAC
				pos++;
				scanned = pos;
				return;
			}

			TelnetCodes telCode = (TelnetCodes)data[pos + 1];

#if TELNET_DEBUG
			ChiConsole.WriteLine(telCode.ToString());
#endif
			if(telCode == TelnetCodes.DO || telCode == TelnetCodes.DONT ||
				telCode == TelnetCodes.WILL || telCode == TelnetCodes.WONT)
			{
				if(pos+2 >= len)
				{
#if TELNET_DEBUG
					ChiConsole.WriteLine("Uncomplete telnet sequence");
#endif
					// skip IAC and DO/DONT. We will try again when we receive more data
					pos += 2;
					return;
				}

				if(System.Enum.IsDefined(TelnetOpts.TELOPT_ECHO.GetType(), data[pos + 2]) == false)
				{
					ChiConsole.WriteLine("Unknown telnet option " + data[pos + 2]);
					// consume IAC
					pos++;
					scanned = pos;
					return;
				}

				TelnetOpts telOpt = (TelnetOpts)data[pos + 2];

#if TELNET_DEBUG
				ChiConsole.WriteLine(telOpt.ToString());
#endif
				if(telnetEvent != null)
				{
					telnetEvent(telCode, telOpt);
				}

				pos += 3;
				scanned = pos;
				return;
			}
			else if(telCode == TelnetCodes.GA)
			{
				string str = m_stringBuilder.ToString();

				if(promptEvent != null)
				{
					promptEvent(str);
				}

				m_stringBuilder = new StringBuilder();

				pos += 2;
				scanned = pos;
				return;
			}
			else
			{
				if(telnetEvent != null)
				{
					telnetEvent(telCode, 0);
				}

				pos += 2;
				scanned = pos;
				return;
			}
		}
/*
		private void send_DO(TelnetOpts opt)
		{
			byte[] data = new byte[3];
			data[0] = (byte)TelnetCodes.IAC;
			data[1] = (byte)TelnetCodes.DO;
			data[2] = (byte)opt;


			m_clientSocket.Send(data, data.Length);
		}

		private void send_DONT(TelnetOpts opt)
		{
			byte[] data = new byte[3];
			data[0] = (byte)TelnetCodes.IAC;
			data[1] = (byte)TelnetCodes.DONT;
			data[2] = (byte)opt;

			m_clientSocket.Send(data, data.Length);
		}
		*/
	}
}
