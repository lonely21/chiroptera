using System;
using System.Collections.Generic;
using System.Text;

namespace BatMud.BatClientBase
{
	public class HistoryCollection : System.Collections.Specialized.StringCollection
	{
		public HistoryCollection()
		{
		}

		public HistoryCollection(string[] arr)
		{
			base.AddRange(arr);
		}

		public string[] ToArray()
		{
			string[] arr = new string[base.Count];
			base.CopyTo(arr, 0);
			return arr;
		}
	}

	public class TriggerCollection : System.Collections.CollectionBase
	{
		public int Add(Trigger trigger)
		{
			return List.Add(trigger);
		}

		public Trigger this[int idx]
		{
			get { return (Trigger)List[idx]; }
			set { List[idx] = value; }
		}
	}

	public class KeyBindingCollection : System.Collections.CollectionBase
	{
		public int Add(KeyBinding binding)
		{
			return List.Add(binding);
		}

		public KeyBinding this[int idx]
		{
			get { return (KeyBinding)List[idx]; }
			set { List[idx] = value; }
		}
	}

}
