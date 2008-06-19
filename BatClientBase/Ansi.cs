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
	public class Ansi
	{
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
			White   = 7
		}

		[Flags]
		public enum AnsiStyle
		{
			None = 0x00,
			HighIntensity = 0x01,
			Inverse = 0x02
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

		public static AnsiColor DefaultFgColor
		{
			get { return Ansi.AnsiColor.White; }
		}

		public static AnsiColor DefaultBgColor
		{
			get { return Ansi.AnsiColor.Black; }
		}

		public static void SendAnsiInit()
		{
			Console.WriteLine("\x1b[{0};{1}m", 30 + DefaultFgColor, 40 + DefaultBgColor);
		}

		public static Color AnsiColorToColor(AnsiColor ansiColor, bool highIntensity)
		{
			switch (ansiColor)
			{
				case AnsiColor.None:
					return Color.Empty;
				case AnsiColor.Black:
					return highIntensity ? m_ansiBoldColorArray[0] : m_ansiColorArray[0];
				case AnsiColor.Red:
					return highIntensity ? m_ansiBoldColorArray[1] : m_ansiColorArray[1];
				case AnsiColor.Green:
					return highIntensity ? m_ansiBoldColorArray[2] : m_ansiColorArray[2];
				case AnsiColor.Yellow:
					return highIntensity ? m_ansiBoldColorArray[3] : m_ansiColorArray[3];
				case AnsiColor.Blue:
					return highIntensity ? m_ansiBoldColorArray[4] : m_ansiColorArray[4];
				case AnsiColor.Purple:
					return highIntensity ? m_ansiBoldColorArray[5] : m_ansiColorArray[5];
				case AnsiColor.Cyan:
					return highIntensity ? m_ansiBoldColorArray[6] : m_ansiColorArray[6];
				case AnsiColor.White:
					return highIntensity ? m_ansiBoldColorArray[7] : m_ansiColorArray[7];
				default:
					throw new Exception("rikki");
			}
		}

		public static AnsiColor ColorToAnsiColor(Color color, out bool highIntensity)
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

			return NumberToAnsiColor(bestIdx);
		}

		public static AnsiColor NumberToAnsiColor(int colorNum)
		{
			switch (colorNum)
			{
				case 0:
					return AnsiColor.Black;
				case 1:
					return AnsiColor.Red;
				case 2:
					return AnsiColor.Green;
				case 3:
					return AnsiColor.Yellow;
				case 4:
					return AnsiColor.Blue;
				case 5:
					return AnsiColor.Purple;
				case 6:
					return AnsiColor.Cyan;
				case 7:
					return AnsiColor.White;
				default:
					throw new Exception("Väärä väri");
			}
		}

		public static ColorMessage ParseAnsi(string text,
			ref Ansi.AnsiColor currentFgColor, ref Ansi.AnsiColor currentBgColor, ref Ansi.AnsiStyle currentAnsiStyle)
		{
			const char ESC = '\x1b';

			StringBuilder stringBuilder = new StringBuilder(text.Length);
			List<ColorMessage.MetaData> metaData = new List<ColorMessage.MetaData>();

			int pos = 0;
			int oldPos = 0;

			AnsiColor fgColor = currentFgColor;
			AnsiColor bgColor = currentBgColor;
			AnsiStyle ansiStyle = currentAnsiStyle;

			AnsiColor lastFgColor = fgColor;
			AnsiColor lastBgColor = bgColor;
			AnsiStyle lastAnsiStyle = ansiStyle;

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
						BatConsole.WriteLine("Incomplete ansi sequence");
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
						BatConsole.WriteLine("Incomplete ansi sequence");
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
									fgColor = Ansi.AnsiColor.None;
									bgColor = Ansi.AnsiColor.None;
									ansiStyle = Ansi.AnsiStyle.None;
									break;
								case 1:		// bold
									ansiStyle |= Ansi.AnsiStyle.HighIntensity;
									break;

								case 7:			// inverse
									ansiStyle |= Ansi.AnsiStyle.Inverse;
									break;

								case 30:
								case 31:
								case 32:
								case 33:
								case 34:
								case 35:
								case 36:
								case 37:
									fgColor = Ansi.NumberToAnsiColor(num - 30);
									break;

								case 39:		// default color
									fgColor = Ansi.AnsiColor.None;
									break;


								case 40:
								case 41:
								case 42:
								case 43:
								case 44:
								case 45:
								case 46:
								case 47:
									bgColor = Ansi.NumberToAnsiColor(num - 40);
									break;

								case 49:		// default color
									bgColor = Ansi.AnsiColor.None;
									break;

								default:
									BatConsole.WriteLine("Unknown ansi code {0}", num);
									break;
							}
						}

						if (lastFgColor != fgColor ||
							lastBgColor != bgColor ||
							lastAnsiStyle != ansiStyle)
						{
							if (fgColor == Ansi.AnsiColor.None)
								fgColor = Ansi.DefaultFgColor;

							Color fg = Ansi.AnsiColorToColor(fgColor,
								(ansiStyle & Ansi.AnsiStyle.HighIntensity) != 0);

							Color bg;
							if (bgColor == Ansi.AnsiColor.None)
								bg = Color.Empty;
							else
								bg = Ansi.AnsiColorToColor(bgColor,
									(ansiStyle & Ansi.AnsiStyle.HighIntensity) != 0);

							//Console.WriteLine("FG {0} {1} -> {2}", currentAnsiFgColor, currentAnsiStyle, fg);
							//Console.WriteLine("BG {0} {1} -> {2}", currentAnsiBgColor, currentAnsiStyle, bg);

							ColorMessage.MetaData md = new ColorMessage.MetaData(stringBuilder.Length, fg, bg);
							metaData.Add(md);
						}

						lastFgColor = fgColor;
						lastBgColor = bgColor;
						lastAnsiStyle = ansiStyle;
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

			if (fgColor == DefaultFgColor)
				currentFgColor = AnsiColor.None;
			else
				currentFgColor = fgColor;

			if (bgColor == DefaultBgColor)
				currentBgColor = AnsiColor.None;
			else
				currentBgColor = bgColor;

			currentAnsiStyle = ansiStyle;

			return new ColorMessage(stringBuilder.ToString(), metaData);
		}
	}
}
