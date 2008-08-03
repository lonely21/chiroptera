using System;
using System.Collections.Generic;
using System.Text;
using BatMud.BatClientBase;
using IronPython.Hosting;
using IronPython.Runtime;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Drawing;

namespace BatMud.BatClientWindows
{
	delegate object KeyEvalx();

	public class ClientCore : IBatConsole, INetwork
	{
		public static ClientCore s_clientCore;

		MainWindow m_mainWindow;
		Telnet m_telnet;
		ParagraphContainer m_paragraphContainer;
		PythonEngine m_pythonEngine;

		BaseServicesDispatcher m_baseServicesDispatcher;
		CommandManager m_commandManager;
		TriggerManager m_triggerManager;
		HiliteManager m_hiliteManager;
		KeyManager m_keyManager;

		bool m_pythonMode = false;
		bool m_showOutputDebug = false;

		public ClientCore()
		{
			s_clientCore = this;
		}

		public void Initialize(string[] args)
		{
			if (args.Length == 1 && args[0] == "/reset")
				Properties.Settings.Default.Reset();

			m_baseServicesDispatcher = new BaseServicesDispatcher();

			// Init mainwindow and display
			m_mainWindow = new MainWindow(this);
			m_paragraphContainer = new ParagraphContainer();
			m_mainWindow.TextView.ParagraphContainer = m_paragraphContainer;
			BatConsole.SetBatConsole(this);

			// Initialize ironpython
			IronPython.Compiler.Options.GenerateModulesAsSnippets = true;
			/*			IronPython.Compiler.Options.GenerateDynamicMethods = false;
						IronPython.Compiler.Options.DebugMode = true;
						IronPython.Compiler.Options.EngineDebug = true;
						IronPython.Compiler.Options.ILDebug = true;
						IronPython.Compiler.Options.Frames = true;
			*/
			m_pythonEngine = new PythonEngine();
			//m_pythonEngine.CreateModule("globals", true);

			BatPythonStream s = new BatPythonStream();
			m_pythonEngine.SetStandardOutput(s);
			m_pythonEngine.SetStandardError(s);
			m_pythonEngine.SetStandardInput(s);
			m_pythonEngine.AddToPath(Application.StartupPath + "/lib");
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



			m_commandManager = new CommandManager(m_baseServicesDispatcher);
			AddBuiltinCommands();

			m_triggerManager = new TriggerManager(m_baseServicesDispatcher);
			m_triggerManager.SetTriggers(Properties.Settings.Default.Triggers);

			m_hiliteManager = new HiliteManager(m_triggerManager);

			m_keyManager = new KeyManager(m_baseServicesDispatcher);
			m_keyManager.SetKeyBindings(Properties.Settings.Default.KeyBindings);

			PythonInterface.Initialize(m_baseServicesDispatcher, m_triggerManager, m_commandManager, this,
				this, m_pythonEngine, m_keyManager, m_hiliteManager);

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

			try
			{
				string userScript = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "BatClient/init.bc");
				if (File.Exists(userScript))
					PythonInterface.RunScript(userScript);
			}
			catch (Exception e)
			{
				BatConsole.WriteError("Error running init.bc", e);
			}

			Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			Version baseVersion = System.Reflection.Assembly.GetAssembly(typeof(Telnet)).GetName().Version;
			BatConsole.WriteLine("BatClientText version {0} (base {1})", currentVersion, baseVersion);
			BatConsole.WriteLine("Using {0}", PythonEngine.VersionString);

			CheckClientVersion();
		}

		public void Uninitialize()
		{
			Properties.Settings.Default.KeyBindings = PythonInterface.KeyManager.GetSavedKeyBindings();
			Properties.Settings.Default.Triggers = PythonInterface.TriggerManager.GetSavedTriggers();
		}

		void CheckClientVersion()
		{
			DateTime lastCheck = Properties.Settings.Default.LastVersionCheck;
			if (lastCheck + TimeSpan.FromDays(1) >= DateTime.Now)
			{
				return;
			}
			
			Properties.Settings.Default.LastVersionCheck = DateTime.Now;

			WebClient webClient = new WebClient();

			webClient.Headers.Add("pragma", "no-cache");
			webClient.Headers.Add("cache-control", "no-cache, must-revalidate, max_age=0");
			webClient.Headers.Add("expires", "0");

			webClient.OpenReadCompleted += CheckClientVersionFinished;
			webClient.OpenReadAsync(new Uri("http://www.bat.org/~tomba/BatClient2/LATEST"));
		}

		// Transfers control to MainForm's thread
		void _CheckClientVersionFinished(object sender, OpenReadCompletedEventArgs e)
		{
			m_mainWindow.BeginInvoke(new OpenReadCompletedEventHandler(CheckClientVersionFinished), 
				new object[] { sender, e });
		}

