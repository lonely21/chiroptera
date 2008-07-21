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
			public TextStyle m_style;

			public MetaData(int index, TextStyle style)
			{
				m_index = index;
				m_style = style;
			}
			
			public MetaData(int index, Color fg, Color bg) 
				: this(index, new TextStyle(fg, bg))
			{
			}

			public override string ToString()
			{
				return String.Format("{0}: {1}", m_index, m_style);
			}
		}


		const char ESC = '\x1b';

		StringBuilder m_text;
		List<MetaData> m_metaData;

		public ColorMessage(string text, List<MetaData> metadata)
		{
			m_text = new StringBuilder(text);
			m_metaData = metadata;

			Validate();
		}
		
		public static ColorMessage CreateFromAnsi(string str, TextStyle lastStyle)
		{
			ColorMessage colorMsg = Ansi.ParseAnsi(str, ref lastStyle);
			return colorMsg;
		}

		public void SetText(string text)
		{
			m_text = new StringBuilder(text);
			m_metaData = new List<MetaData>();
			//m_text = ControlCodes.ParseBatControls(text, out m_metaData);

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
			TextStyle currentStyle = null;

			while (i < m_metaData.Count && m_metaData[i].m_index < startIndex)
			{
				currentStyle = m_metaData[i].m_style;
				i++;
			}

			while (i < m_metaData.Count && m_metaData[i].m_index < startIndex + length)
			{
				currentStyle = m_metaData[i].m_style;
				m_metaData.RemoveAt(i);
			}

			if(currentStyle != null)
				m_metaData.Insert(i++, new MetaData(startIndex, currentStyle));

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

			TextStyle currentStyle = null;

			while (i < m_metaData.Count && m_metaData[i].m_index < index)
			{
				currentStyle = m_metaData[i].m_style;
				i++;
			}

			if (fgColor.IsEmpty && currentStyle != null)
				fgColor = currentStyle.Fg;

			if (bgColor.IsEmpty && currentStyle != null)
				bgColor = currentStyle.Bg;

			// xxx broken, I think
			m_metaData.Insert(i++, new MetaData(index, fgColor, bgColor));
			if(currentStyle != null)
				m_metaData.Insert(i++, new MetaData(index + length, currentStyle));
			else
				m_metaData.Insert(i++, new MetaData(index + length, TextStyle.CreateDefaultStyle()));

			// remove the nodes inside the colored area
			while (i < m_metaData.Count && m_metaData[i].m_index < index + length)
			{
				m_metaData.RemoveAt(i);
			}

			Validate();
		}
		
		public void Colorize(int index, int length, TextStyle style)
		{
			int i = 0;

			TextStyle currentStyle = null;

			while (i < m_metaData.Count && m_metaData[i].m_index < index)
			{
				currentStyle = m_metaData[i].m_style;
				i++;
			}

			if(currentStyle != null)
				style = style.Combine(currentStyle);

			// xxx broken, I think
			m_metaData.Insert(i++, new MetaData(index, style));
			if(currentStyle != null)
				m_metaData.Insert(i++, new MetaData(index + length, currentStyle));
			else
				m_metaData.Insert(i++, new MetaData(index + length, TextStyle.CreateDefaultStyle()));

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
		
		public int Length
		{
			get { return m_text.Length; }
		}

		public string ToAnsiString(bool use256)
		{
			StringBuilder sb = new StringBuilder(m_text.ToString());

			for (int i = m_metaData.Count - 1; i >= 0; i--)
			{
				MetaData md = m_metaData[i];
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

			if(m_metaData.Count > 0)
				sb.Append(String.Format("{0}[0m", ESC));
			
			return sb.ToString();
		}
		
		public string ToDebugString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(m_text.ToString());

			int i = m_metaData.Count - 1;
			
			for(; i >= 0; i--)
			{
				string s = m_metaData[i].m_style.ToString();
				sb.Insert(m_metaData[i].m_index, s);
			}
			
			return sb.ToString();
		}

	}

}
