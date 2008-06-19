namespace BatMud.BatClientWindows
{
	partial class HiliteForm
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
			this.fgButton = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.listView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.bgButton = new System.Windows.Forms.Button();
			this.ignoreCaseCheckBox = new System.Windows.Forms.CheckBox();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.newButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.hiliteLineCheckBox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.patternTextBox = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.bgCheckBox = new System.Windows.Forms.CheckBox();
			this.fgCheckBox = new System.Windows.Forms.CheckBox();
			this.applyButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// fgButton
			// 
			this.fgButton.Location = new System.Drawing.Point(210, 48);
			this.fgButton.Name = "fgButton";
			this.fgButton.Size = new System.Drawing.Size(23, 23);
			this.fgButton.TabIndex = 3;
			this.fgButton.UseVisualStyleBackColor = false;
			this.fgButton.Click += new System.EventHandler(this.fgButton_Click);
			// 
			// button3
			// 
			this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button3.Location = new System.Drawing.Point(609, 345);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 4;
			this.button3.Text = "Close";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// listView
			// 
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
			this.listView.FullRowSelect = true;
			this.listView.HideSelection = false;
			this.listView.Location = new System.Drawing.Point(12, 12);
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(600, 166);
			this.listView.TabIndex = 0;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Pattern";
			this.columnHeader1.Width = 312;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Foreground";
			this.columnHeader2.Width = 72;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Background";
			this.columnHeader3.Width = 72;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "IgnoreCase";
			this.columnHeader4.Width = 72;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "HiliteLine";
			this.columnHeader5.Width = 66;
			// 
			// bgButton
			// 
			this.bgButton.Location = new System.Drawing.Point(210, 73);
			this.bgButton.Name = "bgButton";
			this.bgButton.Size = new System.Drawing.Size(23, 23);
			this.bgButton.TabIndex = 4;
			this.bgButton.UseVisualStyleBackColor = false;
			this.bgButton.Click += new System.EventHandler(this.bgButton_Click);
			// 
			// ignoreCaseCheckBox
			// 
			this.ignoreCaseCheckBox.AutoSize = true;
			this.ignoreCaseCheckBox.Location = new System.Drawing.Point(6, 49);
			this.ignoreCaseCheckBox.Name = "ignoreCaseCheckBox";
			this.ignoreCaseCheckBox.Size = new System.Drawing.Size(83, 17);
			this.ignoreCaseCheckBox.TabIndex = 1;
			this.ignoreCaseCheckBox.Text = "Ignore Case";
			this.ignoreCaseCheckBox.UseVisualStyleBackColor = true;
			// 
			// colorDialog
			// 
			this.colorDialog.SolidColorOnly = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(143, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(61, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Foreground";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(143, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Background";
			// 
			// newButton
			// 
			this.newButton.Location = new System.Drawing.Point(618, 12);
			this.newButton.Name = "newButton";
			this.newButton.Size = new System.Drawing.Size(75, 23);
			this.newButton.TabIndex = 2;
			this.newButton.Text = "New";
			this.newButton.UseVisualStyleBackColor = true;
			this.newButton.Click += new System.EventHandler(this.newButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.Location = new System.Drawing.Point(618, 41);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(75, 23);
			this.deleteButton.TabIndex = 3;
			this.deleteButton.Text = "Delete";
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// hiliteLineCheckBox
			// 
			this.hiliteLineCheckBox.AutoSize = true;
			this.hiliteLineCheckBox.Location = new System.Drawing.Point(6, 72);
			this.hiliteLineCheckBox.Name = "hiliteLineCheckBox";
			this.hiliteLineCheckBox.Size = new System.Drawing.Size(99, 17);
			this.hiliteLineCheckBox.TabIndex = 2;
			this.hiliteLineCheckBox.Text = "Hilite whole line";
			this.hiliteLineCheckBox.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Pattern";
			// 
			// patternTextBox
			// 
			this.patternTextBox.Location = new System.Drawing.Point(53, 23);
			this.patternTextBox.Name = "patternTextBox";
			this.patternTextBox.Size = new System.Drawing.Size(336, 20);
			this.patternTextBox.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.bgCheckBox);
			this.groupBox1.Controls.Add(this.fgCheckBox);
			this.groupBox1.Controls.Add(this.applyButton);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.patternTextBox);
			this.groupBox1.Controls.Add(this.fgButton);
			this.groupBox1.Controls.Add(this.hiliteLineCheckBox);
			this.groupBox1.Controls.Add(this.bgButton);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.ignoreCaseCheckBox);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(12, 184);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(600, 116);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Hilite";
			// 
			// bgCheckBox
			// 
			this.bgCheckBox.AutoSize = true;
			this.bgCheckBox.Location = new System.Drawing.Point(239, 77);
			this.bgCheckBox.Name = "bgCheckBox";
			this.bgCheckBox.Size = new System.Drawing.Size(121, 17);
			this.bgCheckBox.TabIndex = 6;
			this.bgCheckBox.Text = "Default Background";
			this.bgCheckBox.UseVisualStyleBackColor = true;
			this.bgCheckBox.CheckedChanged += new System.EventHandler(this.bgCheckBox_CheckedChanged);
			// 
			// fgCheckBox
			// 
			this.fgCheckBox.AutoSize = true;
			this.fgCheckBox.Location = new System.Drawing.Point(239, 52);
			this.fgCheckBox.Name = "fgCheckBox";
			this.fgCheckBox.Size = new System.Drawing.Size(117, 17);
			this.fgCheckBox.TabIndex = 5;
			this.fgCheckBox.Text = "Default Foreground";
			this.fgCheckBox.UseVisualStyleBackColor = true;
			this.fgCheckBox.CheckedChanged += new System.EventHandler(this.fgCheckBox_CheckedChanged);
			// 
			// applyButton
			// 
			this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.applyButton.Location = new System.Drawing.Point(519, 87);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(75, 23);
			this.applyButton.TabIndex = 7;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// HiliteForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(696, 380);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.deleteButton);
			this.Controls.Add(this.newButton);
			this.Controls.Add(this.listView);
			this.Controls.Add(this.button3);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "HiliteForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Hilites";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button fgButton;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.Button bgButton;
		private System.Windows.Forms.CheckBox ignoreCaseCheckBox;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button newButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.CheckBox hiliteLineCheckBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox patternTextBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button applyButton;
		private System.Windows.Forms.CheckBox bgCheckBox;
		private System.Windows.Forms.CheckBox fgCheckBox;
	}
}