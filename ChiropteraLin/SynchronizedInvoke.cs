using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Chiroptera.Lin
{
	class SynchronizedInvoke
	{
		Queue<Delegate> m_delegateQueue = new Queue<Delegate>();
		Queue<object[]> m_argumentQueue = new Queue<object[]>();

		public void BeginInvoke(Delegate del, object[] args)
		{
			lock(this)
			{
				m_delegateQueue.Enqueue(del);
				m_argumentQueue.Enqueue(args);
			}

			ClientCore.s_clientCore.m_netPipe.Writing.WriteByte(0);
			ClientCore.s_clientCore.m_netPipe.Writing.Flush();
		}

		public void DispatchInvokes()
		{
			lock(this)
			{
				while (m_delegateQueue.Count > 0)
				{
					Delegate del = m_delegateQueue.Dequeue();
					object[] args = m_argumentQueue.Dequeue();
					
					del.DynamicInvoke(args);
				}
			}
		}
	}
}
