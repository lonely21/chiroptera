using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using BatMud.BatClientBase;

namespace BatMud.BatClientWindows
{
	// non-editable paragraph
	public class Paragraph
	{
		public string m_text;
		public MetaData[] m_meta;
		public int m_lines; // how many lines this paragraphs takes with current number of columns
		
		static Regex s_linkRegexp = new Regex(@"((ftp|http|https|mailto|news|nntp|telnet|file)://" +
			@"(([A-Za-z0-9$_.+!*(),;/?:@&~=-])|%[A-Fa-f0-9]{2}){2,}(#([a-zA-Z0-9][a-zA-Z0-9$_.+!*(),;/?:@&~=%-]*))?([A-Za-z0-9$_+!*();/?:~-]))",
			 RegexOptions.Compiled);

		public class MetaData
		{
			public int m_index;
			public System.Drawing.Color m_fgColor;
			public System.Drawing.Color m_bgColor;
			public bool m_isLink = false;

			public MetaData(ColorMessage.MetaData cmMeta)
			{
				m_index = cmMeta.m_index;
				m_fgColor = cmMeta.m_style.Fg.ToSystemColor();
				if (m_fgColor.IsEmpty)
					m_fgColor = System.Drawing.Color.FromArgb(160, 160, 160);
				m_bgColor = cmMeta.m_style.Bg.ToSystemColor();
				if (m_bgColor.IsEmpty)
					m_bgColor = System.Drawing.Color.Black;

				if ((cmMeta.m_style.Flags & TextStyleFlags.HighIntensity) != 0)
				{
					bool bold;
					int c;

					c = Ansi.ColorToAnsiColor8(BatClientBase.Color.FromSystemColor(m_fgColor), out bold);
					if (bold == false)
						m_fgColor = Ansi.AnsiColor8ToColor(c, true).ToSystemColor();

					if (!cmMeta.m_style.Bg.IsEmpty)
					{
						c = Ansi.ColorToAnsiColor8(BatClientBase.Color.FromSystemColor(m_bgColor), out bold);
						if (bold == false)
							m_bgColor = Ansi.AnsiColor8ToColor(c, true).ToSystemColor();
					}
				}

				if ((cmMeta.m_style.Flags & TextStyleFlags.Inverse) != 0)
				{
					System.Drawing.Color tmp = m_fgColor;
					m_fgColor = m_bgColor;
					m_bgColor = tmp;
				}
			}

			public MetaData(int index, System.Drawing.Color textColor, System.Drawing.Color backgroundColor)
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

		public Paragraph()
		{
		}

		public Paragraph(ColorMessage msg)
		{
			m_text = msg.Text;
			m_meta = new MetaData[msg.GetMetaDataArray().Length];
			ColorMessage.MetaData[] marray = msg.GetMetaDataArray();
			for (int i = 0; i < marray.Length; i++)
				m_meta[i] = new MetaData(marray[i]);

			// look for links. this is not really the best place for this...
			// TODO: breaks if there are color changes in the link

			MatchCollection matches = s_linkRegexp.Matches(m_text);

			if (matches.Count > 0)
			{
				List<MetaData> metaDataList = new List<MetaData>(m_meta);

				foreach (Match match in matches)
				{
					System.Drawing.Color currentTextColor = System.Drawing.Color.FromArgb(160, 160, 160);
					System.Drawing.Color currentBackgroundColor = System.Drawing.Color.Black;

					int matchIdx = match.Index;
					int matchLen = match.Length;

					int i = 0;

					while (i < metaDataList.Count && metaDataList[i].m_index <= matchIdx)
					{
						if (metaDataList[i].m_fgColor != System.Drawing.Color.Empty)
						{
							currentTextColor = metaDataList[i].m_fgColor;
						}

						if (metaDataList[i].m_bgColor != System.Drawing.Color.Empty)
						{
							currentBackgroundColor = metaDataList[i].m_bgColor;
						}

						i++;
					}

					MetaData md = new MetaData(matchIdx, currentTextColor, currentBackgroundColor);
					md.m_isLink = true;

					metaDataList.Insert(i, md);

					i++;

					metaDataList.Insert(i, new MetaData(matchIdx + matchLen, currentTextColor, currentBackgroundColor));

					i++;
				}

				m_meta = metaDataList.ToArray();
			}
		}

		public override string ToString()
		{
			System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
			for (int i = 0; i < m_meta.Length; i++)
			{
				strBuilder.Append(m_meta[i].ToString());
				strBuilder.Append(", ");
			}
			return strBuilder.ToString();
		}

		public string ToDebugString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(m_text.ToString());

			int i = m_meta.Length - 1;

			for (; i >= 0; i--)
			{
				string s = String.Format("<{0}/{1}>", m_meta[i].m_fgColor, m_meta[i].m_bgColor);
				sb.Insert(m_meta[i].m_index, s);
			}

			return sb.ToString();
		}

	}
}
