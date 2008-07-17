
using System;
using SD = System.Drawing;

namespace BatMud.BatClientBase
{
	[Flags]
	public enum TextStyleFlags
	{
		Empty = 0,		// No flags, doesn't change the text
		None = 1,		// No flags, resets the text to defaults
		HighIntensity = 2,
		Inverse = 3
	}
	
	public class TextStyle
	{
		Color m_fgColor;
		Color m_bgColor;
		TextStyleFlags m_flags;
		
		public TextStyle()
		{
		}
		
		public TextStyle(TextStyle style)
		{
			m_fgColor = style.m_fgColor;
			m_bgColor = style.m_bgColor;
			m_flags = style.m_flags;
		}
		
		public TextStyle(Color fg, Color bg)
			: this(fg, bg, 0)
		{
		}
		
		public TextStyle(TextStyleFlags flags)
		{
			m_flags = flags;
		}
		
		public TextStyle(Color fg, Color bg, TextStyleFlags flags)
		{
			m_fgColor = fg;
			m_bgColor = bg;
			m_flags = flags;
		}
		
		public TextStyleFlags Flags { get { return m_flags; } }
		public Color Fg { get { return m_fgColor; } }
		public Color Bg { get { return m_bgColor; } }
		public bool IsHighIntensity { get { return (m_flags & TextStyleFlags.HighIntensity) != 0; } }
		public bool IsReverse  { get { return (m_flags & TextStyleFlags.Inverse) != 0; } }
		
		public TextStyle Combine(TextStyle style)
		{
			TextStyle s = new TextStyle(this);
			
			if(m_fgColor.IsEmpty)
				s.m_fgColor = style.m_fgColor;
			
			if(m_bgColor.IsEmpty)
				s.m_bgColor = style.m_bgColor;
			
			if(m_flags == TextStyleFlags.Empty)
				s.m_flags = style.m_flags;
			
			return s;
		}
		
		public override string ToString ()
		{
			return String.Format("<{0}/{1}/{2}>", m_fgColor, m_bgColor, m_flags);
			
		}

		
		public static TextStyle CreateDefaultStyle()
		{
			return new TextStyle(Color.Default, Color.Default, TextStyleFlags.None);
		}
	}
}
