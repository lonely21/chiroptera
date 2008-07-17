
using System;
using SD = System.Drawing;

namespace BatMud.BatClientBase
{
	public struct Color : IEquatable<Color>
	{
		readonly SD.Color m_color;
		readonly bool m_notEmpty;
		readonly bool m_default;
		
		public int R { get { return m_color.R; } }
		public int G { get { return m_color.G; } }
		public int B { get { return m_color.B; } }

		public Color(int r, int g, int b)
		{
			m_color = SD.Color.FromArgb(r, g, b);
			m_notEmpty = true;		
			m_default = false;
		}
		
		private Color(bool def)
		{
			m_color = SD.Color.Empty;
			m_notEmpty = true;
			m_default = def;
		}
		
		public bool IsEmpty
		{
			get { return !m_notEmpty; }
		}
		
		public bool IsDefault
		{
			get { return m_notEmpty && m_default; }
		}

		public SD.Color ToSystemColor()
		{
			return m_color;
		}

		public string Name
		{
			get { return m_color.Name; }
		}

		public string ToHtml()
		{
			return SD.ColorTranslator.ToHtml(ToSystemColor());
		}

		public override bool Equals (object other)
		{
			if(!(other is Color))
				return false;
			return Equals((Color)other);
		}
		
		public bool Equals(Color other)
		{
			return m_color == other.m_color && m_notEmpty == other.m_notEmpty && m_default == other.m_default;
		}
		
		public static bool operator==(Color c1, Color c2)
		{
			return c1.Equals(c2);
		}

		public static bool operator!=(Color c1, Color c2)
		{
			return !c1.Equals(c2);
		}
		
		public override int GetHashCode ()
		{
			return m_color.GetHashCode();
		}
		
		public override string ToString ()
		{
			if(!m_notEmpty)
				return "Color(Empty)";
			
			if(m_default)
				return "Color(Default)";
			
			return String.Format("Color({0})", ToHtml());
		}

		
		
		
		public static readonly Color Default = new Color(true);
		public static readonly Color Empty = new Color();
		
		public static Color FromArgb(int r, int g, int b)
		{
			Color c = new Color(r, g, b);
			return c;
		}
		
		public static Color FromSystemColor(SD.Color color)
		{
			return FromArgb(color.R, color.G, color.B);
		}

		public static Color FromHtml(string color)
		{
			return Color.FromSystemColor(SD.ColorTranslator.FromHtml(color));
		}
		
	}
	
}
