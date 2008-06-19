using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace BatMud.BatClientBase
{
	public class Hilite : Trigger
	{
		Color m_fgColor;
		Color m_bgColor;
		bool m_hiliteLine;

		public Hilite() : base("", null, null, null, null, 0, false, false, false)
		{
			base.Action = HiliteCallback;
		}

		public Hilite(string pattern, bool ignoreCase, Color fgColor, Color bgColor, bool hiliteLine)
			: base(pattern, null, null, null, null, 1, true, false, ignoreCase)
		{
			base.Action = HiliteCallback;

			m_fgColor = fgColor;
			m_bgColor = bgColor;
			m_hiliteLine = hiliteLine;
		}

		[XmlIgnore()]
		public Color FgColor
		{
			set { m_fgColor = value; }
			get { return m_fgColor; }
		}

		[XmlElement("FgColor")]
		public string XmlFgColor
		{
			get { return System.Drawing.ColorTranslator.ToHtml(m_fgColor); }
			set { m_fgColor = System.Drawing.ColorTranslator.FromHtml(value); }
		}

		[XmlIgnore()]
		public Color BgColor
		{
			set { m_bgColor = value; }
			get { return m_bgColor; }
		}

		[XmlElement("BgColor")]
		public string XmlBgColor
		{
			get { return System.Drawing.ColorTranslator.ToHtml(m_bgColor); }
			set { m_bgColor = System.Drawing.ColorTranslator.FromHtml(value); }
		}

		public bool HiliteLine
		{
			set { m_hiliteLine = value; }
			get { return m_hiliteLine; }
		}

		bool HiliteCallback(ColorMessage msg, Match match, object userdata)
		{
			if (m_hiliteLine)
			{
				msg.SetText(ControlCodes.ColorizeString(msg.Text, m_fgColor, m_bgColor));
			}
			else
			{
				while (match.Success)
				{
					msg.Colorize(match.Index, match.Length, m_fgColor, m_bgColor);
					match = match.NextMatch();
				}
			}

			return false;
		}
	}

	public class HiliteManager
	{

		TriggerManager m_triggerManager;

		public HiliteManager(TriggerManager triggerManager)
		{
			m_triggerManager = triggerManager;
		}

		public void AddHilite(Hilite hilite)
		{
			m_triggerManager.AddTrigger(hilite);
		}

		public void RemoveHilite(Hilite hilite)
		{
			m_triggerManager.RemoveTrigger(hilite);
		}

		public Hilite[] GetHilites()
		{
			List<Hilite> hilites = new List<Hilite>();
			Trigger[] triggers = m_triggerManager.Triggers;

			foreach (Trigger trigger in triggers)
			{
				if (!(trigger is Hilite))
					continue;

				hilites.Add((Hilite)trigger);
			}

			return hilites.ToArray();
		}


	}
}
