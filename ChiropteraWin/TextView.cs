using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Chiroptera.Win
{
	public partial class TextViewControl : UserControl
	{
		#region BitBlt
		/// <summary>
		///    Performs a bit-block transfer of the color data corresponding to a
		///    rectangle of pixels from the specified source device context into
		///    a destination device context.
		/// </summary>
		/// <param name="hdc">Handle to the destination device context.</param>
		/// <param name="nXDest">The leftmost x-coordinate of the destination rectangle (in pixels).</param>
		/// <param name="nYDest">The topmost y-coordinate of the destination rectangle (in pixels).</param>
		/// <param name="nWidth">The width of the source and destination rectangles (in pixels).</param>
		/// <param name="nHeight">The height of the source and the destination rectangles (in pixels).</param>
		/// <param name="hdcSrc">Handle to the source device context.</param>
		/// <param name="nXSrc">The leftmost x-coordinate of the source rectangle (in pixels).</param>
		/// <param name="nYSrc">The topmost y-coordinate of the source rectangle (in pixels).</param>
		/// <param name="dwRop">A raster-operation code.</param>
		/// <returns>
		///    <c>true</c> if the operation succeeded, <c>false</c> otherwise.
		/// </returns>
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
			int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

		/// <summary>
		///     Specifies a raster-operation code. These codes define how the color data for the
		///     source rectangle is to be combined with the color data for the destination
		///     rectangle to achieve the final color.
		/// </summary>
		public enum TernaryRasterOperations : uint
		{
			/// <summary>dest = source</summary>
			SRCCOPY = 0x00CC0020,
			/// <summary>dest = source OR dest</summary>
			SRCPAINT = 0x00EE0086,
			/// <summary>dest = source AND dest</summary>
			SRCAND = 0x008800C6,
			/// <summary>dest = source XOR dest</summary>
			SRCINVERT = 0x00660046,
			/// <summary>dest = source AND (NOT dest)</summary>
			SRCERASE = 0x00440328,
			/// <summary>dest = (NOT source)</summary>
			NOTSRCCOPY = 0x00330008,
			/// <summary>dest = (NOT src) AND (NOT dest)</summary>
			NOTSRCERASE = 0x001100A6,
			/// <summary>dest = (source AND pattern)</summary>
			MERGECOPY = 0x00C000CA,
			/// <summary>dest = (NOT source) OR dest</summary>
			MERGEPAINT = 0x00BB0226,
			/// <summary>dest = pattern</summary>
			PATCOPY = 0x00F00021,
			/// <summary>dest = DPSnoo</summary>
			PATPAINT = 0x00FB0A09,
			/// <summary>dest = pattern XOR dest</summary>
			PATINVERT = 0x005A0049,
			/// <summary>dest = (NOT dest)</summary>
			DSTINVERT = 0x00550009,
			/// <summary>dest = BLACK</summary>
			BLACKNESS = 0x00000042,
			/// <summary>dest = WHITE</summary>
			WHITENESS = 0x00FF0062
		}
		#endregion

		public delegate void LinkClickDelegate(string link);
		public event LinkClickDelegate LinkClicked;

		#region Member variables
		ParagraphContainer m_paragraphContainer;

		/// <summary>Number of new paragraphs added since last OnPaint</summary>
		int m_paragraphsAdded = 0;

		/// <summary>Paragraph index at the bottom of the screen (possibly not painted yet)</summary>
		int m_paragraphAtBottom = -1;

		/// <summary>Amount of paragraphs to scroll, can be positive or negative</summary>
		int m_scrollOffset = 0;

		/// <summary>Paragraph index at the bottom of the screen</summary>
		int m_paragraphAtBottomOnScreen = -1;

		/// <summary>Number of lines on screen</summary>
		int m_linesOnScreen;

		/// <summary>Number of paragraphs on screen</summary>
		int m_paragraphsOnScreen;

		/// <summary>Number of text lines that fit in the control</summary>
		int m_lines;

		/// <summary>Number of columns that fit in the control</summary>
		int m_columns;

		/// <summary>Width of a single character in pixels</summary>
		float m_charWidth;

		/// <summary>Height of a single character in pixels</summary>
		int m_charHeight;

		/// <summary>Indicates that Paint should redraw everything</summary>
		bool m_redrawScreen = false;

		Font m_underlineFont;
		SolidBrush m_defaultBrush = new SolidBrush(Color.Black); // Brush for the text background
		SolidBrush m_backgroundBrush;
		BufferedGraphics m_buffer;

		bool m_scrollLock = false;

		// Selection support
		bool m_selectionScrollLock = false; // stores scrolllock status during selection

		// selection start
		int m_selection1ParagraphNum = -1;
		int m_selection1ParagraphPos;
		//selection end
		int m_selection2ParagraphNum = -1;
		int m_selection2ParagraphPos;

		Point m_clickPos1;		// Position where mouse button went down

		#endregion

		public TextViewControl()
		{
			InitializeComponent();

			RecalcFontSize();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Opaque, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.UpdateStyles();
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			m_backgroundBrush = new SolidBrush(this.BackColor);
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);

			RecalcFontSize();
		}

		private void RecalcFontSize()
		{
			using (Graphics g = this.CreateGraphics())
			{
				StringFormat format = StringFormat.GenericTypographic;
				SizeF size = g.MeasureString("M", this.Font, 10000, format);
				m_charHeight = (int)(size.Height + 0.5);
				m_charWidth = size.Width;

				if (m_underlineFont != null)
					m_underlineFont.Dispose();
				m_underlineFont = new Font(this.Font, FontStyle.Underline);

				RecalcSize();
			}

			m_redrawScreen = true;
			this.Invalidate();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			RecalcSize();

			if (m_buffer != null)
				m_buffer.Dispose();
			BufferedGraphicsContext ctx = BufferedGraphicsManager.Current;
			Rectangle r = this.ClientRectangle;
			r.Width -= scrollBar.Width;
			m_buffer = ctx.Allocate(this.CreateGraphics(), r);
		}

		private void RecalcSize()
		{
			if (m_charHeight > 0)
				m_lines = this.ClientSize.Height / m_charHeight;

			if (m_charWidth > 0)
			{
				m_columns = (int)((this.ClientSize.Width - scrollBar.Width) / m_charWidth);
				if (m_columns < 0)
					m_columns = 0;
			}

			if (m_paragraphContainer != null)
			{
				m_paragraphContainer.SetColumns(m_columns);
			}

			UpdateScrollBar();

			m_redrawScreen = true;
			this.Invalidate();
		}

		public ParagraphContainer ParagraphContainer
		{
			set
			{
				if (m_paragraphContainer != null)
					m_paragraphContainer.paragraphAddedEvent -= OnParagraphAddedEvent;

				m_paragraphContainer = value;

				if (m_paragraphContainer != null)
				{
					m_paragraphContainer.paragraphAddedEvent += OnParagraphAddedEvent;
					m_paragraphContainer.SetColumns(m_columns);
					m_paragraphAtBottom = m_paragraphContainer.Count - 1;

					UpdateScrollBar();
				}
				else
				{
					scrollBar.Maximum = 0;
					scrollBar.LargeChange = 1;
				}

				m_scrollLock = false;
				m_redrawScreen = true;

				this.Invalidate();
			}

			get
			{
				return m_paragraphContainer;
			}
		}

		void OnParagraphAddedEvent(bool historyFull)
		{
			if (m_scrollLock == false)
			{
				if (m_paragraphAtBottom == m_paragraphContainer.Count - 2)
				{
					//Console.WriteLine("scroll");
					m_paragraphsAdded++;
				}
				else
				{
					//Console.WriteLine("redraw");
					m_redrawScreen = true;
				}

				m_paragraphAtBottom = m_paragraphContainer.Count - 1;

				this.Invalidate();
			}
			else if (historyFull)
			{
				if (m_paragraphAtBottom > m_paragraphsOnScreen)
				{
					m_paragraphAtBottom--;
				}
				else
				{
					m_scrollOffset++;
					//m_redrawScreen = true;
					this.Invalidate();
				}
			}

			if(historyFull)
			{
				if (m_selection1ParagraphNum != -1)
				{
					if (m_selection1ParagraphNum == 0)
					{
						m_selection1ParagraphPos = 0;
					}
					else
					{
						m_selection1ParagraphNum--;
					}
				}

				if (m_selection2ParagraphNum != -1)
				{
					if (m_selection2ParagraphNum == 0)
					{
						m_selection2ParagraphPos = 0;
					}
					else
					{
						m_selection2ParagraphNum--;
					}
				}

				if (m_selection1ParagraphNum == 0 && m_selection1ParagraphPos == 0 &&
					m_selection2ParagraphNum == 0 && m_selection2ParagraphPos == 0)
				{
					m_selection1ParagraphNum = -1;
					m_selection2ParagraphNum = -1;
				}
			}


			UpdateScrollBar();
		}

		void UpdateScrollBar()
		{
			if (m_paragraphAtBottom != -1)
			{
				int lines = 0, paragraphs = 0;

				for (int i = m_paragraphAtBottom; i >= 0 && lines < m_lines; i--)
				{
					paragraphs++;
					lines += m_paragraphContainer[i].m_lines;
				}

				scrollBar.Maximum = m_paragraphContainer.Count - 1;
				scrollBar.LargeChange = paragraphs;
				if(paragraphs > 0)
					scrollBar.Value = m_paragraphAtBottom - scrollBar.LargeChange + 1;

				//Console.WriteLine("Scrollbar max {0}, largechange {1}, value {2}, bot {3}",
				//	scrollBar.Maximum, scrollBar.LargeChange, scrollBar.Value, m_paragraphAtBottom);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.DesignMode)
			{
				e.Graphics.Clear(Color.Black);
				return;
			}

			PaintBuffer();

			m_buffer.Render(e.Graphics);

			DrawSelection(e.Graphics);
		}

		void PaintBuffer()
		{
			Graphics g = m_buffer.Graphics;

			if (m_paragraphContainer == null)
			{
				g.Clear(this.BackColor);
				m_redrawScreen = false;
				return;
			}

			int h = m_lines;

			if (m_redrawScreen)
			{
				//Console.WriteLine("Redrawing all lines");

				g.Clear(this.BackColor);

				// Draw paragraphs starting from the bottom of the screen, ie. from the newest paragraph
				int paragraphNum = m_paragraphAtBottom;

				m_linesOnScreen = 0;
				m_paragraphsOnScreen = 0;

				while (paragraphNum >= 0 && m_linesOnScreen < m_lines)
				{
					Paragraph paragraph = m_paragraphContainer[paragraphNum];

					h -= paragraph.m_lines;

					DrawParagraph(paragraph, h, g);

					m_linesOnScreen += paragraph.m_lines;
					m_paragraphsOnScreen++;
					paragraphNum--;
				}

			}
			else if (m_scrollOffset != 0)
			{
				//Console.WriteLine("Scrolling {0} paragraphs", m_scrollOffset);

				if (m_scrollOffset > 0)
				{
					int newLines = 0;
					for (int i = 0; i < m_scrollOffset; i++)
					{
						newLines += m_paragraphContainer[m_paragraphAtBottomOnScreen + i + 1].m_lines;
					}

					//Console.WriteLine("Height of new lines {0}", newLines);

					if (m_lines > newLines)
						CopyLines(g, newLines, 0, m_lines - newLines);

					ClearLines(g, m_lines - newLines, newLines);

					// Draw paragraphs starting from the bottom of the screen, ie. from the newest paragraph
					int paragraphNum = m_paragraphAtBottom;

					m_linesOnScreen = 0;
					m_paragraphsOnScreen = 0;

					while (paragraphNum >= 0 && m_linesOnScreen < m_lines)
					{
						Paragraph paragraph = m_paragraphContainer[paragraphNum];

						if (m_paragraphsOnScreen >= m_scrollOffset)
						{
							// All new paragraphs have been drawn
							m_linesOnScreen += paragraph.m_lines;
							m_paragraphsOnScreen++;
							paragraphNum--;
							continue;
						}

						h -= paragraph.m_lines;

						DrawParagraph(paragraph, h, g);

						m_linesOnScreen += paragraph.m_lines;
						m_paragraphsOnScreen++;
						paragraphNum--;
					}
				}
				else // m_scrollOffset < 0
				{
					m_scrollOffset = Math.Abs(m_scrollOffset);

					int oldLines = 0;
					for (int i = 0; i < m_scrollOffset; i++)
					{
						oldLines += m_paragraphContainer[m_paragraphAtBottomOnScreen - i].m_lines;
					}

					//Console.WriteLine("Height of old lines {0}", oldLines);

					if (m_lines > oldLines)
						CopyLines(g, 0, oldLines, m_lines - oldLines);

					//Console.WriteLine("{0} - {1} = {2}", m_linesOnScreen, m_lines, m_linesOnScreen - m_lines);

					// TODO: 
					// m_linesOnScreen - m_lines tells how many lines are (were) hidden of the topmost paragraph
					// this should clear also the visible lines of the partial paragraph
					// loop through paragraphs to find the number of lines?
					ClearLines(g, 0, oldLines);

					// Draw paragraphs starting from the bottom of the screen, ie. from the newest paragraph
					int paragraphNum = m_paragraphAtBottom;
					int skip = m_paragraphsOnScreen - m_scrollOffset;
					if (m_linesOnScreen - m_lines != 0)
						skip--; // skip one less to draw the partial paragraph
					//Console.WriteLine("Skip {0}", skip);

					m_linesOnScreen = 0;
					m_paragraphsOnScreen = 0;

					while (paragraphNum >= 0 && m_linesOnScreen < m_lines)
					{
						Paragraph paragraph = m_paragraphContainer[paragraphNum];

						h -= paragraph.m_lines;

						if (skip <= 0)
						{
							DrawParagraph(paragraph, h, g);
						}
						else
							skip--;

						m_linesOnScreen += paragraph.m_lines;
						m_paragraphsOnScreen++;
						paragraphNum--;
					}
				}
			}
			else if (m_paragraphsAdded > 0)
			{
				//Console.WriteLine("Drawing {0} new paragraphs", m_paragraphsAdded);

				// height of new paragraphs
				int newLines = 0;
				for (int i = 0; i < m_paragraphsAdded; i++)
				{
					newLines += m_paragraphContainer[m_paragraphAtBottom - i].m_lines;
				}

				//Console.WriteLine("Height of new lines {0}", newLines);

				// Copy old stuff
				CopyLines(g, newLines, 0, m_lines - newLines);

				// Clear background for new lines
				ClearLines(g, m_lines - newLines, newLines);

				// Draw paragraphs starting from the bottom of the screen, ie. from the newest paragraph
				int paragraphNum = m_paragraphAtBottom;

				m_linesOnScreen = 0;
				m_paragraphsOnScreen = 0;

				while (paragraphNum >= 0 && m_linesOnScreen < m_lines)
				{
					Paragraph paragraph = m_paragraphContainer[paragraphNum];

					if (m_paragraphsOnScreen >= m_paragraphsAdded)
					{
						// All new paragraphs have been drawn
						m_linesOnScreen += paragraph.m_lines;
						m_paragraphsOnScreen++;
						paragraphNum--;
						continue;
					}

					h -= paragraph.m_lines;

					DrawParagraph(paragraph, h, g);

					m_linesOnScreen += paragraph.m_lines;
					m_paragraphsOnScreen++;
					paragraphNum--;
				}
			}

			m_scrollOffset = 0;
			m_paragraphAtBottomOnScreen = m_paragraphAtBottom;
			m_paragraphsAdded = 0;
			m_redrawScreen = false;

			//Console.WriteLine("Painted: lines: {0}/{1}, paragraphs: {2}/{3}",
			//	m_linesOnScreen, m_lines, m_paragraphsOnScreen, m_paragraphContainer.Count);
		}

		void ClearLines(Graphics g, int srcLine, int lines)
		{
			g.FillRectangle(m_backgroundBrush, 0, srcLine * m_charHeight,
				this.ClientSize.Width - scrollBar.Width, lines * m_charHeight);
		}

		void CopyLines(Graphics g, int srcLine, int dstLine, int lines)
		{
			//Console.WriteLine("CopyLines {0}-{1} -> {2}", srcLine, srcLine + lines, dstLine);

			IntPtr handle = g.GetHdc();
			BitBlt(handle,
				0, dstLine * m_charHeight,
				this.ClientSize.Width - scrollBar.Width, lines * m_charHeight,
				handle,
				0, srcLine * m_charHeight,
				TernaryRasterOperations.SRCCOPY);
			g.ReleaseHdc();
		}

		void DrawParagraph(Paragraph paragraph, int destLine, Graphics g)
		{
			StringFormat format = StringFormat.GenericTypographic;

			Color fgColor = Color.FromArgb(160, 160, 160);
			Color bgColor = Color.Black;
			bool isLink = false;

			int metaIdx = 0;
			int currentStyleMetaIdx = -1;

			/* for each line the paragraph takes */
			for (int lineNumber = 0; lineNumber < paragraph.m_lines; lineNumber++)
			{
				//Console.WriteLine("new line");

				string lineString = paragraph.m_text.Substring(lineNumber * m_columns, Math.Min(paragraph.m_text.Length - lineNumber * m_columns, m_columns));

				int idx1 = 0;
				int idx2 = 0;

				while (idx1 < lineString.Length)
				{
					if (paragraph.m_meta != null && paragraph.m_meta.Length > 0 && metaIdx < paragraph.m_meta.Length)
					{
						if (paragraph.m_meta[metaIdx].m_index < (lineNumber + 1) * m_columns)
						{
							idx2 = paragraph.m_meta[metaIdx].m_index - lineNumber * m_columns;
							currentStyleMetaIdx = metaIdx;
							metaIdx++;
						}
						else
						{
							idx2 = Math.Min(m_columns, lineString.Length);
						}
					}
					else
					{
						idx2 = Math.Min(m_columns, lineString.Length);
					}

					if (idx2 > idx1)
					{
						if (!bgColor.IsEmpty && bgColor != Color.Black)
						{
							m_defaultBrush.Color = bgColor;
							g.FillRectangle(m_defaultBrush, idx1 * m_charWidth, destLine * m_charHeight + lineNumber * m_charHeight, (idx2 - idx1) * m_charWidth, m_charHeight);
						}

						if (!fgColor.IsEmpty)
						{
							m_defaultBrush.Color = fgColor;
						}
						else
						{
							m_defaultBrush.Color = Color.FromArgb(160, 160, 160);
						}

						Font f;

						if (isLink)
							f = m_underlineFont;
						else
							f = this.Font;

						g.DrawString(lineString.Substring(idx1, idx2 - idx1), f, m_defaultBrush, idx1 * m_charWidth, destLine * m_charHeight + lineNumber * m_charHeight, format);
					}

					if (currentStyleMetaIdx != -1)
					{
						fgColor = paragraph.m_meta[currentStyleMetaIdx].m_fgColor;
						bgColor = paragraph.m_meta[currentStyleMetaIdx].m_bgColor;
						isLink = paragraph.m_meta[currentStyleMetaIdx].m_isLink;
					}

					idx1 = idx2;
				}
			}
		}

		void DrawSelection(Graphics g)
		{
			if (m_selection1ParagraphNum != -1 && m_selection2ParagraphNum != -1)
			{
				int str1Num;
				int str2Num;
				int str1Pos;
				int str2Pos;

				if (m_selection1ParagraphNum < m_selection2ParagraphNum)
				{
					str1Num = m_selection1ParagraphNum;
					str1Pos = m_selection1ParagraphPos;
					str2Num = m_selection2ParagraphNum;
					str2Pos = m_selection2ParagraphPos;
				}
				else if (m_selection1ParagraphNum == m_selection2ParagraphNum)
				{
					str1Num = m_selection1ParagraphNum;
					str2Num = m_selection2ParagraphNum;

					if (m_selection1ParagraphPos < m_selection2ParagraphPos)
					{
						str1Pos = m_selection1ParagraphPos;
						str2Pos = m_selection2ParagraphPos;
					}
					else
					{
						str2Pos = m_selection1ParagraphPos;
						str1Pos = m_selection2ParagraphPos;
					}
				}
				else
				{
					str2Num = m_selection1ParagraphNum;
					str2Pos = m_selection1ParagraphPos;
					str1Num = m_selection2ParagraphNum;
					str1Pos = m_selection2ParagraphPos;
				}

				int line1, line2, column1, column2;

				MapStringToScreen(str1Num, str1Pos, out line1, out column1);
				MapStringToScreen(str2Num, str2Pos, out line2, out column2);

				if (line1 == -1 && line2 == -1)
				{
					return;
				}

				if (line1 == line2)
				{
					BltInvert(g, column1 * m_charWidth, line1 * m_charHeight,
						(column2 - column1) * m_charWidth, m_charHeight);
				}
				else
				{
					RectangleF r = new RectangleF();
					r.X = column1 * m_charWidth;
					r.Y = line1 * m_charHeight;
					r.Width = this.ClientSize.Width - scrollBar.Width;
					r.Height = m_charHeight;
					
					if (line1 >= 0)
					{
						BltInvert(g, column1 * m_charWidth, line1 * m_charHeight,
							(m_columns - column1) * m_charWidth, m_charHeight);
					}

					if (line1 + 1 < line2)
					{
						
						BltInvert(g, 0, Math.Max(line1 + 1, 0) * m_charHeight,
							m_columns * m_charWidth, (Math.Min((line2 - line1 - 1), m_lines)) * m_charHeight);
					}

					if (line2 < m_lines)
					{
						BltInvert(g, 0, line2 * m_charHeight,
							column2 * m_charWidth, m_charHeight);
					}
				}

			}
		}

		private void MapStringToScreen(int stringNum, int stringPos, out int line, out int column)
		{
			if (stringNum < m_paragraphAtBottomOnScreen + 1 - m_paragraphsOnScreen)
			{
				line = -1;
				column = 0;
				return;
			}

			if (stringNum >= m_paragraphAtBottomOnScreen + 1)
			{
				line = m_lines;
				column = 0;
				return;
			}

			line = -(m_linesOnScreen - m_lines);

			int i;
			for (i = m_paragraphAtBottomOnScreen + 1 - m_paragraphsOnScreen; i < stringNum; i++)
			{
				line += m_paragraphContainer[i].m_text.Length / (m_columns + 1) + 1;
			}

			line += stringPos / (m_columns + 1);

			column = stringPos % m_columns;
		}

		private void BltInvert(Graphics g, RectangleF r)
		{
			BltInvert(g, r.X, r.Y, r.Width, r.Height);
		}

		private void BltInvert(Graphics g, float x, float y, float w, float h)
		{
			if ((int)(x + w + 0.5) != (int)(x + 0.5) + (int)(w + 0.5))
			{
				w += 1;
			}

			if ((int)(y + h + 0.5) != (int)(y + 0.5) + (int)(h + 0.5))
			{
				h += 1;
			}

			int ix = (int)(x + 0.5);
			int iy = (int)(y + 0.5);
			int iw = (int)(w + 0.5);
			int ih = (int)(h + 0.5);

			IntPtr handle = g.GetHdc();
			BitBlt(handle, ix, iy, iw, ih, handle, ix, iy, TernaryRasterOperations.DSTINVERT);
			g.ReleaseHdc();

			//g.CopyFromScreen(ix, iy, ix, iy, new Size(iw, ih), CopyPixelOperation.DestinationInvert);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				int line;
				int column;
				int paragraphPos;

				m_clickPos1 = new Point(e.X, e.Y);

				int paragraphNum = GetStringNumber(e.X, e.Y, out line, out column, out paragraphPos);

				if (paragraphNum != -1)
				{
					//Console.WriteLine("down on {0}, {1}/{2}, paragraph {3}, pos {4}", 
					//	line, column, m_columns, paragraphNum, paragraphPos);

					m_selection1ParagraphNum = paragraphNum;
					m_selection1ParagraphPos = paragraphPos;

					m_selectionScrollLock = m_scrollLock;
					m_scrollLock = true;
				}
			}

			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (m_selection1ParagraphNum != -1 && e.Button == MouseButtons.Left)
			{
				int paragraphNum;
				int paragraphPos;

				if (e.Y < 0)
				{
					ScrollUp(1);
					paragraphNum = m_paragraphAtBottomOnScreen + 1 - m_paragraphsOnScreen;
					paragraphPos = 0;
				}
				else if (e.Y > this.Height)
				{
					//Console.WriteLine("scrolldown");
					ScrollDown(1);

					paragraphNum = m_paragraphAtBottomOnScreen;
					paragraphPos = m_paragraphContainer[m_selection2ParagraphNum].m_text.Length;
				}
				else
				{
					int line;
					int column;

					paragraphNum = GetStringNumber(e.X, e.Y, out line, out column, out paragraphPos);

					if (paragraphNum == -1)
					{
						paragraphNum = m_selection2ParagraphNum;
						paragraphPos = m_selection2ParagraphPos;
					}

					//Console.WriteLine("move on {0}, {1}, string {2}, pos {3}", line, column, paragraphNum, paragraphPos);
				}

				if (m_selection2ParagraphNum != paragraphNum || m_selection2ParagraphPos != paragraphPos)
				{
					m_selection2ParagraphNum = paragraphNum;
					m_selection2ParagraphPos = paragraphPos;

					this.Invalidate();
				}
			}

			//base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (m_selection1ParagraphNum != -1 && e.Button == MouseButtons.Left)
			{
				int line;
				int column;
				int paragraphPos;

				m_scrollLock = m_selectionScrollLock;
/*				m_paragraphAtBottom = m_paragraphContainer.Count - 1;
				UpdateScrollBar();
				m_redrawScreen = true;
				this.Invalidate();
*/
				if (m_clickPos1 == new Point(e.X, e.Y))
				{
					OnMouseClick(e);
					m_selection1ParagraphNum = -1;
					m_selection2ParagraphNum = -1;
					return;
				}

				int paragraphNum = GetStringNumber(e.X, e.Y, out line, out column, out paragraphPos);

				if (paragraphNum != -1)
				{
					//Console.WriteLine("up on {0}, {1}, string {2}, pos {3}", line, column, paragraphNum, paragraphPos);
				}

				if (paragraphNum != -1)
				{
					m_selection2ParagraphNum = paragraphNum;
					m_selection2ParagraphPos = paragraphPos;
				}

				System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

				int str1Num;
				int str2Num;
				int str1Pos;
				int str2Pos;

				if (m_selection1ParagraphNum < m_selection2ParagraphNum)
				{
					str1Num = m_selection1ParagraphNum;
					str1Pos = m_selection1ParagraphPos;
					str2Num = m_selection2ParagraphNum;
					str2Pos = m_selection2ParagraphPos;
				}
				else if (m_selection1ParagraphNum == m_selection2ParagraphNum)
				{
					str1Num = m_selection1ParagraphNum;
					str2Num = m_selection2ParagraphNum;

					if (m_selection1ParagraphPos < m_selection2ParagraphPos)
					{
						str1Pos = m_selection1ParagraphPos;
						str2Pos = m_selection2ParagraphPos;
					}
					else
					{
						str2Pos = m_selection1ParagraphPos;
						str1Pos = m_selection2ParagraphPos;
					}
				}
				else
				{
					str2Num = m_selection1ParagraphNum;
					str2Pos = m_selection1ParagraphPos;
					str1Num = m_selection2ParagraphNum;
					str1Pos = m_selection2ParagraphPos;
				}

				if (str1Num == str2Num)
				{
					strBuilder.Append(m_paragraphContainer[str1Num].m_text.Substring(str1Pos, str2Pos - str1Pos));
				}
				else
				{
					strBuilder.Append(m_paragraphContainer[str1Num].m_text.Substring(str1Pos));
					strBuilder.Append("\r\n");

					for (int i = str1Num + 1; i < str2Num; i++)
					{
						strBuilder.Append(m_paragraphContainer[i].m_text);
						strBuilder.Append("\r\n");
					}

					strBuilder.Append(m_paragraphContainer[str2Num].m_text.Substring(0, str2Pos));
				}

				string str = strBuilder.ToString();

				//Console.WriteLine("Selected: '{0}'", str);

				Clipboard.SetDataObject(str);
			}

			//m_redrawScreen = true;
			this.Invalidate();

			//base.OnMouseUp(e);
		}

		protected new void OnMouseClick(MouseEventArgs e)
		{
			int line;
			int column;
			int stringPos;

			int stringNum = GetStringNumber(e.X, e.Y, out line, out column, out stringPos);

			if (stringNum != -1)
			{
				//Console.WriteLine("click on {0}, {1}, string {2}, pos {3}", line, column, stringNum, stringPos);
			}

			if (stringNum == -1)
			{
				return;
			}

			Paragraph p = m_paragraphContainer[stringNum];
			int metaIndex = -1;
			for (int i = 0; i < p.m_meta.Length; i++)
			{
				if (p.m_meta[i].m_index <= stringPos)
					continue;

				metaIndex = i - 1;

				break;
			}

			if (metaIndex >= 0 && p.m_meta[metaIndex].m_isLink)
			{
				int start = p.m_meta[metaIndex].m_index;
				int len = p.m_meta[metaIndex + 1].m_index - p.m_meta[metaIndex].m_index;
				string link = p.m_text.Substring(start, len);

				if (LinkClicked != null)
					LinkClicked(link);
			}
		}

		// Returns paragraph number
		private int GetStringNumber(int x, int y, out int line, out int column, out int paragraphPos)
		{
			line = y / m_charHeight;
			column = (int)((x + m_charWidth / 2) / m_charWidth);
			paragraphPos = 0;

			if (m_paragraphContainer == null || m_paragraphContainer.Count == 0)
			{
				return -1;
			}

			if (line < m_lines - m_linesOnScreen)
			{
				paragraphPos = 0;
				return m_paragraphAtBottomOnScreen + 1 - m_paragraphsOnScreen;
			}

			if (line >= m_lines)
			{
				return -1;
			}

			if (column < 0)
			{
				column = 0;
			}

			if (column >= m_columns)
			{
				column = m_columns + 1;
			}

			int paragraphNum = 0;
			int linesCounted = 0;

			for (paragraphNum = m_paragraphAtBottomOnScreen; paragraphNum >= 0 && linesCounted < m_lines; paragraphNum--)
			{
				Paragraph paragraph = m_paragraphContainer[paragraphNum];

				string originalString = paragraph.m_text;

				int totalLines = paragraph.m_lines;

				linesCounted += totalLines;

				if (linesCounted > m_lines - line - 1)
				{
					int lineOnString = linesCounted - (m_lines - line);

					paragraphPos = lineOnString * m_columns + column;
					if (paragraphPos > originalString.Length)
					{
						paragraphPos = originalString.Length;
					}

					return paragraphNum;
				}
			}

			return -1;
		}

		void OnScrollBarScroll(object sender, ScrollEventArgs e)
		{
			//Console.WriteLine("new {0}, old {1}, type {2}", e.NewValue, e.OldValue, e.Type);

			if (e.NewValue == e.OldValue)
				return;

			m_paragraphAtBottom = e.NewValue + scrollBar.LargeChange - 1;
			m_scrollOffset += (e.NewValue - e.OldValue);

			if (m_paragraphAtBottom != m_paragraphContainer.Count - 1)
				m_scrollLock = true;
			else
				m_scrollLock = false;

			//m_redrawScreen = true;
			this.Invalidate();
		}

		public void ScrollPageUp()
		{
			m_paragraphAtBottom -= m_paragraphsOnScreen;
			if (m_paragraphAtBottom < m_paragraphsOnScreen)
			{
				m_paragraphAtBottom = m_paragraphsOnScreen - 1;
			}

			if (m_paragraphAtBottom != m_paragraphContainer.Count - 1)
				m_scrollLock = true;

			UpdateScrollBar();
			m_redrawScreen = true;
			this.Invalidate();
		}

		public void ScrollPageDown()
		{
			// TODO: m_paragraphsOnScreen is not really the amount of paragraphs we should move down
			m_paragraphAtBottom += m_paragraphsOnScreen;
			if (m_paragraphAtBottom >= m_paragraphContainer.Count)
			{
				m_paragraphAtBottom = m_paragraphContainer.Count - 1;
			}

			if (m_paragraphAtBottom == m_paragraphContainer.Count - 1)
				m_scrollLock = false;

			UpdateScrollBar();
			m_redrawScreen = true;
			this.Invalidate();
		}

		public void ScrollUp(int paragraphs)
		{
			if (m_paragraphAtBottom - paragraphs < m_paragraphsOnScreen - 1)
			{
				paragraphs -= m_paragraphsOnScreen - (m_paragraphAtBottom - paragraphs) - 1;
			}

			if (paragraphs == 0)
				return;

			m_paragraphAtBottom -= paragraphs;
			m_scrollOffset -= paragraphs;

			if (m_paragraphAtBottom != m_paragraphContainer.Count - 1)
				m_scrollLock = true;

			UpdateScrollBar();
			this.Invalidate();
		}

		public void ScrollDown(int paragraphs)
		{
			if (m_paragraphAtBottom + paragraphs >= m_paragraphContainer.Count)
			{
				paragraphs -= (m_paragraphAtBottom + paragraphs) - m_paragraphContainer.Count + 1;
			}

			if (paragraphs == 0)
				return;

			m_paragraphAtBottom += paragraphs;
			m_scrollOffset += paragraphs;

			if (m_paragraphAtBottom == m_paragraphContainer.Count - 1)
				m_scrollLock = false;

			UpdateScrollBar();
			this.Invalidate();
		}
	}
}