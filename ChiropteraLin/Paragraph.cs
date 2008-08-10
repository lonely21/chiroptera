using System;
using System.Text;
using System.Collections.Generic;
using Chiroptera.Base;

namespace Chiroptera.Lin
{
	// non-editable paragraph
	public class Paragraph
	{
		public string m_text;
		public ColorMessage.MetaData[] m_meta;
		public int m_lines; // how many lines this paragraphs takes with current number of columns
		
		public Paragraph()
		{
		}

		public Paragraph(ColorMessage msg)
		{
			m_text = msg.Text;
			m_meta = msg.GetMetaDataArray();
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
				string s = String.Format("<{0}>", m_meta[i]);
				sb.Insert(m_meta[i].m_index, s);
			}

			return sb.ToString();
		}

		string StyleToAnsi(TextStyle style, bool use256)
		{
			StringBuilder esb = new StringBuilder();
			
			esb.Append("\x1b[0m");
			
			if(style.IsHighIntensity)
				esb.Append("\x1b[1m");

			if(style.IsReverse)
				esb.Append("\x1b[7m");
			
			if(!style.Fg.IsEmpty)
			{
				if(use256)
					esb.Append(Ansi.ColorToAnsiString256(style.Fg, false, style.IsHighIntensity));
				else
					esb.Append(Ansi.ColorToAnsiString8(style.Fg, false));
			}
			
			if(!style.Bg.IsEmpty)
			{
				if(use256)
					esb.Append(Ansi.ColorToAnsiString256(style.Bg, true, false));
				else
					esb.Append(Ansi.ColorToAnsiString8(style.Bg, true));
			}
			
			return esb.ToString();
		}

		public string ToAnsiString(bool use256)
		{
			return ToAnsiString(use256, 0, m_text.Length);
		}
		
		public string ToAnsiString(bool use256, int startIndex, int length)
		{
			StringBuilder sb = new StringBuilder();
			
			int i = 0;
			TextStyle currentStyle = null;
			int currentIndex = startIndex;

			while (i < m_meta.Length && m_meta[i].m_index < startIndex)
			{
				currentStyle = m_meta[i].m_style;
				i++;
			}
			
			while (i < m_meta.Length && m_meta[i].m_index <= startIndex + length)
			{
				if(currentStyle != null)
					sb.Append(StyleToAnsi(currentStyle, use256));

				sb.Append(m_text, currentIndex, m_meta[i].m_index - currentIndex);
				
				currentStyle = m_meta[i].m_style;
				currentIndex = m_meta[i].m_index;
				i++;
			}
			
			if(currentStyle != null)
				sb.Append(StyleToAnsi(currentStyle, use256));
			
			sb.Append(m_text, currentIndex, length - (currentIndex - startIndex));
			
			if(currentStyle != null)
				sb.Append("\x1b[0m");

			return sb.ToString();
		}
		
	}
}
