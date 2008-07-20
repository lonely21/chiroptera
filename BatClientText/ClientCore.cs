using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using BatMud.BatClientBase;
using IronPython.Hosting;
using IronPython.Runtime;
using Mono.Unix;
using Mono.Unix.Native;

namespace BatMud.BatClientText
{
	public class ClientCore : INetwork
	{
		public static ClientCore s_clientCore;

		TextConsole m_textConsole;
		Telnet m_telnet;
		PythonEngine m_pythonEngine;

		BaseServicesDispatcher m_baseServicesDispatcher;
		CommandManager m_commandManager;
		TriggerManager m_triggerManager;
		KeyManager m_keyManager;
		HiliteManager m_hiliteManager;

		SynchronizedInvoke m_synchronizedInvoke;

		public UnixPipes m_netPipe;
		
		bool m_pythonMode = false;
		bool m_exit = false;
		
		bool m_debugReceive = false;

		public ClientCore()
		{
			s_clientCore = this;

			//Ansi.SendAnsiInit();

			m_synchronizedInvoke = new SynchronizedInvoke();

			// Services
			m_baseServicesDispatcher = new BaseServicesDispatcher();

			// Init console
			m_textConsole = new TextConsole();
			BatConsole.SetBatConsole(m_textConsole);
			
			// Initialize ironpython
			IronPython.Compiler.Options.GenerateModulesAsSnippets = true;
			m_pythonEngine = new PythonEngine();

			BatPythonStream s = new BatPythonStream();
			m_pythonEngine.SetStandardOutput(s);
			m_pythonEngine.SetStandardError(s);
			m_pythonEngine.SetStandardInput(s);
			//m_pythonEngine.AddToPath(Environment.CurrentDirectory);
			//m_pythonEngine.AddToPath(Application.StartupPath + "/lib");
#if DEBUG
			m_pythonEngine.AddToPath(@"../../../scripts/lib");
#endif
			m_pythonEngine.LoadAssembly(typeof(TriggerManager).Assembly); // load BatClientBase
			m_pythonEngine.LoadAssembly(typeof(System.Drawing.Bitmap).Assembly); // load System.Drawing
			m_pythonEngine.LoadAssembly(typeof(System.Windows.Forms.Keys).Assembly); // load System.Windows.Forms
			
			
			
			// Network
			m_telnet = new Telnet();
			m_telnet.connectEvent += new Telnet.ConnectDelegate(_ConnectEvent);
			m_telnet.disconnectEvent += new Telnet.DisconnectDelegate(_DisconnectEvent);
			m_telnet.receiveEvent += new Telnet.ReceiveDelegate(_ReceiveEvent);
			m_telnet.promptEvent += new Telnet.PromptDelegate(_PromptEvent);
			m_telnet.telnetEvent += new Telnet.TelnetDelegate(_TelnetEvent);

			m_netPipe = UnixPipes.CreatePipes();
			

			m_commandManager = new CommandManager(m_baseServicesDispatcher);
			AddBuiltinCommands();

			m_triggerManager = new TriggerManager(m_baseServicesDispatcher);

			m_keyManager = new KeyManager(m_baseServicesDispatcher);

			m_hiliteManager = new HiliteManager(m_triggerManager);

			PythonInterface.Initialize(m_baseServicesDispatcher, m_triggerManager, m_commandManager, this,
				m_textConsole, m_pythonEngine, m_keyManager, m_hiliteManager);

			// run init script

			BatConsole.WriteLine("Using {0}", PythonEngine.VersionString);
						
			try
			{
#if DEBUG
				PythonInterface.RunScript(Path.GetFullPath("../../../scripts/std/init_std.bc"));
#else
				PythonInterface.RunScript(Path.Combine(Environment.CurrentDirectory, "std/init_std.bc"));
#endif
			}
			catch (Exception e)
			{
				BatConsole.WriteError("Error running init_std.bc", e);
			}
			
			
/*
			m_pythonEngine.Import("site");

			try
			{
				m_pythonEngine.ExecuteFile("init.py");
			}
			catch (Exception e)
			{
				BatConsole.WriteError("Eval failed", e);
			}
			*/
		}

		void AddBuiltinCommands()
		{
			m_commandManager.AddCommand("quit", QuitCommandHandler, "exit the client",
"usage: /quit\n\nExits the client.");

			m_commandManager.AddCommand("eval", EvalCommandHandler, "evaluate python code",
"usage: /eval <python code>\n\nEvaluates the given python code and returns the value of the expression. " +
"The code is executed in the main python environment.");

			m_commandManager.AddCommand("run", RunCommandHandler, "run script",
"usage: /run <script file>\n\nExecutes a python (.py) or batclient (.bc) script. If no file extension is " +
"given, .py extension is tried first and .bc second. The script is first searched from <user's documents dir>/BatClient " +
"and the from <batclient dir>/std. Python scripts are ran in its own python environment, and batclient scripts " +
"are executed in the main python environment.");

			// command handler is null, because this is just a help container. the command is hardcoded.
			m_commandManager.AddCommand("py", null, "Enter/Exit python mode", 
"usage: /py\n\nEnters or exits python mode. When in python mode, all input is considered as python code and is executed " +
"as if the code had been given with /eval command.");
		}

