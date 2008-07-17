namespace BatMud.BatClientWindows
{
	partial class KeyBindForm
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
			this.keyTextBox = new System.Windows.Forms.TextBox();
			this.styleRadioButton1 = new System.Windows.Forms.RadioButton();
			this.styleRadioButton2 = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.actionTextBox = new System.Windows.Forms.TextBox();
			this.listView = new System.Windows.Forms.ListView();
			this.Key = new System.Windows.Forms.ColumnHeader();
			this.Type = new System.Windows.Forms.ColumnHeader();
			this.Action = new System.Windows.Forms.ColumnHeader();
			this.deleteButton = new System.Windows.Forms.Button();
			this.assignButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.newButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.panel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// keyTextBox
			// 
			this.keyTextBox.AcceptsReturn = true;
			this.keyTextBox.AcceptsTab = true;
			this.keyTextBox.Location = new System.Drawing.Point(6, 32);
			this.keyTextBox.Multiline = true;
			this.keyTextBox.Name = "keyTextBox";
			this.keyTextBox.Size = new System.Drawing.Size(160, 20);
			this.keyTextBox.TabIndex = 0;
			this.keyTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyTextBox_KeyDown);
			// 
			// styleRadioButton1
			// 
			this.styleRadioButton1.AutoSize = true;
			this.styleRadioButton1.Checked = true;
			this.styleRadioButton1.Location = new System.Drawing.Point(3, 3);
			this.styleRadioButton1.Name = "styleRadioButton1";
			this.styleRadioButton1.Size = new System.Drawing.Size(50, 17);
			this.styleRadioButton1.TabIndex = 0;
			this.styleRadioButton1.TabStop = true;
			this.styleRadioButton1.Text = "Send";
			this.styleRadioButton1.UseVisualStyleBackColor = true;
			// 
			// styleRadioButton2
			// 
			this.styleRadioButton2.AutoSize = true;
			this.styleRadioButton2.Location = new System.Drawing.Point(3, 26);
			this.styleRadioButton2.Name = "styleRadioButton2";
			this.styleRadioButton2.Size = new System.Drawing.Size(52, 17);
			this.styleRadioButton2.TabIndex = 1;
			this.styleRadioButton2.Text = "Script";
			this.styleRadioButton2.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.styleRadioButton2);
			this.panel1.Controls.Add(this.styleRadioButton1);
			this.panel1.Location = new System.Drawing.Point(172, 9);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(58, 46);
			this.panel1.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(124, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Press desired key below:";
			// 
			// actionTextBox
			// 
			this.actionTextBox.AcceptsReturn = true;
			this.actionTextBox.AcceptsTab = true;
			this.actionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.actionTextBox.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.actionTextBox.Location = new System.Drawing.Point(6, 61);
			this.actionTextBox.Multiline = true;
			this.actionTextBox.Name = "actionTextBox";
			this.actionTextBox.Size = new System.Drawing.Size(541, 165);
			this.actionTextBox.TabIndex = 3;
			// 
			// listView
			// 
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Key,
            this.Type,
            this.Action});
			this.listView.FullRowSelect = true;
			this.listView.HideSelection = false;
			this.listView.Location = new System.Drawing.Point(12, 12);
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(553, 162);
			this.listView.TabIndex = 0;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
			// 
			// Key
			// 
			this.Key.Text = "Key";
			// 
			// Type
			// 
			this.Type.Text = "Type";
			// 
			// Action
			// 
			this.Action.Text = "Action";
			this.Action.Width = 200;
			// 
			// deleteButton
			// 
			this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.deleteButton.Location = new System.Drawing.Point(571, 41);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(75, 23);
			this.deleteButton.TabIndex = 3;
			this.deleteButton.Text = "Delete";
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// assignButton
			// 
			this.assignButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.assignButton.Location = new System.Drawing.Point(472, 19);
			this.assignButton.Name = "assignButton";
			this.assignButton.Size = new System.Drawing.Size(75, 23);
			this.assignButton.TabIndex = 2;
			this.assignButton.Text = "Assign";
			this.assignButton.UseVisualStyleBackColor = true;
			this.assignButton.Click += new System.EventHandler(this.assignButton_Click);
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.closeButton.Location = new System.Drawing.Point(571, 389);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 4;
			this.closeButton.Text = "Close";
			this.closeButton.UseVisualStyleBackColor = true;
			// 
			// newButton
			// 
			this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.newButton.Location = new System.Drawing.Point(571, 12);
			this.newButton.Name = "newButton";
			this.newButton.Size = new System.Drawing.Size(75, 23);
			this.newButton.TabIndex = 2;
			this.newButton.Text = "New";
			this.newButton.UseVisualStyleBackColor = true;
			this.newButton.Click += new System.EventHandler(this.newButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.keyTextBox);
			this.groupBox1.Controls.Add(this.actionTextBox);
			this.groupBox1.Controls.Add(this.panel1);
			this.groupBox1.Controls.Add(this.assignButton);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 180);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(553, 232);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Key";
			// 
			// KeyBindForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(658, 424);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.newButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.deleteButton);
			this.Controls.Add(this.listView);
			this.Name = "KeyBindForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Key Bindings";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox keyTextBox;
		private System.Windows.Forms.RadioButton styleRadioButton1;
		private System.Windows.Forms.RadioButton styleRadioButton2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox actionTextBox;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ColumnHeader Key;
		private System.Windows.Forms.ColumnHeader Type;
		private System.Windows.Forms.ColumnHeader Action;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button assignButton;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Button newButton;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}