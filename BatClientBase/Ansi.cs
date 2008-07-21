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
      49	Default
*/


namespace BatMud.BatClientBase
{
	public class Ansi
	{
		const char ESC = '\x1b';
		
		public enum AnsiColor
		{
			None    = -1,
			Black   = 0,
			Red     = 1,
			Green   = 2,
			Yellow  = 3,
			Blue    = 4,
			Purple  = 5,
			Cyan    = 6,
			White   = 7,
			Default	= 9
		}

		[Flags]
		public enum AnsiStyle
		{
			None = 0,
			Default = 1<<0,
			HighIntensity = 1<<1,
			Inverse = 1<<2
		}

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

		static byte[] valuerange = { 0x00, 0x5F, 0x87, 0xAF, 0xD7, 0xFF };

		/*
		 * colors 16-231 are a 6x6x6 color cube
		 * colors 232-255 are a grayscale ramp, intentionally leaving out black and white
		 */

		static Color AnsiColor256ToColor(int color)
		{
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
				int r = valuerange[(color/36) % 6];
				int g = valuerange[(color/6) % 6];
				int b = valuerange[color % 6];
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

		static byte[,] colortable = new byte[256,3];
		
		static Ansi()
		{
			int c;
			
			for(c = 0; c < 256; c++)
			{
				Color color = AnsiColor256ToColor(c);
				colortable[c,0] = (byte)color.R;
				colortable[c,1] = (byte)color.G;
				colortable[c,2] = (byte)color.B;
				//BatConsole.WriteLineLow("{0}: {1}", c, color);
			}
		}
		
		public static int ColorToAnsiColor256(Color color)
		{
			int c, best_match=0;
			double d, smallest_distance;

			smallest_distance = 10000000000.0;

			for(c = 16; c < 256; c++)
			{
				d = Math.Pow(colortable[c,0] - color.R, 2.0) + 
					Math.Pow(colortable[c,1] - color.G, 2.0) + 
						Math.Pow(colortable[c,2] - color.B, 2.0);
				
				if(d < smallest_distance)
				{
					smallest_distance = d;
					best_match = c;
				}
			}
			
			return best_match;
		}

		public static ColorMessage ParseAnsi(string text, ref TextStyle currentStyle)
		{
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			List<ColorMessage.MetaData> metaData = new List<ColorMessage.MetaData>();

			int pos = 0;
			int oldPos = 0;

			Color fgColor = currentStyle.Fg;
			Color bgColor = currentStyle.Bg;
			TextStyleFlags flags = currentStyle.Flags;

			Color lastFgColor = fgColor;
			Color lastBgColor = bgColor;
			TextStyleFlags lastFlags = flags;
			
			if(!currentStyle.Fg.IsEmpty ||
			   !currentStyle.Bg.IsEmpty ||
			   (currentStyle.Flags & TextStyleFlags.Empty) != 0)
			{
				ColorMessage.MetaData md = new ColorMessage.MetaData(stringBuilder.Length, currentStyle);
				metaData.Add(md);
			}
/*
			if (fgColor != Ansi.AnsiColor.None ||
				bgColor != Ansi.AnsiColor.None ||
				ansiStyle != Ansi.AnsiStyle.None)
			{
				Color fg = Ansi.AnsiColorToColor(fgColor,
					(ansiStyle & Ansi.AnsiStyle.HighIntensity) != 0);

				Color bg;
				if (bgColor == Ansi.AnsiColor.None)
					bg = Color.Empty;
				else
					bg = Ansi.AnsiColorToColor(bgColor,
						(ansiStyle & Ansi.AnsiStyle.HighIntensity) != 0);

				ColorMessage.MetaData md = new ColorMessage.MetaData(stringBuilder.Length, fg, bg);
				metaData.Add(md);
			}
*/
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
						BatConsole.WriteLineLow("Incomplete ansi sequence");
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
						BatConsole.WriteLineLow("Incomplete ansi sequence");
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
									fgColor = Color.Default;
									bgColor = Color.Default;
									flags = TextStyleFlags.None;
									break;
								case 1:		// bold
									flags |= TextStyleFlags.HighIntensity;
									break;

								case 7:			// inverse
									flags |= TextStyleFlags.Inverse;
									break;

								case 30:
								case 31:
								case 32:
								case 33:
								case 34:
								case 35:
								case 36:
								case 37:
									fgColor = Ansi.AnsiColor8ToColor(num - 30, false);
									break;
								
									// 38;5;c
								case 38:
									if(arr.Length != 3 || i != 0 || arr[1] != "5")
									{
										BatConsole.WriteLineLow("Illegal 256 color ansi code: {0}", str2);
										break;
									}
									
									fgColor = AnsiColor256ToColor(byte.Parse(arr[2]));
									
									i += 3;
									
									break;

								case 39:		// default color
									fgColor = Color.Default;
									break;


								case 40:
								case 41:
								case 42:
								case 43:
								case 44:
								case 45:
								case 46:
								case 47:
									bgColor = Ansi.AnsiColor8ToColor(num - 40, false);
									break;

								case 48:
									if(arr.Length != 3 || i != 0 || arr[1] != "5")
									{
										BatConsole.WriteLineLow("Illegal 256 color ansi code: {0}", str2);
										break;
									}
									
									bgColor = AnsiColor256ToColor(byte.Parse(arr[2]));
									
									i += 3;
									
									break;
									
								case 49:		// default color
									bgColor = Color.Default;
									break;

								default:
									BatConsole.WriteLineLow("Unknown ansi code {0}", num);
									break;
							}
						}