		delegate void SignalDelegate();
		bool m_sigThreadStop = false;
		
		void SignalThread()
		{
			Dbg.WriteLine("Starting Signal Thread");
			
			UnixSignal sigint = new UnixSignal(Signum.SIGINT);
			UnixSignal sigtstp = new UnixSignal(Signum.SIGTSTP);
			UnixSignal sigwinch = new UnixSignal(Signum.SIGWINCH);

			UnixSignal[] signals = { sigint, sigtstp, sigwinch };
			
			while(true)
			{
				// We need timeout to be able to stop this thread
				int s = UnixSignal.WaitAny(signals, 100);

				if(m_sigThreadStop)
				{
					Dbg.WriteLine("Stopping Signal Thread");
					return;
				}
				
				SignalDelegate del;
				
				if(s == 0)
					del = HandleSigInt;
				else if(s == 1)
					del = HandleSigTstp;
				else if(s == 2)
					del = HandleSigWinch;
				else
					continue;
				
				m_synchronizedInvoke.BeginInvoke(del, null);
				
				signals[s].Reset();
			}
		}
		
		void HandleSigInt()
		{
			m_exit = true;
			Dbg.WriteLine("SIGINT");
		}

		void HandleSigWinch()
		{
			m_textConsole.HandleSigWinch();
		}
		
		void HandleSigTstp()
		{
			Dbg.WriteLine("SIGTSTP");
			
			m_textConsole.CleanupAfterSigStop();
			
			Stdlib.raise(Signum.SIGSTOP);

			m_textConsole.RestoreAfterSigStop();
			
			Dbg.WriteLine("cont");
		}
		
		public void Run()
		{
			Thread m_signalThread = new Thread(SignalThread);
			m_signalThread.Start();
			
			Pollfd[] fds = new Pollfd[2];
			
			while(m_exit == false)
			{
				fds[0].fd = Mono.Unix.UnixStream.StandardInputFileDescriptor;
				fds[0].events = PollEvents.POLLIN;
				fds[0].revents = 0;
				
				fds[1].fd = m_netPipe.Reading.Handle;
				fds[1].events = PollEvents.POLLIN;
				fds[1].revents = 0;

				int ret = Syscall.poll(fds, -1);

				if(ret == 0)
				{
					//BatConsole.Prompt = String.Format("pr{0}> ", z++);
					BatConsole.WriteLine("timeout");
				}
				else if(ret > 0)
				{
					if(fds[0].revents != 0)
					{
						m_textConsole.ReadChars();
						
						string str;
						while((str = m_textConsole.GetLine()) != null)
						{
							//m_textConsole.WriteLine("Tuli {0}", str);
							HandleInput(str);
						}
					}
					
					if(fds[1].revents != 0)
					{
						m_netPipe.Reading.ReadByte();
						m_synchronizedInvoke.DispatchInvokes();
					}
				}
			}

			Dbg.WriteLine("Exiting");
			
			m_sigThreadStop = true;
			if(m_signalThread.Join(1000) == false)
				m_signalThread.Abort();
			
			m_textConsole.UnInit();
		}

		internal void DispatchEvents()
		{
			m_synchronizedInvoke.DispatchInvokes();
		}

		public Telnet Telnet
		{
			get { return m_telnet; }
		}

		public TriggerManager TriggerManager
		{
			get { return m_triggerManager; }
		}

		public CommandManager CommandManager
		{
			get { return m_commandManager; }
		}

		// Transfers control to main thread
		void _ConnectEvent(Exception exception, string address, int port)
		{
			m_synchronizedInvoke.BeginInvoke(new Telnet.ConnectDelegate(ConnectEvent), 
				new object[] { exception, address, port });
		}

		void ConnectEvent(Exception exception, string address, int port)
		{
			m_baseServicesDispatcher.DispatchConnectEvent(exception);

			if (exception == null)
			{
				if (address == "batmud.bat.org")
				{
					// Send version string as the first thing, so batmud recognizes us correctly
					// hcbat doesn't support this
					m_telnet.Send(String.Format("\x1b<v{0}>\n", typeof(ClientCore).Assembly.GetName().Version));
				}

				BatConsole.WriteLine("Connected to {0}:{1}", address, port);
			}
			else
			{
				BatConsole.WriteLine("Connect failed to {0}:{1} : {2}", address, port, exception.Message);
			}
		}

		// Transfers control to main thread
		void _DisconnectEvent(string address, int port)
		{
			m_synchronizedInvoke.BeginInvoke(new Telnet.DisconnectDelegate(DisconnectEvent),
				new object[] { address, port });
		}

