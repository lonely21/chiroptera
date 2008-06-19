namespace BatMud.BatClientWindows
{
    partial class TextViewControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.scrollBar = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// scrollBar
			// 
			this.scrollBar.Cursor = System.Windows.Forms.Cursors.Default;
			this.scrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.scrollBar.Location = new System.Drawing.Point(507, 0);
			this.scrollBar.Name = "scrollBar";
			this.scrollBar.Size = new System.Drawing.Size(17, 348);
			this.scrollBar.TabIndex = 0;
			this.scrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnScrollBarScroll);
			// 
			// TextView2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.scrollBar);
			this.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "TextView2";
			this.Size = new System.Drawing.Size(524, 348);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar scrollBar;
    }
}
