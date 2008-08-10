#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing; 

#endregion

/*
  Set Attribute Mode	<ESC>[{attr1};...;{attrn}m

    * Sets multiple display attribute settings. The following lists standard attributes:

      0	Reset all attributes
      1	Bright
      2	Dim
      3 Italics
      4	Underscore	
      5	Blink
      7	Reverse
      8	Hidden
      9 Strikethrough
      22 Bold off
      23 Italics off
      24 Underline off
      27 Inverse off
      29 Strikethrough off

      	Foreground Colours
      30	Black
      31	Red
      32	Green
      33	Yellow
      34	Blue
      35	Magenta
      36	Cyan
      37	White
      38;5;c	256 color
      39	Default

      	Background Colours
      40	Black
      41	Red
      42	Green
      43	Yellow
      44	Blue
      45	Magenta
      46	Cyan
      47	White
      48;5;c	256 color
      49	Default
*/


namespace Chiroptera.Base
{
	public struct AnsiTextStyle
	{
		public int Fg;
		public int Bg;
		public bool IsBold;
		public bool IsInverse;

		public static readonly AnsiTextStyle Empty;
		
		static AnsiTextStyle()
		{
			Empty = new AnsiTextStyle();
			Empty.Fg = -1;
			Empty.Bg = -1;
			Empty.IsBold = false;
			Empty.IsInverse = false;
		}
		
		public bool IsEmpty
		{
			get { return Fg == -1 && Bg == -1 && IsBold == false && IsInverse == false; }
		}
		
		public TextStyle ToTextStyle()
		{
			int fgNum = Fg;
			if(IsBold && fgNum < 8)
				fgNum += 8;
			Color fg = Ansi.AnsiColor256ToColor(fgNum);
			Color bg = Ansi.AnsiColor256ToColor(Bg);
			TextStyleFlags flags = TextStyleFlags.Empty;
			if(IsBold)
				flags |= TextStyleFlags.HighIntensity;
			if(IsInverse)
				flags |= TextStyleFlags.Inverse;
			return new TextStyle(fg, bg, flags);
		}
		
		public bool IsSame(AnsiTextStyle style)
		{
			return Fg == style.Fg && Bg == style.Bg && IsBold == style.IsBold && IsInverse == style.IsInverse;
		}
	}
		
	public class Ansi
	{
		const char ESC = '\x1b';

		static Color[] m_ansiColorArray = new Color[] { 
			Color.FromArgb(0, 0, 0),
			Color.FromArgb(160, 0, 0), Color.FromArgb(0, 160, 0), Color.FromArgb(160, 160, 0),
			Color.FromArgb(0, 0, 160), Color.FromArgb(160, 0, 160), Color.FromArgb(0, 160, 160),
			Color.FromArgb(160, 160, 160) 
		};

		static Color[] m_ansiBoldColorArray = new Color[] { 
			Color.FromArgb(85, 85, 85),
			Color.FromArgb(255, 0, 0), Color.FromArgb(0, 255, 0), Color.FromArgb(255, 255, 0),
			Color.FromArgb(0, 0, 255), Color.FromArgb(255, 0, 255), Color.FromArgb(0, 255, 255),
			Color.FromArgb(255, 255, 255) 
		};

		static byte[,] s_colorTable = new byte[256, 3];
		static byte[] s_valueRange = { 0x00, 0x5F, 0x87, 0xAF, 0xD7, 0xFF };

		static Ansi()
		{
			int c;

			for (c = 0; c < 256; c++)
			{
				Color color = AnsiColor256ToColor(c);
				s_colorTable[c, 0] = (byte)color.R;
				s_colorTable[c, 1] = (byte)color.G;
				s_colorTable[c, 2] = (byte)color.B;
				//if(c < 16)
				//	ChiConsole.WriteLineLow("{0}: {1}", c, color);
			}
		}
		
		public static Color AnsiColor8ToColor(int color, bool highIntensity)
		{
			switch (color)
			{
				case -1:
					return Color.Empty;
				case 9:
					return Color.Default;				
				case 0:
					return highIntensity ? m_ansiBoldColorArray[0] : m_ansiColorArray[0];
				case 1:
					return highIntensity ? m_ansiBoldColorArray[1] : m_ansiColorArray[1];
				case 2:
					return highIntensity ? m_ansiBoldColorArray[2] : m_ansiColorArray[2];
				case 3:
					return highIntensity ? m_ansiBoldColorArray[3] : m_ansiColorArray[3];
				case 4:
					return highIntensity ? m_ansiBoldColorArray[4] : m_ansiColorArray[4];
				case 5:
					return highIntensity ? m_ansiBoldColorArray[5] : m_ansiColorArray[5];
				case 6:
					return highIntensity ? m_ansiBoldColorArray[6] : m_ansiColorArray[6];
				case 7:
					return highIntensity ? m_ansiBoldColorArray[7] : m_ansiColorArray[7];
				default:
					throw new Exception("rikki");
			}
		}

