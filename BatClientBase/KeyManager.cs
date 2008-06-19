#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

#endregion

namespace BatMud.BatClientBase
{
	public delegate bool KeyBindingAction(object userdata);

	[XmlInclude(typeof(ScriptedKeyBinding))]
	public class KeyBinding
	{
		public delegate bool KeyBound(KeyBinding binding, Keys newKey, Keys oldKey);
		public event KeyBound keyBound;

		Keys m_key;
		KeyBindingAction m_action;
		object m_userData;

		public KeyBinding()
		{
			m_key = Keys.None;
		}

		public KeyBinding(Keys key, KeyBindingAction action, object userData)
		{
			this.Key = key;
			this.Action = action;
			this.UserData = userData;
		}

		public virtual bool Run()
		{
			if (m_action != null)
				return m_action(m_userData);
			else
				return false;
		}

		public Keys Key
		{
			get { return m_key; }
			set
			{
				if (value == m_key)
					return;

				if (keyBound != null)
				{
					if (keyBound(this, value, m_key) == false)
						throw new Exception("Key already bound");
				}
				m_key = value;
			}
		}

		[XmlIgnore()]
		public KeyBindingAction Action
		{
			get { return m_action; }
			set { m_action = value; }
		}

		[XmlIgnore()]
		public object UserData
		{
			get { return m_userData; }
			set { m_userData = value; }
		}

		public virtual bool Save
		{
			get { return false; }
			set { }
		}
	}

	public enum KeyBindingType
	{
		Send,
		Script
	}

	public class ScriptedKeyBinding : KeyBinding
	{
		KeyBindingType m_type;
		string m_text;
		KeyManager.KeySubst m_subst;
		bool m_save;

		public ScriptedKeyBinding()
		{
		}

		public ScriptedKeyBinding(Keys key, KeyBindingType type, string text, bool save)
		{
			base.Key = key;
			BindingType = type;
			m_text = text;
			m_save = save;
		}

		public override bool Run()
		{
			Compile();
			return base.Run();
		}

		public KeyBindingType BindingType
		{
			get { return m_type; }
			set
			{
				m_type = value;
				if (m_type == KeyBindingType.Send)
					base.Action = SendAction;
				m_subst = null;
				base.Action = null;
			}
		}

		public string Text
		{
			get { return m_text; }
			set
			{
				m_text = value;
				m_subst = null;
				base.Action = null;
			}
		}

		public override bool Save
		{
			get { return m_save; }
			set { m_save = value; }
		}

		public void Compile()
		{
			if (m_type == KeyBindingType.Send)
			{
				m_subst = new KeyManager.KeySubst(m_text);
				base.Action = SendAction;
			}
			else if (m_type == KeyBindingType.Script)
			{
				base.Action = PythonInterface.PythonEngine.CreateMethod<KeyBindingAction>(m_text);
			}
		}

		bool SendAction(object userdata)
		{
			string text = m_subst.Replace();
			PythonInterface.Network.SendLine(text);
			return false;
		}
	}

	public class KeyManager
	{
		List<KeyBinding> m_keyBindingList = new List<KeyBinding>();
		Dictionary<Keys, KeyBinding> m_keyBindingMap = new Dictionary<Keys, KeyBinding>();

		public KeyManager(BaseServicesDispatcher dispatcher)
		{
			dispatcher.RegisterKeyDownHandler(HandleKey);
		}

		public void AddBinding(KeyBinding binding)
		{
			if (binding.Key != Keys.None && m_keyBindingMap.ContainsKey(binding.Key))
			{
				throw new Exception("Key already bound");
			}

			m_keyBindingList.Add(binding);
			if (binding.Key != Keys.None)
				m_keyBindingMap[binding.Key] = binding;
			binding.keyBound += KeyBoundCallback;
		}

		public void RemoveBinding(KeyBinding binding)
		{
			m_keyBindingList.Remove(binding);
			if (binding.Key != Keys.None)
				m_keyBindingMap.Remove(binding.Key);
			binding.keyBound -= KeyBoundCallback;
		}

		public void RemoveBinding(Keys key)
		{
			if (IsKeyBound(key))
			{
				RemoveBinding(this[key]);
			}
		}

		bool KeyBoundCallback(KeyBinding binding, Keys newKey, Keys oldKey)
		{
			// Note: binding.Key is still the oldKey here
			if (newKey != Keys.None && m_keyBindingMap.ContainsKey(newKey))
				return false;

			m_keyBindingMap.Remove(oldKey);
			if (newKey != Keys.None)
				m_keyBindingMap[newKey] = binding;

			return true;
		}

		public bool IsKeyBound(Keys key)
		{
			return m_keyBindingMap.ContainsKey(key);
		}

		public KeyBinding this[Keys key]
		{
			get
			{
				return m_keyBindingMap[key];
			}
		}

		public KeyBinding[] GetKeyBindings()
		{
			return m_keyBindingList.ToArray();
		}

		public KeyBindingCollection GetSavedKeyBindings()
		{
			KeyBindingCollection collection = new KeyBindingCollection();
			foreach (KeyBinding binding in m_keyBindingList)
			{
				if (binding.Save)
					collection.Add(binding);
			}
			return collection;
		}

		public void SetKeyBindings(KeyBindingCollection bindings)
		{
			m_keyBindingList.Clear();
			m_keyBindingMap.Clear();

			if (bindings == null)
				return;

			foreach (KeyBinding binding in bindings)
				AddBinding(binding);
		}

		bool HandleKey(Keys key)
		{
			bool fallthrough = true;

			if (m_keyBindingMap.ContainsKey(key))
			{
				KeyBinding binding = m_keyBindingMap[key];

				try
				{
					fallthrough = binding.Run();
				}
				catch (Exception e)
				{
					BatConsole.WriteError("Error calling key handler for key " + key.ToString(), e);
					fallthrough = false;
				}
			}

			return !fallthrough;
		}

		public class KeySubst
		{
			delegate object KeyEval();

			List<object> m_list;
			string m_template;

			public KeySubst(string pattern)
			{
				m_list = new List<object>();
				m_template = Regex.Replace(pattern,
					@"(?<exp>%{([^}]*)})|(?<fun>%%{([^}]*)})", ParseEvaluator, RegexOptions.ExplicitCapture);
			}

			string ParseEvaluator(Match m)
			{
				string str;

				str = m.Groups["exp"].Value;
				if (str.Length > 0)
				{
					string code = str.Substring(2, str.Length - 3);
					KeyEval e = PythonInterface.PythonEngine.CreateLambda<KeyEval>(code);
					m_list.Add(e);
				}

				str = m.Groups["fun"].Value;
				if (str.Length > 0)
				{
					string code = str.Substring(3, str.Length - 4);
					KeyEval e = PythonInterface.PythonEngine.CreateMethod<KeyEval>(code);
					m_list.Add(e);
				}

				int num = m_list.Count - 1;
				return "{" + num.ToString() + "}";
			}

			public string Replace()
			{
				string[] args = new string[m_list.Count];

				for (int i = 0; i < m_list.Count; i++)
				{
					object arg = m_list[i];
					if (arg is KeyEval)
					{
						object o = ((KeyEval)arg)();
						if (o == null)
							args[i] = "";
						else
							args[i] = o.ToString();
					}
					else
						throw new Exception("unknown item in substitution list");
				}

				return String.Format(m_template, args);
			}
		}
	}
}
