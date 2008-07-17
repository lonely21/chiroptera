namespace BatMud.BatClientWindows
{
	partial class MainWindow
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
			System.Windows.Forms.Splitter splitter1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.mainMenu = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.connectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.batMUDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.hCBatMUDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.keyBindingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.hilitesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.triggersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.propertyEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quickHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.textView = new BatMud.BatClientWindows.TextViewControl();
			this.promptTextBox = new BatMud.BatClientWindows.PromptTextBox();
			splitter1 = new System.Windows.Forms.Splitter();
			this.mainMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitter1
			// 
			splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			splitter1.Location = new System.Drawing.Point(0, 477);
			splitter1.Name = "splitter1";
			splitter1.Size = new System.Drawing.Size(729, 4);
			splitter1.TabIndex = 3;
			splitter1.TabStop = false;
			// 
			// mainMenu
			// 
			this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.connectToolStripMenuItem1,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.mainMenu.Location = new System.Drawing.Point(0, 0);
			this.mainMenu.Name = "mainMenu";
			this.mainMenu.Size = new System.Drawing.Size(729, 24);
			this.mainMenu.TabIndex = 2;
			this.mainMenu.Text = "mainMenu";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// quitToolStripMenuItem
			// 
			this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			this.quitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.quitToolStripMenuItem.Text = "Exit";
			this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
			// 
			// connectToolStripMenuItem1
			// 
			this.connectToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.batMUDToolStripMenuItem,
            this.hCBatMUDToolStripMenuItem,
            this.toolStripSeparator1,
            this.disconnectToolStripMenuItem});
			this.connectToolStripMenuItem1.Name = "connectToolStripMenuItem1";
			this.connectToolStripMenuItem1.Size = new System.Drawing.Size(59, 20);
			this.connectToolStripMenuItem1.Text = "Connect";
			// 
			// batMUDToolStripMenuItem
			// 
			this.batMUDToolStripMenuItem.Name = "batMUDToolStripMenuItem";
			this.batMUDToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.batMUDToolStripMenuItem.Text = "BatMUD";
			this.batMUDToolStripMenuItem.Click += new System.EventHandler(this.batMUDToolStripMenuItem_Click);
			// 
			// hCBatMUDToolStripMenuItem
			// 
			this.hCBatMUDToolStripMenuItem.Name = "hCBatMUDToolStripMenuItem";
			this.hCBatMUDToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.hCBatMUDToolStripMenuItem.Text = "HC BatMUD";
			this.hCBatMUDToolStripMenuItem.Click += new System.EventHandler(this.hCBatMUDToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(137, 6);
			// 
			// disconnectToolStripMenuItem
			// 
			this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
			this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.disconnectToolStripMenuItem.Text = "Disconnect";
			this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontToolStripMenuItem,
            this.keyBindingsToolStripMenuItem,
            this.hilitesToolStripMenuItem,
            this.triggersToolStripMenuItem,
            this.propertyEditorToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// fontToolStripMenuItem
			// 
			this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
			this.fontToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.fontToolStripMenuItem.Text = "Font...";
			this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
			// 
			// keyBindingsToolStripMenuItem
			// 
			this.keyBindingsToolStripMenuItem.Name = "keyBindingsToolStripMenuItem";
			this.keyBindingsToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.keyBindingsToolStripMenuItem.Text = "Key bindings...";
			this.keyBindingsToolStripMenuItem.Click += new System.EventHandler(this.keyBindingsToolStripMenuItem_Click);
			// 
			// hilitesToolStripMenuItem
			// 
			this.hilitesToolStripMenuItem.Name = "hilitesToolStripMenuItem";
			this.hilitesToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.hilitesToolStripMenuItem.Text = "Hilites...";
			this.hilitesToolStripMenuItem.Click += new System.EventHandler(this.hilitesToolStripMenuItem_Click);
			// 
			// triggersToolStripMenuItem
			// 
			this.triggersToolStripMenuItem.Name = "triggersToolStripMenuItem";
			this.triggersToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.triggersToolStripMenuItem.Text = "Triggers...";
			this.triggersToolStripMenuItem.Click += new System.EventHandler(this.triggersToolStripMenuItem_Click);
			// 
			// propertyEditorToolStripMenuItem
			// 
			this.propertyEditorToolStripMenuItem.Name = "propertyEditorToolStripMenuItem";
			this.propertyEditorToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.propertyEditorToolStripMenuItem.Text = "Property Editor...";
			this.propertyEditorToolStripMenuItem.Click += new System.EventHandler(this.propertyEditorToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quickHelpToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// quickHelpToolStripMenuItem
			// 
			this.quickHelpToolStripMenuItem.Name = "quickHelpToolStripMenuItem";
			this.quickHelpToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.quickHelpToolStripMenuItem.Text = "Quick Help";
			this.quickHelpToolStripMenuItem.Click += new System.EventHandler(this.quickHelpToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.aboutToolStripMenuItem.Text = "About BatClient...";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// textView
			// 
			this.textView.BackColor = System.Drawing.Color.Black;
			this.textView.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.textView.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::BatMud.BatClientWindows.Properties.Settings.Default, "MainFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textView.Font = global::BatMud.BatClientWindows.Properties.Settings.Default.MainFont;
			this.textView.ForeColor = System.Drawing.Color.White;
			this.textView.Location = new System.Drawing.Point(0, 24);
			this.textView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.textView.Name = "textView";
			this.textView.ParagraphContainer = null;
			this.textView.Size = new System.Drawing.Size(729, 453);
			this.textView.TabIndex = 4;
			this.textView.TabStop = false;
			this.textView.LinkClicked += new BatMud.BatClientWindows.TextViewControl.LinkClickDelegate(this.textView_LinkClicked);
			this.textView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textView_MouseDown);
			this.textView.Enter += new System.EventHandler(this.textView_Enter);
			// 
			// promptTextBox
			// 
			this.promptTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.promptTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.promptTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::BatMud.BatClientWindows.Properties.Settings.Default, "MainFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.promptTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.promptTextBox.Font = global::BatMud.BatClientWindows.Properties.Settings.Default.MainFont;
			this.promptTextBox.History = new string[0];
			this.promptTextBox.Location = new System.Drawing.Point(0, 481);
			this.promptTextBox.Name = "promptTextBox";
			this.promptTextBox.Prompt = "prompt >";
			this.promptTextBox.PromptPassword = false;
			this.promptTextBox.SelectedText = "";
			this.promptTextBox.SelectionLength = 0;
			this.promptTextBox.SelectionStart = 0;
			this.promptTextBox.Size = new System.Drawing.Size(729, 39);
			this.promptTextBox.TabIndex = 1;
			this.promptTextBox.textEntered += new BatMud.BatClientWindows.PromptTextBox.TextEnteredDelegate(this.promptTextBox_textEntered);
			this.promptTextBox.rawKeyDown += new System.Windows.Forms.KeyEventHandler(this.promptTextBox_rawKeyDown);
			this.promptTextBox.rawKeyUp += new System.Windows.Forms.KeyEventHandler(this.promptTextBox_rawKeyUp);
			this.promptTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textView_MouseDown);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(729, 520);
			this.Controls.Add(this.textView);
			this.Controls.Add(splitter1);
			this.Controls.Add(this.mainMenu);
			this.Controls.Add(this.promptTextBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.mainMenu;
			this.Name = "MainWindow";
			this.Text = "BatClient";
			this.Load += new System.EventHandler(this.MainWindow_Load);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textView_MouseDown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
			this.mainMenu.ResumeLayout(false);
			this.mainMenu.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private PromptTextBox promptTextBox;
		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem batMUDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hCBatMUDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
		private TextViewControl textView;
		private System.Windows.Forms.ToolStripMenuItem quickHelpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem keyBindingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hilitesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem triggersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem propertyEditorToolStripMenuItem;
	}
}

