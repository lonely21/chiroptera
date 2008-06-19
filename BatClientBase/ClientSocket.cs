using System;
using System.Net;
using System.Net.Sockets;

namespace BatMud.BatClientBase
{
	/// <summary>
	/// Summary description for ClientSocket.
	/// </summary>
	class ClientSocket
	{
		public delegate void ConnectDelegate(Exception exception, string address, int port);
		public delegate int ReceiveDelegate(byte[] data, int len);
		public delegate void DisconnectDelegate(string address, int port);

		public event ConnectDelegate connectEvent;
		public event ReceiveDelegate receiveEvent;
		public event DisconnectDelegate disconnectEvent;

		private Socket m_socket = null;

		private const int m_receiveBufferSize = 16384;
		private byte[] m_receiveBuffer = new byte[m_receiveBufferSize];
		private int m_receiveBufferUsed = 0;

		private string m_address;
		private int m_port;

		enum State
		{
			None,
			Resolving,
			Connecting,
			Connected,
			Disconnected
		}

		State m_state;

		public ClientSocket()
		{
			m_state = State.None;
		}

		public string Address
		{
			get { return m_address; }
		}

		public int Port
		{
			get { return m_port; }
		}

		public void Connect(string address, int port)
		{
			if(m_state != State.None)
			{
				throw new Exception("Connect: socket in wrong state");
			}

			m_address = address;
			m_port = port;

			m_state = State.Resolving;

            Dns.BeginGetHostEntry(m_address, new AsyncCallback(OnResolve), 0);
		}

		public void Disconnect()
		{
			lock (this)
			{
				if (m_socket != null && m_socket.Connected)
				{
					m_socket.Shutdown(SocketShutdown.Both);
				}
			}

			m_state = State.Disconnected;
		}

		public bool IsConnected
		{
			get
			{
				if(m_socket != null)
				{
					return m_socket.Connected;
				}
				else
				{
					return false;
				}
			}
		}

		public void Send(byte[] data)
		{
			Send(data, data.Length);
		}

		public void Send(byte[] data, int len)
		{
			lock (this)
			{
				if (m_socket != null && m_socket.Connected)
				{
					m_socket.Send(data, 0, len, SocketFlags.None);
				}
			}
		}

		private void OnResolve(IAsyncResult ar)
		{
			if(m_state != State.Resolving)
			{
				return;
			}

			try
			{
				IPHostEntry hostEntry = Dns.EndGetHostEntry(ar);

				IPAddress[] addrList = hostEntry.AddressList;

				if (addrList.Length == 0)
				{
					m_state = State.Disconnected;

					if (connectEvent != null)
					{
						connectEvent(new Exception("Failed to resolve"), m_address, m_port);
					}

					return;
				}

				m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				IPEndPoint ipEndPoint = new IPEndPoint(addrList[0], m_port);

				m_state = State.Connecting;

				m_socket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), 0);
			}
			catch (Exception e)
			{
				if (connectEvent != null)
				{
					connectEvent(new Exception("Failed to resolve: " + e.Message), m_address, m_port);
				}
			}
		}

		private void OnConnect(IAsyncResult ar)
		{
			if(m_state != State.Connecting)
			{
				return;
			}

			try
			{
				m_socket.EndConnect(ar);
			}
			catch(SocketException e)
			{
				m_state = State.Disconnected;
				m_socket = null;
				if(connectEvent != null)
				{
					connectEvent(e, m_address, m_port);
				}
				return;
			}
			
			m_state = State.Connected;

			if(connectEvent != null)
			{
				connectEvent(null, m_address, m_port);
			}

			m_socket.BeginReceive(m_receiveBuffer, m_receiveBufferUsed, m_receiveBufferSize - m_receiveBufferUsed,
				SocketFlags.None, new AsyncCallback(OnReceiveData), m_socket);
		}

		private void OnReceiveData(IAsyncResult ar)
		{
			Socket socket = (Socket)ar.AsyncState;

			int len = 0;

			try
			{
				len = socket.EndReceive(ar);
			}
			catch (SocketException)
			{
				len = 0;
			}

			if(len == 0)
			{
				lock(this)
				{
					m_socket.Shutdown(SocketShutdown.Both);
					m_socket.Close();

					m_socket = null;

					if(disconnectEvent != null)
					{
						disconnectEvent(m_address, m_port);
					}
				}

				m_state = State.Disconnected;

				return;
			}

			m_receiveBufferUsed += len;

			if(receiveEvent != null)
			{
				int handled;

				handled = receiveEvent(m_receiveBuffer, m_receiveBufferUsed);

				System.Array.Copy(m_receiveBuffer, handled, m_receiveBuffer, 0, m_receiveBufferUsed - handled);

				m_receiveBufferUsed = m_receiveBufferUsed - handled;
			}
			else
			{
				m_receiveBufferUsed = 0;
			}

			m_socket.BeginReceive(m_receiveBuffer, m_receiveBufferUsed, m_receiveBufferSize - m_receiveBufferUsed,
				SocketFlags.None, new AsyncCallback(OnReceiveData), m_socket);
		}

	}
}
