#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

#endregion

namespace Chiroptera.Base
{
	public delegate int CommandHandler(string input);

	public class CommandManager
	{
		class CommandData
		{
			public CommandHandler m_handler;
			public string m_help;
			public string m_longhelp;
		}

		Dictionary<string, CommandData> m_commandMap = new Dictionary<string, CommandData>();

		public CommandManager(BaseServicesDispatcher dispatcher)
		{
			dispatcher.RegisterInputHandler(HandleInput);

			AddCommand("help", HelpCmd, "list all commands or display help for a single command", "");
		}

		public void AddCommand(string cmd, CommandHandler handler)
		{
			AddCommand(cmd, handler, "", "");
		}

		public void AddCommand(string cmd, CommandHandler handler, string help, string longhelp)
		{
			if (m_commandMap.ContainsKey(cmd))
			{
				throw new Exception("command already defined");
			}

			CommandData data = new CommandData();
			data.m_handler = handler;
			data.m_help = help;
			data.m_longhelp = longhelp;
			m_commandMap[cmd] = data;
		}

        public void RemoveCommand(string cmd)
        {
            m_commandMap.Remove(cmd);
        }
		
		public bool HasCommand(string cmd)
		{
			return m_commandMap.ContainsKey(cmd);
		}

		public string HandleInput(string input)
		{
			if (input.Length == 0 || input[0] != '/')
				return input;

			RunCommand(input);
			return null;
		}

		public int RunCommand(string input)
		{
			string cmd;

			if (input.Length == 0)
				return 0;

			if (input[0] != '/')
			{
				ChiConsole.WriteLine("Input is not a command: {0}", input);
				return -1;
			}

			int idx = input.IndexOf(' ');

			if (idx == -1)
			{
				cmd = input.Substring(1);
				input = "";
			}
			else
			{
				cmd = input.Substring(1, idx-1);
				input = input.Substring(idx + 1);
			}

			if (!m_commandMap.ContainsKey(cmd))
			{
				List<string> l = new List<string>();
				
				foreach(string c in m_commandMap.Keys)
				{
					if(c.StartsWith(cmd))
						l.Add(c);
				}
				
				if(l.Count == 0)
				{
					ChiConsole.WriteLine("Unknown command {0}", cmd);
					return -1;
				}
				else if(l.Count > 1)
				{
					ChiConsole.WriteLine("Ambigious command. ({0})", String.Join(", ", l.ToArray()));
					return -1;
				}
				else
				{
					cmd = l[0];
				}
			}

			CommandData cmdData = m_commandMap[cmd];

			int res = -1;

			try
			{
				res = cmdData.m_handler(input);
			}
			catch (Exception e)
			{
				ChiConsole.WriteError("Error handling command " + cmd, e);
			}

			return res;
		}

		int HelpCmd(string input)
		{
			if (input.Length == 0)
			{
				ChiConsole.WriteLine("Commands");
				ChiConsole.WriteLine("--------");

				string[] keys = new string[m_commandMap.Count];
				m_commandMap.Keys.CopyTo(keys, 0);
				Array.Sort<string>(keys);
				
				foreach (string key in keys)
				{
					if (m_commandMap[key].m_help != null)
						ChiConsole.WriteLine("{0} - {1}", key, m_commandMap[key].m_help);
					else
						ChiConsole.WriteLine("{0}", key);
				}
			}
			else
			{
				if (m_commandMap.ContainsKey(input))
				{
					CommandData data = m_commandMap[input];
					if (data.m_longhelp == null)
					{
						ChiConsole.WriteLine("No help available.");
					}
					else
					{
						ChiConsole.WriteLine("");
						ChiConsole.WriteLine("/{0} - {1}", input, data.m_help);
						ChiConsole.WriteLine("");
						ChiConsole.WriteLine(data.m_longhelp);
					}
				}
				else
				{
					ChiConsole.WriteLine("No such command.");
				}
			}

			return 0;
		}


		static Regex s_regexp1 = new Regex(@"(?<=^|[^\\])""", RegexOptions.Compiled);
		static Regex s_regexp2 = new Regex(@"
(?<=(\s|^)) # starts with white space or line start
((([^\s]*?) ((?<=^|[^\\])"") (.*?) ((?<=$|[^\\])"") ([^\s]*?))  |  ([^\s]*?))
(?=(\s|$))  # ends with white space or line end
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
		static Regex s_regexp3 = new Regex(@"(?<=^|[^\\])""");

		public static void GetOpts(string input, string optstring, out string[] args, out StringDictionary opts)
		{
			args = new string[0];
			opts = new StringDictionary();

			if (input.Length == 0)
			{
				return;
			}

			MatchCollection mc1 = s_regexp1.Matches(input);
			if (mc1.Count % 2 != 0)
			{
				throw new Exception("Mismatched quotation marks.");
			}

			MatchCollection mc = s_regexp2.Matches(input);

			List<string> argsList = new List<string>();

			for (int i = 0; i < mc.Count; i++)
			{
				string arg = mc[i].Value;

				arg = s_regexp3.Replace(arg, "");
				arg = arg.Replace(@"\""", @"""");

				if (optstring != null && arg.Length == 2 && arg[0] == '-')
				{
					int optIdx = optstring.IndexOf(arg[1]);

					if (optIdx == -1)
					{
						throw new Exception(String.Format("Unknown option {0}", arg));
					}

					if (optstring.Length > optIdx + 1 && optstring[optIdx + 1] == ':')
					{
						// Requires parameter
						if (mc.Count < i + 2)
						{
							throw new Exception(String.Format("Option {0} requires a parameter", arg));
						}

						opts[arg[1].ToString()] = mc[i + 1].Value;
						i++;
					}
					else
					{
						opts[arg[1].ToString()] = null;
					}
				}
				else
				{
					argsList.Add(arg);
				}
			}

			args = argsList.ToArray();
		}

	}
}
