#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#endregion

namespace Chiroptera.Win
{
	public partial class PromptTextBox : UserControl
	{
		public delegate void TextEnteredDelegate(string str);
		public event TextEnteredDelegate textEntered = null;

		public event KeyEventHandler rawKeyUp = null;
		public event KeyEventHandler rawKeyDown = null;

		public PromptTextBox()
		{
			InitializeComponent();

			m_promptLabel.Text = "";

			m_inputBox.textEntered += new HistoryTextBox.TextEnteredDelegate(m_inputBox_textEntered);

			m_inputBox.rawKeyUp += new KeyEventHandler(m_inputBox_rawKeyUp);
			m_inputBox.rawKeyDown += new KeyEventHandler(m_inputBox_rawKeyDown);
			m_inputBox.MouseWheel += new MouseEventHandler(m_inputBox_MouseWheel);

			m_promptLabel.BackColor = m_inputBox.BackColor;
			this.BackColor = m_inputBox.BackColor;
		}

		void m_inputBox_MouseWheel(object sender, MouseEventArgs e)
		{
			OnMouseWheel(e);
		}

		private void m_inputBox_textEntered(string str)
		{
			this.Prompt = "";
			if (textEntered != null)
    			textEntered(str);
		}


		private void m_inputBox_rawKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (rawKeyUp != null)
				rawKeyUp(sender, e);
		}

		private void m_inputBox_rawKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (rawKeyDown != null)
				rawKeyDown(sender, e);
		}

		public void Paste()
		{
			m_inputBox.Paste();
		}

		public string[] History
		{
			get { return m_inputBox.History; }
			set { m_inputBox.History = value; }
		}

		public string Prompt
		{
			get
			{
				return m_promptLabel.Text;
			}

			set
			{
				m_promptLabel.Text = value;

				//this.PerformLayout();
			}
		}

		public bool PromptPassword
		{
			get
			{
				return m_inputBox.PasswordChar != (char)0;
			}

			set
			{
				if (value == true)
				{
					m_inputBox.PasswordChar = '*';
				}
				else
				{
					m_inputBox.PasswordChar = (char)0;
				}
			}
		}

		public new string Text
		{
			get
			{
				return m_inputBox.Text;
			}

			set
			{
				m_inputBox.Text = value;
			}
		}

		public int SelectionStart
		{
			get
			{
				return m_inputBox.SelectionStart;
			}

			set
			{
				m_inputBox.SelectionStart = value;
			}
		}

		public int SelectionLength
		{
			get
			{
				return m_inputBox.SelectionLength;
			}

			set
			{
				m_inputBox.SelectionLength = value;
			}
		}

		public string SelectedText
		{
			get
			{
				return m_inputBox.SelectedText;
			}

			set
			{
				if (value.Length > 0)
				{
					m_inputBox.SelectedText = value;
				}
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);

			m_promptLabel.Font = this.Font;
			m_inputBox.Font = this.Font;
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged(e);

			m_promptLabel.BackColor = this.BackColor;
			m_inputBox.BackColor = this.BackColor;
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			base.OnForeColorChanged(e);

			m_promptLabel.ForeColor = this.ForeColor;
			m_inputBox.ForeColor = this.ForeColor;
		}
	}
}