		void CheckClientVersionFinished(object sender, OpenReadCompletedEventArgs e)
		{
			Stream reply = null;
			StreamReader s = null;

			try
			{
				reply = (Stream)e.Result;
				s = new StreamReader(reply);
				string versionString = s.ReadToEnd();

				Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
				Version v = new Version(versionString);

				if (v > currentVersion)
				{
					BatConsole.WriteLine("There is a new version of BatClient available.");
					BatConsole.WriteLine("You can get new versions at http://www.bat.org/~tomba/batclient.html.");
				}
			}
			catch (Exception)
			{
				BatConsole.WriteLine("Failed to check for new version.");
				BatConsole.WriteLine("You can get new versions at http://www.bat.org/~tomba/batclient.html.");
			}
			finally
			{
				if (s != null)
				{
					s.Close();
				}

				if (reply != null)
				{
					reply.Close();
				}
			}
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

			m_commandManager.AddCommand("py", null, "Enter/Exit python mode", // command handler is null, because this is just a help container
"usage: /py\n\nEnters or exits python mode. When in python mode, all input is considered as python code and is executed " +
"as if the code had been given with /eval command."); 
		}

		public MainWindow MainWindow
		{
			get { return m_mainWindow; }
		}
		/*
		public Telnet Telnet
		{
			get { return m_telnet; }
		}
		*/
		public TriggerManager TriggerManager
		{
			get { return m_triggerManager; }
		}

		public CommandManager CommandManager
		{
			get { return m_commandManager; }
		}
		
