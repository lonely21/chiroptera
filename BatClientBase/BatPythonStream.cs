using System;
using System.Collections.Generic;
using System.Text;
using BatMud.BatClientBase;

namespace BatMud.BatClientBase
{
	public class BatPythonStream : System.IO.Stream
	{
		StringBuilder m_stringBuilder = new StringBuilder();

		public BatPythonStream()
		{
		}

		public override bool CanRead
		{
			get { return false; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override long Position
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void SetLength(long value)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			string str = Encoding.Default.GetString(buffer, offset, count);

			m_stringBuilder.Append(str);

			string[] lines = m_stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			if (lines.Length > 1)
			{
				for (int i = 0; i < lines.Length - 1; i++)
				{
					string line = lines[i];
					BatConsole.WriteLine("> " + line);
				}

				m_stringBuilder = new StringBuilder(lines[lines.Length - 1]);
			}
		}
	}
}
