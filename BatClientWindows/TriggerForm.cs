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
	public partial class TriggerForm : Form
	{
		public TriggerForm()
		{
			InitializeComponent();

			Trigger[] triggers = PythonInterface.TriggerManager.Triggers;
			foreach (Trigger t in triggers)
			{
				if (!(t is ScriptedTrigger))
					continue;

				ScriptedTrigger trigger = (ScriptedTrigger)t;

				ListViewItem item = TriggerToListViewItem(trigger);
				listView.Items.Add(item);
			}
		}

		ListViewItem TriggerToListViewItem(ScriptedTrigger trigger)
		{
			ListViewItem item = new ListViewItem(new string[5]);
			item.Tag = trigger;
			UpdateListViewItem(item);
			return item;
		}

		void UpdateListViewItem(ListViewItem item)
		{
			ScriptedTrigger trigger = (ScriptedTrigger)item.Tag;
			string flags = "";
			if (trigger.IgnoreCase)
				flags += "I";

			item.SubItems[0].Text = trigger.TriggerID.ToString();
			item.SubItems[1].Text = trigger.Pattern;
			item.SubItems[2].Text = trigger.TriggerType.ToString();
			item.SubItems[3].Text = trigger.Script;
			item.SubItems[4].Text = flags;
		}

		private void listView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count == 0)
			{
				patternTextBox.Text = "";
				ignoreCaseCheckBox.Checked = false;
				typeComboBox.Text = null;
				scriptTextBox.Text = "";
				return;
			}

			ListViewItem item = listView.SelectedItems[0];
			
			ScriptedTrigger trigger = (ScriptedTrigger)item.Tag;

			patternTextBox.Text = trigger.Pattern;
			ignoreCaseCheckBox.Checked = trigger.IgnoreCase;
			if (trigger.TriggerType == TriggerType.Send)
				typeComboBox.SelectedIndex = 0;
			else if (trigger.TriggerType == TriggerType.Replace)
				typeComboBox.SelectedIndex = 1;
			else
				typeComboBox.SelectedIndex = 2;
			scriptTextBox.Text = trigger.Script;
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			ScriptedTrigger trigger = new ScriptedTrigger("", false, TriggerType.Send, "");
			PythonInterface.TriggerManager.AddTrigger(trigger);

			ListViewItem item = TriggerToListViewItem(trigger);
			listView.Items.Add(item);

			item.Selected = item.Focused = true;
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count == 0)
			{
				return;
			}

			ListViewItem item = listView.SelectedItems[0];

			ScriptedTrigger trigger = (ScriptedTrigger)item.Tag;

			PythonInterface.TriggerManager.RemoveTrigger(trigger);

			int idx = item.Index;

			item.Remove();

			if (idx >= listView.Items.Count)
				idx--;

			if (idx < 0)
				idx = 0;

			if (idx < listView.Items.Count)
			{
				ListViewItem newItem = listView.Items[idx];
				newItem.Focused = newItem.Selected = true;
			}
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count == 0)
			{
				return;
			}

			ListViewItem item = listView.SelectedItems[0];

			ScriptedTrigger trigger = (ScriptedTrigger)item.Tag;

			try
			{
				trigger.Pattern = patternTextBox.Text;
			}
			catch (ArgumentException exc)
			{
				MessageBox.Show(this, "Error in pattern: " + exc.Message, "Error in pattern");
				return;
			}

			trigger.IgnoreCase = ignoreCaseCheckBox.Checked;
			if (typeComboBox.SelectedIndex == 0)
				trigger.TriggerType = TriggerType.Send;
			else if (typeComboBox.SelectedIndex == 1)
				trigger.TriggerType = TriggerType.Replace;
			else
				trigger.TriggerType = TriggerType.Script;
			trigger.Script = scriptTextBox.Text;

			UpdateListViewItem(item);
			item.Selected = false;
			item.Selected = item.Focused = true; // updates the controls

			try
			{
				trigger.Compile();
			}
			catch (Exception exc)
			{
				MessageBox.Show(this, "Error in script: " + exc.Message, "Error in script");
			}
		}

	}
}