		// Transfers control to MainForm's thread
		void _ConnectEvent(Exception exception, string address, int port)
		{
			m_mainWindow.BeginInvoke(new Telnet.ConnectDelegate(ConnectEvent), new object[] { exception, address, port });
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

				WriteLine("Connected to {0}:{1}", address, port);
			}
			else
			{
				WriteLine("Connect failed to {0}:{1} : {2}",address, port, exception.Message);
			}
		}

		// Transfers control to MainForm's thread
		void _DisconnectEvent(string address, int port)
		{
			m_mainWindow.BeginInvoke(new Telnet.DisconnectDelegate(DisconnectEvent), new object[] { address, port });
		}

		void DisconnectEvent(string address, int port)
		{
			m_baseServicesDispatcher.DispatchDisconnectEvent();

			WriteLine("Disconnected from {0}:{1}", address, port);

			m_mainWindow.PromptTextBox.Prompt = "";
			m_mainWindow.PromptTextBox.PromptPassword = false;
		}

		Queue<string> m_receiveQueue = new Queue<string>();
		bool m_receiveEventSent = false;

		delegate void ReceiveEventDelegate();

		// Transfers control to MainForm's thread
		void _ReceiveEvent(string data)
		{
			lock (m_receiveQueue)
			{
				m_receiveQueue.Enqueue(data);
				if (m_receiveEventSent == false)
				{
					m_receiveEventSent = true;
					m_mainWindow.BeginInvoke(new ReceiveEventDelegate(ReceiveEventBulk), null);
				}
			}
		}

		TextStyle m_currentStyle = new TextStyle();

		void ReceiveEventBulk()
		{
			string[] arr;

			lock (m_receiveQueue)
			{
				arr = m_receiveQueue.ToArray();
				m_receiveQueue.Clear();
				m_receiveEventSent = false;
			}

			BatConsole.WriteLineLow("Got {0} lines", arr.Length);
			foreach (string s in arr)
			{
				ReceiveEvent(s);
			}
		}

		void ReceiveEvent(string data)
		{
			string[] strs = data.Split('\n');

			foreach (string str in strs)
			{
#if DEBUG
				if (m_showOutputDebug)
					BatConsole.WriteLineLow("rcv: " + str.Replace("\x1b", "<esc>"));
#endif

				ColorMessage colorMsg = Ansi.ParseAnsi(str, ref m_currentStyle);

				colorMsg = m_baseServicesDispatcher.DispatchReceiveColorMessage(colorMsg);

				if (colorMsg == null)
					return;

				BatConsole.WriteLine(colorMsg);
			}
		}

		// Transfers control to MainForm's thread
		void _PromptEvent(string data)
		{
			m_mainWindow.BeginInvoke(new Telnet.PromptDelegate(PromptEvent), new object[] { data });
		}

		void PromptEvent(string data)
		{
			TextStyle dummy = new TextStyle();
			ColorMessage colorMsg = ColorMessage.CreateFromAnsi(data, dummy);

			colorMsg = m_baseServicesDispatcher.DispatchPromptEvent(colorMsg);

			if (colorMsg == null)
				return;

			if(!m_pythonMode)
				m_mainWindow.PromptTextBox.Prompt = colorMsg.Text;
		}

		// Transfers control to MainForm's thread
		void _TelnetEvent(Telnet.TelnetCodes code, Telnet.TelnetOpts opt)
		{
			m_mainWindow.BeginInvoke(new Telnet.TelnetDelegate(TelnetEvent), new object[] { code, opt });
		}

		void TelnetEvent(Telnet.TelnetCodes code, Telnet.TelnetOpts opt)
		{
			m_baseServicesDispatcher.DispatchTelnetEvent(code, opt);

			if (code == Telnet.TelnetCodes.WILL && opt == Telnet.TelnetOpts.TELOPT_ECHO)
			{
				m_mainWindow.PromptTextBox.PromptPassword = true;
			}
			else if (code == Telnet.TelnetCodes.WONT && opt == Telnet.TelnetOpts.TELOPT_ECHO)
			{
				m_mainWindow.PromptTextBox.PromptPassword = false;
			}
		}


		public void HandleInput(string input)
		{
			if (input == "/py" || input.StartsWith("/py "))
			{
				m_pythonMode = !m_pythonMode;

				if (m_pythonMode)
				{
					BatConsole.WriteLine("Python mode enabled.");
					m_mainWindow.PromptTextBox.Prompt = "python >";
				}
				else
				{
					BatConsole.WriteLine("Python mode disabled.");
					m_mainWindow.PromptTextBox.Prompt = "";
				}
				return;
			}

			if (m_pythonMode)
			{
				m_mainWindow.PromptTextBox.Prompt = "python >";
				
				if (input == null || input.Length == 0)
					return;

				if (input[0] != '/')
				{
					EvalCommandHandler(input);
					return;
				}
			}

			if (input.Length > 1 && input.StartsWith("\\"))
			{
				input = input.Substring(1);
				EvalCommandHandler(input);
				return;
			}

			input = m_baseServicesDispatcher.DispatchInputEvent(input);

			if (input == null)
				return;

			if (m_telnet.IsConnected)
				SendLine(input);
			else
				WriteLine("Not connected.");
		}

		public void HandleRawKeyDown(KeyEventArgs key)
		{
			//WriteLineLow("Down [{0}], [{1}], [{2}]", key.KeyValue, key.KeyCode, key.KeyData);
			
			if (m_baseServicesDispatcher.DispatchKeyDownEvent(key.KeyData) == true)
			{
				key.Handled = true;
			}
		}

		public void HandleRawKeyUp(KeyEventArgs key)
		{
			// XXX: kludge. PromptTextBox never sends keydown for tab, but it sends key up
			// So, we send keydown here
			if (key.KeyCode == Keys.Tab)
			{
				HandleRawKeyDown(key);

				if (key.Handled == true)
				{
					return;
				}
			}

			//WriteLineLow("Up   [{0}], [{1}], [{2}]", key.KeyValue, key.KeyCode, key.KeyData);

			/*
			if (m_baseServicesDispatcher.DispatchKeyUpEvent(key.KeyData) == true)
			{
				key.Handled = true;
				return;
			}
			 */
		}



		int QuitCommandHandler(string input)
		{
			System.Windows.Forms.Application.Exit();
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

		#region IBatConsole Members

		public void WriteLine(string str)
		{
			string[] lines = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			TextStyle style = new TextStyle();
			foreach (string line in lines)
			{
				ColorMessage msg = ColorMessage.CreateFromAnsi(line, style);
				WriteLine(msg);
			}
		}

		public void WriteLine(string format, params object[] args)
		{
			WriteLine(String.Format(format, args));
		}

		public void WriteLine(ColorMessage colorMsg)
		{
			colorMsg = m_baseServicesDispatcher.DispatchOutputMessage(colorMsg);

			if (colorMsg == null)
				return;

#if DEBUG
			if (m_showOutputDebug)
				m_paragraphContainer.Add("dbg: " + colorMsg.ToDebugString());
#endif

			Paragraph p = new Paragraph(colorMsg);

#if DEBUG
			if (m_showOutputDebug)
				m_paragraphContainer.Add("esc: " + p.ToDebugString());
#endif

			m_paragraphContainer.Add(p);
		}

		public void WriteLineLow(string format, params object[] args)
		{
			string str = String.Format(format, args);

			string[] lines = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			TextStyle style = new TextStyle();
			foreach (string line in lines)
			{
				ColorMessage msg = ColorMessage.CreateFromAnsi(line, style);
				m_paragraphContainer.Add(new Paragraph(msg));
			}
		}

		public string Prompt
		{
			get { return m_mainWindow.PromptTextBox.Prompt; }
			set { m_mainWindow.PromptTextBox.Prompt = value; }
		}

		public string InputLine
		{
			get
			{
				return m_mainWindow.PromptTextBox.Text;
			}

			set
			{
				m_mainWindow.PromptTextBox.Text = value;
				m_mainWindow.PromptTextBox.SelectionStart = m_mainWindow.PromptTextBox.Text.Length;
				m_mainWindow.PromptTextBox.SelectionLength = 0;
			}
		}

		#endregion

		#region INetwork Members

		public void Connect(string address, int port)
		{
			Disconnect();

			BatConsole.WriteLine("Connecting to {0}:{1}", address, port);
			m_telnet.Connect(address, port);
		}

		public void Disconnect()
		{
			if(m_telnet.IsConnected)
				BatConsole.WriteLine("Disconnecting {0}:{1}", m_telnet.Address, m_telnet.Port);

			m_telnet.Disconnect();
		}

		public bool IsConnected
		{
			get { return m_telnet.IsConnected; }
		}

		public void SendLine(string str)
		{
			if(IsConnected)
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