		public static int ColorToAnsiColor8(Color color, out bool highIntensity)
		{
			int bestIdx = -1;
			int bestDiff = Int32.MaxValue;
			highIntensity = false;
			
			for(int i = 0; i < m_ansiColorArray.Length; i++)
			{
				int diff = 0;
				diff += Math.Abs(color.R - m_ansiColorArray[i].R);
				diff += Math.Abs(color.G - m_ansiColorArray[i].G);
				diff += Math.Abs(color.B - m_ansiColorArray[i].B);
				if(diff < bestDiff)
				{
					bestDiff = diff;
					bestIdx = i;
					highIntensity = false;
				}
			}

			for(int i = 0; i < m_ansiBoldColorArray.Length; i++)
			{
				int diff = 0;
				diff += Math.Abs(color.R - m_ansiBoldColorArray[i].R);
				diff += Math.Abs(color.G - m_ansiBoldColorArray[i].G);
				diff += Math.Abs(color.B - m_ansiBoldColorArray[i].B);
				if(diff < bestDiff)
				{
					bestDiff = diff;
					bestIdx = i;
					highIntensity = true;
				}
			}

			return bestIdx;
		}

		/*
		 * colors 16-231 are a 6x6x6 color cube
		 * colors 232-255 are a grayscale ramp, intentionally leaving out black and white
		 */

		public static Color AnsiColor256ToColor(int color)
		{
			if(color == -1)
				return Color.Empty;
			
			Color c;
			
			if(color < 16)
			{
				// 16 basic colors
				if(color < 8)
					c = m_ansiColorArray[color];
				else
					c = m_ansiBoldColorArray[color - 8];
			}
			else if(color < 232)
			{
				// color cube color
				color -= 16;
				int r = s_valueRange[(color/36) % 6];
				int g = s_valueRange[(color/6) % 6];
				int b = s_valueRange[color % 6];
				c = new Color(r, g, b);
			}
			else if(color < 256)
			{
				// gray tone
				int g = 8 + (color-232) * 10;
				c = new Color(g, g, g);
			}
			else
				throw new Exception("illegal 256 color code");
			
			return c;
		}

		public static int ColorToAnsiColor256(Color color, bool isHi)
		{
			int c, best_match=0;
			double d, smallest_distance;

			smallest_distance = 10000000000.0;

			// hack. for hilites, try to find basic system color
			if(isHi)
				c = 0;
			else
				c = 16;
			
			for(; c < 256; c++)
			{
				d = Math.Pow(s_colorTable[c,0] - color.R, 2.0) + 
					Math.Pow(s_colorTable[c,1] - color.G, 2.0) + 
						Math.Pow(s_colorTable[c,2] - color.B, 2.0);
				
				if(d < smallest_distance)
				{
					smallest_distance = d;
					best_match = c;
				}
			}
			
			return best_match;
		}
		
		public static string ColorToAnsiString256(Color color, bool isBg, bool isHi)
		{
			if(color.IsDefault)
				return String.Format("\x1b[{0}m", isBg ? 49 : 39);

			int c = ColorToAnsiColor256(color, isHi);
			return String.Format("\x1b[{0};5;{1}m", isBg ? 48 : 38, c);
		}

		public static string ColorToAnsiString8(Color color, bool isBg)
		{
			if(color.IsDefault)
				return String.Format("\x1b[{0}m", isBg ? 49 : 39);
			
			bool hi;
			int c = ColorToAnsiColor8(color, out hi);

			StringBuilder sb = new StringBuilder();
			if(hi && !isBg)
				sb.Append("\x1b[1m");
			sb.AppendFormat("\x1b[{0}m", (isBg ? 40 : 30) + c);
			return sb.ToString();
		}
		
