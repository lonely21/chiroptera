using System;
using System.Collections.Generic;
using Chiroptera.Base;

namespace Chiroptera.Lin
{
	public class ParagraphContainer
	{
		private List<Paragraph> m_paragraphList;
		private int m_maxSize = 1000;

		public delegate void ParagraphAddedDelegate(bool historyFull);
		public event ParagraphAddedDelegate paragraphAddedEvent = null;

		int m_columns = 80;
		int m_totalLines;

		public ParagraphContainer()
		{
			m_paragraphList = new List<Paragraph>();
		}

		public Paragraph Add(string text)
		{
			Paragraph p = new Paragraph();
			p.m_text = text;
			p.m_meta = new ColorMessage.MetaData[0];
			Add(p);
			return p;
		}

		int GetLinesForParagraph(Paragraph paragraph)
		{
			int l = paragraph.m_text.Length / m_columns;
			if (paragraph.m_text.Length > 0 && paragraph.m_text.Length % m_columns == 0)
				l -= 1;
			l += 1;
			return l;
		}
		
		public Paragraph Add(Paragraph paragraph)
		{
			m_paragraphList.Add(paragraph);
			int lines = GetLinesForParagraph(paragraph);
			paragraph.m_lines = lines;
			m_totalLines += lines;

			bool historyFull = false;

			if(m_paragraphList.Count > m_maxSize)
			{
				m_totalLines -= m_paragraphList[0].m_lines;
				m_paragraphList.RemoveAt(0);
				historyFull = true;
			}

			if(paragraphAddedEvent != null)
				paragraphAddedEvent(historyFull);

			return paragraph;
		}

		public Paragraph this [int index]
		{
			get 
			{
				if (index < 0 || index >= m_paragraphList.Count)
					throw new Exception("Index out of bounds");

				return m_paragraphList[index];
			}
		}

		public int Count 
		{
			get { return m_paragraphList.Count; }
		}
		
		public int TotalLines
		{
			get { return m_totalLines; }
		}

		public void SetColumns(int columns)
		{
			m_columns = columns;
			int l = 0;

			for(int i = 0; i < m_paragraphList.Count; i++)
			{
				// 99 / (49+1) + 1
				int lines = GetLinesForParagraph(m_paragraphList[i]);
				m_paragraphList[i].m_lines = lines;
				l += lines;
			}
			
			m_totalLines = l;
		}
	}
}
