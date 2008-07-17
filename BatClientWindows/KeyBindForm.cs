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
	public partial class KeyBindForm : Form
	{
		Keys m_currentKey = Keys.None;
		KeyManager m_manager;

		public KeyBindForm()
		{
			InitializeComponent();
			m_manager = PythonInterface.KeyManager;

			KeyBinding[] arr = m_manager.GetKeyBindings();
			foreach (KeyBinding binding in arr)
			{
				ListViewItem item = KeyBindingToListViewItem(binding);
				listView.Items.Add(item);
			}
		}

		ListViewItem KeyBindingToListViewItem(KeyBinding binding)
		{
			ListViewItem item = new ListViewItem(new string[3]);
			item.Tag = binding;
			UpdateListViewItem(item);
			return item;
		}

		void UpdateListViewItem(ListViewItem item)
		{
			if (item.Tag is ScriptedKeyBinding)
			{
				ScriptedKeyBinding binding = (ScriptedKeyBinding)item.Tag;
				item.SubItems[0].Text = KeyCodeToString(binding.Key);
				item.SubItems[1].Text = binding.BindingType.ToString();
				item.SubItems[2].Text = binding.Text;
			}
			else
			{
				KeyBinding binding = (KeyBinding)item.Tag;
				item.SubItems[0].Text = KeyCodeToString(binding.Key);
				item.SubItems[1].Text = "Function";
				item.SubItems[2].Text = "<function binding>";
			}
		}

		private void keyTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			m_currentKey = Keys.None;
			e.SuppressKeyPress = true;
			e.Handled = true;
			keyTextBox.Text = "";
			
			if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu)
				return;

			string str = KeyCodeToString(e.KeyData);

			keyTextBox.Text = str;

			m_currentKey = e.KeyData;
		}

		string KeyCodeToString(Keys key)
		{
			string str = "";

			if (key == Keys.ShiftKey || key == Keys.ControlKey || key == Keys.Menu)
				return "";

			if ((key & Keys.Shift) != 0)
				str += "Shift + ";
			if ((key & Keys.Alt) != 0)
				str += "Alt + ";
			if ((key & Keys.Control) != 0)
				str += "Ctrl + ";

			str += (key & Keys.KeyCode).ToString();

			return str;
		}

		private void listView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count == 0)
			{
				keyTextBox.Clear();
				m_currentKey = Keys.None;
				styleRadioButton1.Checked = false;
				actionTextBox.Clear();
				return;
			}

			ListViewItem item = listView.SelectedItems[0];

			if (item.Tag is ScriptedKeyBinding)
			{
				ScriptedKeyBinding binding = (ScriptedKeyBinding)item.Tag;

				KeyBindingType type = binding.BindingType;
				if (type == KeyBindingType.Send)
					styleRadioButton1.Checked = true;
				else if (type == KeyBindingType.Script)
					styleRadioButton2.Checked = true;

				keyTextBox.Text = KeyCodeToString(binding.Key);
				m_currentKey = binding.Key;
				actionTextBox.Text = binding.Text;

				assignButton.Enabled = true;
			}
			else
			{
				keyTextBox.Clear();
				m_currentKey = Keys.None;
				actionTextBox.Clear();

				assignButton.Enabled = false;
			}
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			ScriptedKeyBinding binding = new ScriptedKeyBinding();
			m_manager.AddBinding(binding);
			ListViewItem item = KeyBindingToListViewItem(binding);
			listView.Items.Add(item);
			item.EnsureVisible();
			item.Selected = true;

			keyTextBox.Focus();
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count == 0)
				return;

			ListViewItem item = listView.SelectedItems[0];

			KeyBinding binding = (KeyBinding)item.Tag;

			item.Remove();

			m_manager.RemoveBinding(binding);
		}

		private void assignButton_Click(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count == 0)
				return;

			ListViewItem item = listView.SelectedItems[0];
			ScriptedKeyBinding binding = (ScriptedKeyBinding)item.Tag;

			if (m_currentKey != binding.Key && m_manager.IsKeyBound(m_currentKey))
			{
				MessageBox.Show("Key already bound");
				return;
			}

			binding.Key = m_currentKey;
			binding.BindingType = styleRadioButton1.Checked ? KeyBindingType.Send : KeyBindingType.Script;
			binding.Text = actionTextBox.Text;
			binding.Save = true;

			UpdateListViewItem(item);

			item.Selected = false;
			item.Selected = item.Focused = true; // updates the controls

			try
			{
				binding.Compile();
			}
			catch (Exception exc)
			{
				if (exc.Source == "IronPython")
				{
					string str;
					if (exc is IronPython.Runtime.Exceptions.PythonSyntaxErrorException)
					{
						IronPython.Runtime.Exceptions.PythonSyntaxErrorException ee =
							(IronPython.Runtime.Exceptions.PythonSyntaxErrorException)exc;
						str = String.Format("{0}({1},{2}): {3}", ee.FileName, ee.Line, ee.Column, ee.Message);
					}
					else
					{
						str = exc.Message;
					}

					MessageBox.Show(this, str, "Failed to compile keybinding");
				}
				else
				{
					throw;
				}
			}
		}
	}
}