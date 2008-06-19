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
	public partial class HiliteForm : Form
	{
		public HiliteForm()
		{
			InitializeComponent();

			Hilite[] hilites = PythonInterface.HiliteManager.GetHilites();
			foreach (Hilite hilite in hilites)
			{
				ListViewItem item = HiliteToListViewItem(hilite);
				listView.Items.Add(item);
			}
		}

		ListViewItem HiliteToListViewItem(Hilite hilite)
		{
			ListViewItem item = new ListViewItem(new string[5]);
			item.Tag = hilite;
			UpdateListViewItem(item);
			return item;
		}

		void UpdateListViewItem(ListViewItem item)
		{
			Hilite hilite = (Hilite)item.Tag;
			item.SubItems[0].Text = hilite.Pattern;
			item.SubItems[1].Text = hilite.FgColor.Name;
			item.SubItems[2].Text = hilite.BgColor.Name;
			item.SubItems[3].Text = hilite.IgnoreCase.ToString();
			item.SubItems[4].Text = hilite.HiliteLine.ToString();
		}

		private void fgButton_Click(object sender, EventArgs e)
		{
			colorDialog.Color = fgButton.BackColor;
			if(colorDialog.ShowDialog(this) == DialogResult.OK)
				fgButton.BackColor = colorDialog.Color;
		}

		private void bgButton_Click(object sender, EventArgs e)
		{
			colorDialog.Color = bgButton.BackColor;
			if (colorDialog.ShowDialog(this) == DialogResult.OK)
				bgButton.BackColor = colorDialog.Color;
		}

		private void listView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count == 0)
			{
				patternTextBox.Text = "";
				ignoreCaseCheckBox.Checked = false;
				hiliteLineCheckBox.Checked = false;
				fgCheckBox.Checked = true;
				fgButton.Enabled = false;
				bgCheckBox.Checked = true;
				bgButton.Enabled = false;
				fgButton.BackColor = Color.Empty;
				bgButton.BackColor = Color.Empty;
				return;
			}

			ListViewItem item = listView.SelectedItems[0];
			
			Hilite hilite = (Hilite)item.Tag;

			patternTextBox.Text = hilite.Pattern;
			ignoreCaseCheckBox.Checked = hilite.IgnoreCase;
			fgButton.BackColor = hilite.FgColor;
			bgButton.BackColor = hilite.BgColor;
			hiliteLineCheckBox.Checked = hilite.HiliteLine;

			if (hilite.FgColor == Color.Empty)
			{
				fgCheckBox.Checked = true;
				fgButton.Enabled = false;
			}
			else
			{
				fgCheckBox.Checked = false;
				fgButton.Enabled = true;
			}

			if (hilite.BgColor == Color.Empty)
			{
				bgCheckBox.Checked = true;
				bgButton.Enabled = false;
			}
			else
			{
				bgCheckBox.Checked = false;
				bgButton.Enabled = true;
			}
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			Hilite hilite = new Hilite("", false, Color.Empty, Color.Empty, false);
			PythonInterface.HiliteManager.AddHilite(hilite);

			ListViewItem item = HiliteToListViewItem(hilite);
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

			Hilite hilite = (Hilite)item.Tag;

			PythonInterface.HiliteManager.RemoveHilite(hilite);

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

			Hilite hilite = (Hilite)item.Tag;

			try
			{
				hilite.Pattern = patternTextBox.Text;
			}
			catch (ArgumentException exc)
			{
				MessageBox.Show(this, "Error in pattern: " + exc.Message, "Error in pattern");
				return;
			}

			hilite.IgnoreCase = ignoreCaseCheckBox.Checked;
			if (fgCheckBox.Checked)
				hilite.FgColor = Color.Empty;
			else
				hilite.FgColor = fgButton.BackColor;
			if (bgCheckBox.Checked)
				hilite.BgColor = Color.Empty;
			else
				hilite.BgColor = bgButton.BackColor;
			hilite.HiliteLine = hiliteLineCheckBox.Checked;

			UpdateListViewItem(item);
			item.Selected = false;
			item.Selected = item.Focused = true; // updates the controls
		}

		private void fgCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (fgCheckBox.Checked)
				fgButton.Enabled = false;
			else
				fgButton.Enabled = true;
		}

		private void bgCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (bgCheckBox.Checked)
				bgButton.Enabled = false;
			else
				bgButton.Enabled = true;
		}
	}
}
