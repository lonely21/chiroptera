using System;
using System.Text;
using System.Collections.Generic;
using BatMud.BatClientBase;

namespace BatMud.BatClientText
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

		public string ToAnsiString(bool use256)
		{
			const char ESC = '\x1b';
			StringBuilder sb = new StringBuilder(m_text.ToString());

			for (int i = m_meta.Length - 1; i >= 0; i--)
			{
				ColorMessage.MetaData md = m_meta[i];
				TextStyle style = md.m_style;
				
				StringBuilder esb = new StringBuilder();
				
				if(use256)
				{
					esb.Append(ESC);
					esb.Append('[');
					if(style.IsHighIntensity)
						esb.Append('1');
					else
						esb.Append("22");
					esb.Append('m');

					esb.Append(ESC);
					esb.Append('[');
					if(style.IsReverse)
						esb.Append('7');
					else
						esb.Append("27");
					esb.Append('m');
					
					if(!style.Fg.IsEmpty)
					{
						if(style.Fg.IsDefault)
						{
							esb.Append(ESC);
							esb.Append("[39m");
						}
						else
						{
							esb.Append(ESC);
							esb.Append("[38;5;");
							esb.Append(Ansi.ColorToAnsiColor256(style.Fg));
							esb.Append('m');							
						}
					}
					
					if(!style.Bg.IsEmpty)
					{
						if(style.Bg.IsDefault)
						{
							esb.Append(ESC);
							esb.Append("[49m");
						}
						else
						{
							esb.Append(ESC);
							esb.Append("[48;5;");
							esb.Append(Ansi.ColorToAnsiColor256(style.Bg));
							esb.Append('m');							
						}
					}
				}
				else
				{
					// ESC[x;ym
					int fg;
					int bg;
					bool bold = false;

					if(style.Fg.IsDefault)
						fg = 39; // reset fg
					else if(style.Fg.IsEmpty)
						fg = -1;
					else
						fg = 30 + Ansi.ColorToAnsiColor8(style.Fg, out bold);

					if(style.Bg.IsDefault)
						bg = 49; // reset bg
					else if(style.Bg.IsEmpty)
						bg = -1;
					else
					{
						bool dummy;
						bg = 40 + Ansi.ColorToAnsiColor8(style.Bg, out dummy);
					}

					//Console.WriteLine("\nFG {0} -> {1}, {2}", md.m_fgColor, fg, bold);
					//Console.WriteLine("\nBG {0} -> {1}", md.m_bgColor, bg);
					esb.Append(ESC);
					esb.Append('[');

					if(bold || style.IsHighIntensity)
						esb.Append('1');
					else
						esb.Append("22");
					
					esb.Append(';');
					if(style.IsReverse)
						esb.Append("7");
					else
						esb.Append("27");
					
					if(fg != -1)
					{
						esb.Append(';');
						esb.Append(fg.ToString());
					}
					
					if(bg != -1)
					{
						esb.Append(';');
						esb.Append(bg.ToString());
					}
					
					esb.Append('m');
				}
				
				sb.Insert(md.m_index, esb.ToString());
			}

			if(m_meta.Length > 0)
				sb.Append(String.Format("{0}[0m", ESC));
			
			return sb.ToString();
		}
		
	}
}
