namespace Chiroptera.Win
{
	partial class PromptTextBox
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_promptLabel = new System.Windows.Forms.Label();
			this.m_inputBox = new Chiroptera.Win.HistoryTextBox();
			this.SuspendLayout();
			// 
			// m_promptLabel
			// 
			this.m_promptLabel.AutoSize = true;
			this.m_promptLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.m_promptLabel.Location = new System.Drawing.Point(0, 0);
			this.m_promptLabel.Name = "m_promptLabel";
			this.m_promptLabel.Size = new System.Drawing.Size(54, 13);
			this.m_promptLabel.TabIndex = 1;
			this.m_promptLabel.Text = "prompt g>";
			// 
			// m_inputBox
			// 
			this.m_inputBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.m_inputBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_inputBox.History = new string[0];
			this.m_inputBox.Location = new System.Drawing.Point(54, 0);
			this.m_inputBox.Multiline = true;
			this.m_inputBox.Name = "m_inputBox";
			this.m_inputBox.Size = new System.Drawing.Size(340, 55);
			this.m_inputBox.TabIndex = 0;
			// 
			// PromptTextBox
			// 
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Controls.Add(this.m_inputBox);
			this.Controls.Add(this.m_promptLabel);
			this.Name = "PromptTextBox";
			this.Size = new System.Drawing.Size(394, 55);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private HistoryTextBox m_inputBox;
		private System.Windows.Forms.Label m_promptLabel;
	}
}