		void DisconnectEvent(string address, int port)
		{
			m_baseServicesDispatcher.DispatchDisconnectEvent();

			BatConsole.WriteLine("Disconnected from {0}:{1}", address, port);

			BatConsole.Prompt = "";
			//m_mainWindow.PromptTextBox.PromptPassword = false;
		}

		// Transfers control to main thread
		void _ReceiveEvent(string data)
		{
			m_synchronizedInvoke.BeginInvoke(new Telnet.ReceiveDelegate(ReceiveEvent), new object[] { data });
		}

		TextStyle m_currentStyle = new TextStyle();
		
		void ReceiveEvent(string data)
		{
			string[] strs = data.Split('\n');
			
			foreach(string str in strs)
			{
				if(m_debugReceive)
					BatConsole.WriteLineLow("rcv: " + str.Replace("\x1b", "<esc>"));
				
				ColorMessage colorMsg = Ansi.ParseAnsi(str, ref m_currentStyle);
				
				colorMsg = m_baseServicesDispatcher.DispatchReceiveColorMessage(colorMsg);
				
				if (colorMsg == null)
					return;
				
				BatConsole.WriteLine(colorMsg);
			}
		}

		// Transfers control to main thread
		void _PromptEvent(string data)
		{
			m_synchronizedInvoke.BeginInvoke(new Telnet.PromptDelegate(PromptEvent), new object[] { data });
		}

		void PromptEvent(string data)
		{
			ColorMessage colorMsg = new ColorMessage(data);

			colorMsg = m_baseServicesDispatcher.DispatchPromptEvent(colorMsg);

			if (colorMsg == null)
				return;

			if(!m_pythonMode)
				BatConsole.Prompt = colorMsg.Text;
		}

		// Transfers control to main thread
		void _TelnetEvent(Telnet.TelnetCodes code, Telnet.TelnetOpts opt)
		{
			m_synchronizedInvoke.BeginInvoke(new Telnet.TelnetDelegate(TelnetEvent), new object[] { code, opt });
		}

		void TelnetEvent(Telnet.TelnetCodes code, Telnet.TelnetOpts opt)
		{
			m_baseServicesDispatcher.DispatchTelnetEvent(code, opt);

			if (code == Telnet.TelnetCodes.WILL && opt == Telnet.TelnetOpts.TELOPT_ECHO)
			{
				//m_mainWindow.PromptTextBox.PromptPassword = true;
			}
			else if (code == Telnet.TelnetCodes.WONT && opt == Telnet.TelnetOpts.TELOPT_ECHO)
			{
				//m_mainWindow.PromptTextBox.PromptPassword = false;
			}
		}

		public void HandleInput(string input)
		{
			if (input == "/py")
			{
				m_pythonMode = !m_pythonMode;

				if (m_pythonMode)
				{
					BatConsole.WriteLine("Python mode enabled. Use /py to exit python mode.");
					BatConsole.Prompt = "python> ";
				}
				else
				{
					BatConsole.WriteLine("Python mode disabled.");
					BatConsole.Prompt = "";
				}
				return;
			}

			if (m_pythonMode)
			{
				if (input == null)
					return;
				EvalCommandHandler(input);
				return;
			}

			input = m_baseServicesDispatcher.DispatchInputEvent(input);

			if (input == null)
				return;

			if (m_telnet.IsConnected)
			{
				SendLine(input);
				BatConsole.Prompt = "";
			}
			else
				BatConsole.WriteLine("Not connected.");
		}

		int QuitCommandHandler(string input)
		{
			m_exit = true;
			return 0;
		}

		int EvalCommandHandler(string input)
		{

			if (input.Length == 0)
				return -1;

			string source = input;

			try
			{
				//WriteLine("< " + source);

				m_pythonEngine.ExecuteToConsole(source);
			}
			catch (Exception e)
			{
				BatConsole.WriteError("eval failed", e);
			}

			return 0;
		}

		int RunCommandHandler(string input)
		{
			/*
			if (args.Length != 1)
			{
				return -1;
			}
			*/
			PythonInterface.RunScript(input);

			return 0;
		}

		#region INetwork Members

		public void Connect(string address, int port)
		{
			if (m_telnet.IsConnected)
				m_telnet.Disconnect();

			m_telnet.Connect(address, port);
		}

		public void Disconnect()
		{
			m_telnet.Disconnect();
		}

		public bool IsConnected
		{
			get { return m_telnet.IsConnected; }
		}

		public void SendLine(string str)
		{
			m_telnet.Send(str + "\n");
		}

		public void SendLine(string format, params object[] args)
		{
			SendLine(String.Format(format, args));
		}

		public void ReceiveLine(string str)
		{
			ReceiveEvent(str);
		}

		#endregion
	}
}
