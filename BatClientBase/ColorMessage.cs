#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing; 

#endregion

/*
 * TODO: make colormessage editable. ie. msg.Insert("kala", 4), and msg.SetColor(6, Color.Red)
 * TODO: the parsing code sucks. fix.
 * TODO: control codes spanning multiple lines do not work
 */

namespace BatMud.BatClientBase
{
	public class ColorMessage
	{
		public class MetaData
		{
			public int m_index;
			public Color m_fgColor;
			public Color m_bgColor;

			public MetaData(int index, Color textColor, Color backgroundColor)
			{
				m_index = index;
				m_fgColor = textColor;
				m_bgColor = backgroundColor;
			}

			public override string ToString()
			{
				return String.Format("{0}: {1}/{2}", m_index, m_fgColor.ToString(), m_bgColor.ToString());
			}
		}


		const char ESC = '\x1b';

		StringBuilder m_text;
		List<MetaData> m_metaData;

		public ColorMessage(string text)
		{
			m_text = ControlCodes.ParseBatControls(text, out m_metaData);

			Validate();
		}

		public ColorMessage(string text, List<MetaData> metadata)
		{
			m_text = new StringBuilder(text);
			m_metaData = metadata;

			Validate();
		}

		public void SetText(string text)
		{
			m_text = ControlCodes.ParseBatControls(text, out m_metaData);

			Validate();
		}

		public string Text
		{
			get { return m_text.ToString(); }
		}

		public MetaData[] GetMetaDataArray()
		{
			return m_metaData.ToArray();
		}

		public void Remove(int startIndex, int length)
		{
			m_text.Remove(startIndex, length);
			
			int i = 0;
			Color currentFgColor = Color.Empty;
			Color currentBgColor = Color.Empty;

			while (i < m_metaData.Count && m_metaData[i].m_index < startIndex)
			{
				currentFgColor = m_metaData[i].m_fgColor;
				currentBgColor = m_metaData[i].m_bgColor;
				i++;
			}

			while (i < m_metaData.Count && m_metaData[i].m_index < startIndex + length)
			{
				currentFgColor = m_metaData[i].m_fgColor;
				currentBgColor = m_metaData[i].m_bgColor;
				m_metaData.RemoveAt(i);
			}

			if(currentFgColor != Color.Empty || currentBgColor != Color.Empty)
				m_metaData.Insert(i++, new MetaData(startIndex, currentFgColor, currentBgColor));

			while (i < m_metaData.Count)
			{
				m_metaData[i].m_index -= length;
				i++;
			}

			Validate();
		}

		public void Insert(int index, string text)
		{
			m_text.Insert(index, text);

			int i = 0;

			while (i < m_metaData.Count && m_metaData[i].m_index < index)
				i++;

			while (i < m_metaData.Count)
			{
				m_metaData[i].m_index += text.Length;
				i++;
			}

			Validate();
		}

		public void Insert(int index, ColorMessage msg)
		{
			m_text.Insert(index, msg.m_text);

			int i = 0;

			while (i < m_metaData.Count && m_metaData[i].m_index < index)
				i++;

			m_metaData.InsertRange(i, msg.m_metaData);

			int t = 0;
			while (t < msg.m_metaData.Count)
			{
				m_metaData[i + t].m_index += index;
				t++;
			}

			i += t;

			while (i < m_metaData.Count)
			{
				m_metaData[i].m_index += msg.m_text.Length;
				i++;
			}

			Validate();
		}

		public void Colorize(int index, int length, Color fgColor, Color bgColor)
		{
			int i = 0;

			Color currentFgColor = Color.Empty;
			Color currentBgColor = Color.Empty;

			while (i < m_metaData.Count && m_metaData[i].m_index < index)
			{
				currentFgColor = m_metaData[i].m_fgColor;
				currentBgColor = m_metaData[i].m_bgColor;
				i++;
			}

			if (fgColor == Color.Empty)
				fgColor = currentFgColor;

			if (bgColor == Color.Empty)
				bgColor = currentBgColor;

			m_metaData.Insert(i++, new MetaData(index, fgColor, bgColor));
			m_metaData.Insert(i++, new MetaData(index + length, currentFgColor, currentBgColor));

			// remove the nodes inside the colored area
			while (i < m_metaData.Count && m_metaData[i].m_index < index + length)
			{
				m_metaData.RemoveAt(i);
			}

			Validate();
		}

		void Validate()
		{
			int idx = -1;

			foreach (MetaData md in m_metaData)
			{
				if (md.m_index > m_text.Length)
					throw new Exception("ColorMessage corrupted");

				if(md.m_index < idx)
					throw new Exception("ColorMessage corrupted");

				idx = md.m_index;
			}
		}

		public override string ToString()
		{
			return m_text.ToString();
		}


		public string ToAnsiString()
		{
			StringBuilder sb = new StringBuilder(m_text.ToString());

			for (int i = m_metaData.Count - 1; i >= 0; i--)
			{
				MetaData md = m_metaData[i];

				// ESC[31;41m
				int fg;
				int bg;
				bool bold = false;
				
				if(md.m_fgColor.IsEmpty)
					fg = 30 + (int)Ansi.DefaultFgColor;
				else
					fg = 30 + (int)Ansi.ColorToAnsiColor(md.m_fgColor, out bold);

				if(md.m_bgColor.IsEmpty)
					bg = 40 + (int)Ansi.DefaultBgColor;
				else
				{
					bool dummy;
					bg = 40 + (int)Ansi.ColorToAnsiColor(md.m_bgColor, out dummy);
				}

				//Console.WriteLine("\nFG {0} -> {1}, {2}", md.m_fgColor, fg, bold);
				//Console.WriteLine("\nBG {0} -> {1}", md.m_bgColor, bg);

				string ctrl;
				if(bold)
					ctrl = String.Format("{0}[1;{1};{2}m", ESC, fg, bg);
				else
					ctrl = String.Format("{0}[0;{1};{2}m", ESC, fg, bg);

				sb.Insert(md.m_index, ctrl);
			}

			//sb.Append(String.Format("{0}[0m", ESC));
			return sb.ToString();
		}

	}

}
