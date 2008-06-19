using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BatMud.BatClientBase;

namespace BatMud.BatClientWindows
{
	public partial class PropertyForm : Form
	{
		public PropertyForm()
		{
			InitializeComponent();

			foreach (Trigger t in PythonInterface.TriggerManager.Triggers)
			{
				ListViewItem item = new ListViewItem(new string[] { "", t.TriggerID.ToString(), t.Pattern });
				item.Tag = t;
				if (t is Hilite)
					item.SubItems[0].Text = "Hilite";
				else
					item.SubItems[0].Text = "Trigger";
				listView.Items.Add(item);
			}

			foreach (KeyBinding binding in PythonInterface.KeyManager.GetKeyBindings())
			{
				ListViewItem item = new ListViewItem(new string[] { "", binding.Key.ToString() });
				item.Tag = binding;
				item.SubItems[0].Text = "Key";
				listView.Items.Add(item);
			}
		}

		private void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			object[] arr = new object[listView.SelectedItems.Count];
			for (int i = 0; i < listView.SelectedItems.Count; i++)
				arr[i] = listView.SelectedItems[i].Tag;
			propertyGrid.SelectedObjects = arr;
		}
	}
}