using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace BatMud.BatClientBase
{
	public delegate bool TriggerAction(ColorMessage msg, Match match, object userdata);

	public enum TriggerType
	{
		Send,
		Replace,
		Script
	}

	public class ScriptedTrigger : Trigger
	{
		TriggerType m_triggerType;
		string m_script;
		TriggerManager.TriggerSubst m_subst;
		TriggerAction m_scriptAction;

		public ScriptedTrigger()
		{
			this.Script = "";
			this.TriggerType = TriggerType.Send;
		}

		public ScriptedTrigger(string pattern, bool ignoreCase, TriggerType triggerType, string script) 
			: base(pattern, null, null, null, null, 0, true, false, ignoreCase)
		{
			this.TriggerType = triggerType;
			if (script != null)
				this.Script = script;
			else
				this.Script = "";
		}

		public void Compile()
		{
			if (m_triggerType == TriggerType.Script)
			{
				string parsedScript = TriggerManager.TriggerSubst.ReplaceScript(m_script);
				m_scriptAction = PythonInterface.PythonEngine.CreateMethod<TriggerAction>(parsedScript);
			}
			else
			{
				m_subst = new TriggerManager.TriggerSubst(m_script);
			}
		}

		public TriggerType TriggerType
		{
			set
			{
				m_triggerType = value;
				if (m_triggerType == TriggerType.Send)
					base.Action = SendCallback;
				else if (m_triggerType == TriggerType.Replace)
					base.Action = ReplaceCallback;
				else if (m_triggerType == TriggerType.Script)
					base.Action = ScriptCallback;

				m_subst = null;
				m_scriptAction = null;
			}

			get { return m_triggerType; }
		}

		public string Script
		{
			set
			{
				m_script = value;
				m_subst = null;
				m_scriptAction = null;
			}

			get { return m_script; }
		}

		bool SendCallback(ColorMessage msg, Match match, object userdata)
		{
			if (m_subst == null)
				m_subst = new TriggerManager.TriggerSubst(m_script);

			string str = m_subst.Replace(msg.Text, match);
			PythonInterface.Network.SendLine(str);
			return false;
		}

		bool ReplaceCallback(ColorMessage msg, Match match, object userdata)
		{
			if (m_subst == null)
				m_subst = new TriggerManager.TriggerSubst(m_script);

			List<object[]> inserts = new List<object[]>();

			while(match.Success)
			{
				string str = m_subst.Replace(msg.Text, match);
				inserts.Add(new object[] { match.Index, match.Length, str });
				match = match.NextMatch();
			}

			inserts.Reverse();

			foreach(object[] insert in inserts)
			{
				int idx = (int)insert[0];
				int len = (int)insert[1];
				string str = (string)insert[2];
				msg.Remove(idx, len);
				msg.Insert(idx, str);
			}

			return false;
		}

		bool ScriptCallback(ColorMessage msg, Match match, object userdata)
		{
			if (m_scriptAction == null)
			{
				string parsedScript = TriggerManager.TriggerSubst.ReplaceScript(m_script);
				m_scriptAction = PythonInterface.PythonEngine.CreateMethod<TriggerAction>(parsedScript);
			}

			return m_scriptAction(msg, match, userdata);
		}
	}

	[XmlInclude(typeof(ScriptedTrigger))]
	[XmlInclude(typeof(Hilite))]
	public class Trigger
	{
		static int s_triggerCount = 0;

		int m_triggerID;
		Regex m_regex;
		TriggerAction m_action;
		object m_userdata;
		string m_triggerGroup;
		string m_triggerName;
		bool m_fallThrough;
		bool m_gag;
		bool m_ignoreCase;
		int m_priority;

		public Trigger()
		{
			this.Pattern = "";

			m_triggerID = s_triggerCount++;
		}

		public Trigger(string regexp, TriggerAction action, object userdata,
			string triggerName, string triggerGroup, int priority, bool fallThrough, bool gag, bool ignoreCase)
		{
			if (regexp != null)
				this.Pattern = regexp;
			else
				this.Pattern = "";
			m_action = action;
			m_userdata = userdata;
			m_triggerName = triggerName;
			m_triggerGroup = triggerGroup;
			m_priority = priority;				 
			m_fallThrough = fallThrough;
			m_gag = gag;
			m_ignoreCase = ignoreCase;

			m_triggerID = s_triggerCount++;
		}

		public int TriggerID
		{
			get { return m_triggerID; }
		}

		[XmlIgnore()]
		public Regex Regex
		{
			set { m_regex = value; }
			get { return m_regex; }
		}

		public string Pattern
		{
			set
			{
				if (value.Length == 0)
				{
					m_regex = null;
					return;
				}

				RegexOptions opts = RegexOptions.Compiled;
				if (m_ignoreCase)
					opts |= RegexOptions.IgnoreCase;

				m_regex = new Regex(value, opts);
			}

			get 
			{
				if (m_regex != null)
					return m_regex.ToString();
				else
					return "";
			}
		}

		public bool IgnoreCase
		{
			get { return m_ignoreCase; }
			set
			{
				m_ignoreCase = value; 
				this.Pattern = this.Pattern; // To recreate the regexp
			}
		}

		public string TriggerGroup
		{
			set { m_triggerGroup = value; }
			get { return m_triggerGroup; }
		}

		public string TriggerName
		{
			set { m_triggerName = value; }
			get { return m_triggerName; }
		}

		[XmlIgnore()]
		public TriggerAction Action
		{
			set { m_action = value; }
			get { return m_action; }
		}

		[XmlIgnore()]
		public object UserData
		{
			set { m_userdata = value; }
			get { return m_userdata; }
		}

		public bool IsFallThrough
		{
			set { m_fallThrough = value; }
			get { return m_fallThrough; }
		}

		public bool IsGag
		{
			set { m_gag = value; }
			get { return m_gag; }
		}

		public int Priority
		{
			set { m_priority = value; }
			get { return m_priority; }
		}
	}

	public class TriggerManager
	{
		List<Trigger> m_triggerList = new List<Trigger>();

		public TriggerManager(BaseServicesDispatcher dispatcher)
		{
			dispatcher.RegisterMessageHandler(HandleColorMessage);
		}

		public void AddTrigger(Trigger trigger)
		{
			m_triggerList.Remove(trigger);

			if (trigger.TriggerName != null)
			{
				Trigger t = GetTrigger(trigger.TriggerName);
				if (t != null)
					m_triggerList.Remove(t);
			}

			int i = 0;
			while (i < m_triggerList.Count && m_triggerList[i].Priority >= trigger.Priority)
				i++;

			m_triggerList.Insert(i, trigger);
		}

		public bool RemoveTrigger(Trigger trigger)
		{
			return m_triggerList.Remove(trigger);
		}

		public void RemoveTriggerGroup(string groupName)
		{
			m_triggerList.RemoveAll(delegate(Trigger t) { return t.TriggerGroup == groupName; });
		}

		public Trigger GetTrigger(int triggerID)
		{
			return m_triggerList.Find(delegate(Trigger t) { return t.TriggerID == triggerID; });
		}

		public Trigger GetTrigger(string triggerName)
		{
			return m_triggerList.Find(delegate(Trigger t) { return t.TriggerName == triggerName; });
		}

		public Trigger[] Triggers
		{
			get { return m_triggerList.ToArray(); }
		}

		public TriggerCollection GetSavedTriggers()
		{
			TriggerCollection collection = new TriggerCollection();
			foreach (Trigger trigger in m_triggerList)
			{
				// XXX if(save)
				collection.Add(trigger);
			}
			return collection;
		}

		public void SetTriggers(TriggerCollection triggers)
		{
			m_triggerList.Clear();

			if (triggers == null)
				return;

			foreach (Trigger trigger in triggers)
				AddTrigger(trigger);
		}

		public ColorMessage HandleColorMessage(ColorMessage colorMessage)
		{
			foreach (Trigger trigger in m_triggerList)
			{
				if (trigger.Regex == null)
					continue;

				Match match = trigger.Regex.Match(colorMessage.Text);

				if (match.Success == false)
					continue;

				bool gag = false;

				try
				{
					gag = trigger.Action(colorMessage, match, trigger.UserData);
				}
				catch (Exception e)
				{
					BatConsole.WriteError("Failed to run trigger action for trigger ID " + trigger.TriggerID.ToString(), e);
				}

				if(trigger.IsGag || gag)
				{
					colorMessage = null;
					break;
				}

				if (trigger.IsFallThrough == false)
					break;
			}

			return colorMessage;
		}

		public class TriggerSubst
		{
			delegate object TriggerEval(Match match);

			List<object> m_list;
			string m_template;

			public TriggerSubst(string pattern)
			{
				m_list = new List<object>();
				m_template = Regex.Replace(pattern,
					@"(?<exp>%{([^}]*)})|(?<fun>%%{([^}]*)})|(?<subst>%[\dLR])", ParseEvaluator, RegexOptions.ExplicitCapture);
			}

			public static string ReplaceScript(string code)
			{
				code = Regex.Replace(code, @"%(\d)", "match.Groups[$1].Value");
				code = Regex.Replace(code, @"%L", "msg.Text[:match.Groups[0].Index]");
				code = Regex.Replace(code, @"%R", "msg.Text[match.Groups[0].Index + match.Groups[0].Length:]");
				return code;
			}

			string ParseEvaluator(Match m)
			{
				string str;

				str = m.Groups["exp"].Value;
				if (str.Length > 0)
				{
					string code = str.Substring(2, str.Length - 3);
					code = ReplaceScript(code);
					TriggerEval e = PythonInterface.PythonEngine.CreateLambda<TriggerEval>(code);
					m_list.Add(e);
				}

				str = m.Groups["fun"].Value;
				if (str.Length > 0)
				{
					string code = str.Substring(3, str.Length - 4);
					code = ReplaceScript(code);
					TriggerEval e = PythonInterface.PythonEngine.CreateMethod<TriggerEval>(code);
					m_list.Add(e);
				}

				str = m.Groups["subst"].Value;
				if (str.Length > 0)
				{
					if (str[1] == 'L' || str[1] == 'R')
						m_list.Add(str[1]);
					else
						m_list.Add(Int32.Parse(str.Substring(1)));
				}

				int num = m_list.Count - 1;
				return "{" + num.ToString() + "}";
			}

			public string Replace(string text, Match m)
			{
				string[] args = new string[m_list.Count];

				for (int i = 0; i < m_list.Count; i++)
				{
					object arg = m_list[i];
					if (arg is int)
					{
						args[i] = m.Groups[(int)arg].Value;
					}
					else if (arg is TriggerEval)
					{
						object o = ((TriggerEval)arg)(m);
						if (o == null)
							args[i] = "";
						else
							args[i] = o.ToString();
					}
					else if (arg is char)
					{
						char c = (char)arg;
						if (c == 'L')
							args[i] = text.Substring(0, m.Index);
						else if (c == 'R')
							args[i] = text.Substring(m.Index + m.Length);
						else
							throw new Exception("unknown %<char> substitution");
					}
					else
						throw new Exception("unknown item in substitution list");
				}

				return String.Format(m_template, args);
			}
		}
	}
}
