using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BatMud.BatClientBase
{
	static public class ControlCodes
	{
		const char ESC = '\x1b';

		static string FgOpenTag(Color color)
		{
			return String.Format("{0}<20{1:x6}{2}|", ESC, color.ToArgb() & 0xffffff, ESC);
		}

		static string FgCloseTag()
		{
			return String.Format("{0}>20", ESC);
		}

		static string BgOpenTag(Color color)
		{
			return String.Format("{0}<21{1:x6}{2}|", ESC, color.ToArgb() & 0xffffff, ESC);
		}

		static string BgCloseTag()
		{
			return String.Format("{0}>21", ESC);
		}

		static public string ColorizeString(string str, Color foreground, Color background)
		{
			if (!foreground.IsEmpty)
				str = String.Format("{0}{1}{2}", FgOpenTag(foreground), str, FgCloseTag());

			if (!background.IsEmpty)
				str = String.Format("{0}{1}{2}", BgOpenTag(background), str, BgCloseTag());

			return str;
		}

		public static StringBuilder ParseBatControls(string text, out List<ColorMessage.MetaData> metaData)
		{
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			metaData = new List<ColorMessage.MetaData>();

			Color currentFgColor = Color.Empty;
			Color currentBgColor = Color.Empty;

			int pos = 0;
			int oldPos = 0;

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

				if (text[pos] == '<')
				{
					pos++; // skip <

					string codeStr = text.Substring(pos, 2);
					pos += 2;
					int code = 0;

					try
					{
						code = Int32.Parse(codeStr);
					}
					catch (Exception e)
					{
						BatConsole.WriteLine(e.ToString());
					}

					switch (code)
					{
						case 20:
							{
								string colorStr = text.Substring(pos, 6);
								pos += 6;
								int colorNum = Int32.Parse(colorStr, System.Globalization.NumberStyles.HexNumber);
								colorNum |= unchecked((int)0xff000000);
								currentFgColor = Color.FromArgb(colorNum);

								ColorMessage.MetaData md = new ColorMessage.MetaData(stringBuilder.Length, currentFgColor, currentBgColor);
								metaData.Add(md);

								pos += 2; // skip ESC|
								break;
							}

						case 21:
							{
								string colorStr = text.Substring(pos, 6);
								pos += 6;
								int colorNum = Int32.Parse(colorStr, System.Globalization.NumberStyles.HexNumber);
								colorNum |= unchecked((int)0xff000000);
								currentBgColor = Color.FromArgb(colorNum);

								ColorMessage.MetaData md = new ColorMessage.MetaData(stringBuilder.Length, currentFgColor, currentBgColor);
								metaData.Add(md);

								pos += 2; // skip ESC|
								break;
							}

						default:
							BatConsole.WriteLine("Unknown control code <{0}", code);
							break;
					}
				}
				else if (text[pos] == '>')
				{
					pos++; // skip >

					string codeStr = text.Substring(pos, 2);
					pos += 2;
					int code = Int32.Parse(codeStr);

					switch (code)
					{
						case 20:
							{
								currentFgColor = Color.Empty;

								ColorMessage.MetaData md = new ColorMessage.MetaData(stringBuilder.Length, currentFgColor, currentBgColor);
								metaData.Add(md);

								break;
							}

						case 21:
							{
								currentBgColor = Color.Empty;

								ColorMessage.MetaData md = new ColorMessage.MetaData(stringBuilder.Length, currentFgColor, currentBgColor);
								metaData.Add(md);

								break;
							}

						default:
							BatConsole.WriteLine("Unknown control code >{0}", code);
							break;
					}
				}
				else
				{
					BatConsole.WriteLine("Unknown esc-control command: {0}", text[pos]);
				}
			}

			return stringBuilder;
		}
	}
}
