using System;
using System.Collections.Generic;

namespace BatMud.BatClientWindows
{
	public class ParagraphContainer
	{
		private List<Paragraph> m_paragraphList;
		private int m_maxSize = 1000;

		public delegate void ParagraphAddedDelegate(bool historyFull);
		public event ParagraphAddedDelegate paragraphAddedEvent = null;

		int m_columns = 80;

		public ParagraphContainer()
		{
			m_paragraphList = new List<Paragraph>();
		}

		public Paragraph Add(string text)
		{
			Paragraph p = new Paragraph();
			p.m_text = text;
			p.m_meta = new Paragraph.MetaData[0];
			Add(p);
			return p;
		}

		int GetLinesForParagraph(Paragraph paragraph)
		{
			int l = paragraph.m_text.Length / m_columns;
			if(paragraph.m_text.Length % m_columns == 0)
				l -= 1;
			l += 1;
			return l;
		}
		
		public Paragraph Add(Paragraph paragraph)
		{
			m_paragraphList.Add(paragraph);
			paragraph.m_lines = GetLinesForParagraph(paragraph);

			bool historyFull = false;

			if(m_paragraphList.Count > m_maxSize)
			{
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

		public void SetColumns(int columns)
		{
			m_columns = columns;

			for(int i = 0; i < m_paragraphList.Count; i++)
			{
				int lines = GetLinesForParagraph(m_paragraphList[i]);
				m_paragraphList[i].m_lines = lines;
			}
		}
	}
}
