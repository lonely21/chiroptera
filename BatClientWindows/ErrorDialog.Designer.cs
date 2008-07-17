namespace BatMud.BatClientWindows
{
	partial class ErrorDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.errorTextBox = new System.Windows.Forms.TextBox();
			this.sendButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.emailTextBox = new System.Windows.Forms.TextBox();
			this.commentsTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.quitButton = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// errorTextBox
			// 
			this.errorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.errorTextBox.Location = new System.Drawing.Point(12, 25);
			this.errorTextBox.Multiline = true;
			this.errorTextBox.Name = "errorTextBox";
			this.errorTextBox.ReadOnly = true;
			this.errorTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.errorTextBox.Size = new System.Drawing.Size(557, 204);
			this.errorTextBox.TabIndex = 0;
			this.errorTextBox.TabStop = false;
			// 
			// sendButton
			// 
			this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.sendButton.Location = new System.Drawing.Point(494, 401);
			this.sendButton.Name = "sendButton";
			this.sendButton.Size = new System.Drawing.Size(75, 23);
			this.sendButton.TabIndex = 3;
			this.sendButton.Text = "Send";
			this.sendButton.UseVisualStyleBackColor = true;
			this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.closeButton.Location = new System.Drawing.Point(494, 474);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 5;
			this.closeButton.Text = "Close";
			this.closeButton.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(9, 401);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(476, 45);
			this.label1.TabIndex = 3;
			this.label1.Text = "You can send the above information to the developer of BatClient by pressing the " +
				"\"Send\" button. No other information will be sent.\r\n";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 381);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "E-Mail (optional): ";
			// 
			// emailTextBox
			// 
			this.emailTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.emailTextBox.Location = new System.Drawing.Point(106, 378);
			this.emailTextBox.Name = "emailTextBox";
			this.emailTextBox.Size = new System.Drawing.Size(260, 20);
			this.emailTextBox.TabIndex = 2;
			// 
			// commentsTextBox
			// 
			this.commentsTextBox.AcceptsReturn = true;
			this.commentsTextBox.AcceptsTab = true;
			this.commentsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.commentsTextBox.Location = new System.Drawing.Point(12, 248);
			this.commentsTextBox.Multiline = true;
			this.commentsTextBox.Name = "commentsTextBox";
			this.commentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.commentsTextBox.Size = new System.Drawing.Size(557, 124);
			this.commentsTextBox.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 232);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(105, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Comments (optional):";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(58, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Error Data:";
			// 
			// quitButton
			// 
			this.quitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.quitButton.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.quitButton.Location = new System.Drawing.Point(401, 474);
			this.quitButton.Name = "quitButton";
			this.quitButton.Size = new System.Drawing.Size(75, 23);
			this.quitButton.TabIndex = 4;
			this.quitButton.Text = "Quit";
			this.quitButton.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(12, 446);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(354, 51);
			this.label5.TabIndex = 9;
			this.label5.Text = "You can Quit the program (recommended) or Close this window and try to continue t" +
				"he program (not recommended, may not work).";
			// 
			// ErrorDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(581, 509);
			this.ControlBox = false;
			this.Controls.Add(this.label5);
			this.Controls.Add(this.quitButton);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.commentsTextBox);
			this.Controls.Add(this.emailTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.sendButton);
			this.Controls.Add(this.errorTextBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ErrorDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Unrecoverable Error";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox errorTextBox;
		private System.Windows.Forms.Button sendButton;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox emailTextBox;
		private System.Windows.Forms.TextBox commentsTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button quitButton;
		private System.Windows.Forms.Label label5;
	}
}