		public static ColorMessage ParseAnsi(string text, ref AnsiTextStyle currentStyle)
		{
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			List<ColorMessage.MetaData> metaData = new List<ColorMessage.MetaData>();

			int pos = 0;
			int oldPos = 0;
			
			AnsiTextStyle previousStyle = currentStyle;
			
			if(!currentStyle.IsEmpty)
			{
				ColorMessage.MetaData md = new ColorMessage.MetaData(stringBuilder.Length, currentStyle.ToTextStyle());
				metaData.Add(md);
			}

			while (pos < text.Length)
			{
				if (text[pos] == '\t')
				{
					stringBuilder.Append(' ', 4);
					pos++;
					continue;
				}

				if (text[pos] != ESC)
				{
					stringBuilder.Append(text[pos]);
					pos++;
					continue;
				}

				oldPos = pos;

				pos++; // skip ESC

				if (pos >= text.Length)
				{
					stringBuilder.Append(text.Substring(oldPos, pos - oldPos));
					continue;
				}

				if (text[pos] == '[')
				{
					pos++; // skip [

					if (pos >= text.Length)
					{
						ChiConsole.WriteLineLow("Incomplete ansi sequence");
						stringBuilder.Append(text.Substring(oldPos, pos - oldPos));
						continue;
					}

					int seqStart = pos;

					while (pos < text.Length && ((text[pos] >= '0' && text[pos] <= '9') || text[pos] == ';'))
					{
						pos++;
					}

					if (pos == text.Length)
					{
						ChiConsole.WriteLineLow("Incomplete ansi sequence");
						stringBuilder.Append(text.Substring(oldPos, pos - oldPos));
						continue;
					}

					if (text[pos] == 'm')
					{
						int seqEnd = pos;

						pos++; // skip m

						string str2 = text.Substring(seqStart, seqEnd - seqStart);

						string[] arr = str2.Split(';');

						if (str2.Length == 0)
							arr = new string[] { "0" };

						for (int i = 0; i < arr.Length; i++)
						{
							int num = System.Int16.Parse(arr[i]);

							switch (num)
							{
								case 0:		// normal
									currentStyle.Fg = -1;
									currentStyle.Bg = -1;
									currentStyle.IsBold = false;
									currentStyle.IsInverse = false;
									break;
								case 1:		// bold
									currentStyle.IsBold = true;
									break;

								case 7:			// inverse
									currentStyle.IsInverse = true;
									break;

								case 30:
								case 31:
								case 32:
								case 33:
								case 34:
								case 35:
								case 36:
								case 37:
									currentStyle.Fg = num - 30;
									break;
								
								case 38:
									if(arr.Length != 3 || i != 0 || arr[1] != "5")
									{
										ChiConsole.WriteLineLow("Illegal 256 color ansi code: {0}", str2);
										break;
									}
									
									currentStyle.Fg = byte.Parse(arr[2]);
									
									i += 3;
									
									break;

								case 39:		// default color
									currentStyle.Fg = -1;
									break;


								case 40:
								case 41:
								case 42:
								case 43:
								case 44:
								case 45:
								case 46:
								case 47:
									currentStyle.Bg = num - 40;
									break;

								case 48:
									if(arr.Length != 3 || i != 0 || arr[1] != "5")
									{
										ChiConsole.WriteLineLow("Illegal 256 color ansi code: {0}", str2);
										break;
									}
									
									currentStyle.Bg = byte.Parse(arr[2]);
									
									i += 3;
									
									break;
									
								case 49:		// default color
									currentStyle.Bg = -1;
									break;

								default:
									ChiConsole.WriteLineLow("Unknown ansi code {0}", num);
									break;
							}
						}

						if(!previousStyle.IsSame(currentStyle))
						{
							ColorMessage.MetaData md = new ColorMessage.MetaData(stringBuilder.Length, currentStyle.ToTextStyle());
							metaData.Add(md);
						}
						
						previousStyle = currentStyle;
					}
					else if (text[pos] == 'H')
					{
						pos++;
					}
					else if (text[pos] == 'J')
					{
						pos++;
					}
					else
					{
						ChiConsole.WriteLine("Unknown ansi command: {0}", text[pos]);
					}
				}
			}

			return new ColorMessage(stringBuilder.ToString(), metaData);
		}
		
		static public string ColorizeString(string str, Color foreground, Color background)
		{
			int fg = ColorToAnsiColor256(foreground, false);
			int bg = ColorToAnsiColor256(background, false);
			
			StringBuilder sb = new StringBuilder();

			if (!foreground.IsEmpty)
			{
				sb.AppendFormat("\x1b[38;5;{0}m", fg);
			}
			
			if (!background.IsEmpty)
			{
				sb.AppendFormat("\x1b[48;5;{0}m", bg);
			}
			
			sb.Append(str);
			
			sb.Append("\x1b[0m");
			
			return sb.ToString();
		}

	}
}
