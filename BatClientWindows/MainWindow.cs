using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BatMud.BatClientBase;

namespace BatMud.BatClientWindows
{
	public partial class MainWindow : Form
	{
		ClientCore m_clientCore;

		public MainWindow(ClientCore clientCore)
		{
			InitializeComponent();

			m_clientCore = clientCore;
		}

		private void MainWindow_Load(object sender, EventArgs e)
		{
			promptTextBox.Prompt = "";
			promptTextBox.MouseWheel += promptTextBox_MouseWheel;
			HistoryCollection his = Properties.Settings.Default.CommandHistory;
			if (his != null)
			{
				promptTextBox.History = his.ToArray();
			}
			
			this.Size = Properties.Settings.Default.MainWindowSize;
			this.Location = Properties.Settings.Default.MainWindowLocation;
			this.WindowState = Properties.Settings.Default.MainWindowState;
		}

		private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			Properties.Settings.Default.CommandHistory = new HistoryCollection(promptTextBox.History);

			if (this.WindowState == FormWindowState.Minimized)
				Properties.Settings.Default.MainWindowState = FormWindowState.Normal;
			else
				Properties.Settings.Default.MainWindowState = this.WindowState;

			if (this.WindowState == FormWindowState.Normal)
			{
				Properties.Settings.Default.MainWindowSize = this.Size;
				Properties.Settings.Default.MainWindowLocation = this.Location;
			}
			else
			{
				Properties.Settings.Default.MainWindowSize = this.RestoreBounds.Size;
				Properties.Settings.Default.MainWindowLocation = this.RestoreBounds.Location;
			}
		}

		public TextViewControl TextView
		{
			get { return textView; }
		}

		public PromptTextBox PromptTextBox
		{
			get { return promptTextBox; }
		}


		private void textView_Enter(object sender, EventArgs e)
		{
			promptTextBox.Focus();
		}

		private void textView_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				promptTextBox.Paste();
			}
		}

		private void textView_LinkClicked(string link)
		{
			try
			{
				// TODO: make this a bit more secure
				System.Diagnostics.Process.Start(link);
			}
			catch (Exception)
			{
				MessageBox.Show("Failed to open browser for link " + link, "Failed to open link");
			}
		}



		private void promptTextBox_textEntered(string str)
		{
			m_clientCore.HandleInput(str);
		}

		private void promptTextBox_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0)
			{
				textView.ScrollPageUp();
			}
			else
			{
				textView.ScrollPageDown();
			}
		}

		private void promptTextBox_rawKeyUp(object sender, KeyEventArgs e)
		{
			m_clientCore.HandleRawKeyUp(e);

			if (e.Handled == true)
			{
				return;
			}
		}

		private void promptTextBox_rawKeyDown(object sender, KeyEventArgs e)
		{
			m_clientCore.HandleRawKeyDown(e);

			if (e.Handled == true)
			{
				return;
			}

			else if (e.KeyCode == Keys.PageUp)
			{
				textView.ScrollPageUp();

				e.Handled = true;
			}
			else if (e.KeyCode == Keys.PageDown)
			{
				textView.ScrollPageDown();

				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Up && e.Control)
			{
				textView.ScrollUp(1);

				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Down && e.Control)
			{
				textView.ScrollDown(1);

				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Scroll)
			{
				//textView.ScrollLock = !textView.ScrollLock;

				e.Handled = true;
			}
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void batMUDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			m_clientCore.Connect("batmud.bat.org", 23);
		}

		private void hCBatMUDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			m_clientCore.Connect("hcbat.bat.org", 23);
		}

		private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			m_clientCore.Disconnect();
		}

		private void fontToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FontDialog dialog = new FontDialog();
			dialog.Font = textView.Font;
			dialog.FixedPitchOnly = true;
			dialog.ShowEffects = false;
			dialog.AllowScriptChange = false;

			if (dialog.ShowDialog() != DialogResult.Cancel)
			{
				textView.Font = dialog.Font;
				promptTextBox.Font = dialog.Font;
			}
		}

		private void quickHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(Application.StartupPath + "\\Help.html");
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutDialog dlg = new AboutDialog();
			dlg.ShowDialog(this);
		}

		private void keyBindingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			KeyBindForm f = new KeyBindForm();
			f.ShowDialog(this);
			Properties.Settings.Default.KeyBindings = PythonInterface.KeyManager.GetSavedKeyBindings();
			Properties.Settings.Default.Save();
		}

		private void hilitesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HiliteForm f = new HiliteForm();
			f.ShowDialog(this);
			Properties.Settings.Default.Triggers = PythonInterface.TriggerManager.GetSavedTriggers();
			Properties.Settings.Default.Save();
		}

		private void triggersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TriggerForm f = new TriggerForm();
			f.ShowDialog(this);
			Properties.Settings.Default.Triggers = PythonInterface.TriggerManager.GetSavedTriggers();
			Properties.Settings.Default.Save();
		}

		private void propertyEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PropertyForm f = new PropertyForm();
			f.Show(this);
		}
	}
}