						if (lastFgColor != fgColor ||
							lastBgColor != bgColor ||
							lastFlags != flags)
						{
							/*
							if (fgColor == Ansi.AnsiColor.Default)
								fgColor = Ansi.DefaultFgColor;
								*/
/*
							TextStyleFlags flags;
							if((ansiStyle & Ansi.AnsiStyle.HighIntensity) != 0)
								flags = TextStyleFlags.HighIntensity;
							else if((ansiStyle & Ansi.AnsiStyle.Default) != 0)
								flags = TextStyleFlags.None;
*/
							//Console.WriteLine("FG {0} {1} -> {2}", currentAnsiFgColor, currentAnsiStyle, fg);
							//Console.WriteLine("BG {0} {1} -> {2}", currentAnsiBgColor, currentAnsiStyle, bg);
							
							TextStyle style = new TextStyle(fgColor, bgColor, flags);

							ColorMessage.MetaData md = new ColorMessage.MetaData(stringBuilder.Length, style);
							metaData.Add(md);
						}

						lastFgColor = fgColor;
						lastBgColor = bgColor;
						lastFlags = flags;
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
						BatConsole.WriteLine("Unknown ansi command: {0}", text[pos]);
					}
				}
			}

			if(fgColor == Color.Default)
				fgColor = Color.Empty;
			
			if(bgColor == Color.Default)
				bgColor = Color.Empty;
			
			if((flags & TextStyleFlags.None) != 0)
				flags = TextStyleFlags.Empty;
			
			currentStyle = new TextStyle(fgColor, bgColor, flags);
			/*
			if (fgColor == DefaultFgColor)
				currentFgColor = AnsiColor.None;
			else
				currentFgColor = fgColor;

			if (bgColor == DefaultBgColor)
				currentBgColor = AnsiColor.None;
			else
				currentBgColor = bgColor;

			currentAnsiStyle = ansiStyle;
*/
			return new ColorMessage(stringBuilder.ToString(), metaData);
		}

		static public string ColorizeString(string str, Color foreground, Color background)
		{
			int fg = ColorToAnsiColor256(foreground);
			int bg = ColorToAnsiColor256(background);
			
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
