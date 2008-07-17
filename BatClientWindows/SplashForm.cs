using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BatMud.BatClientWindows
{
	public partial class SplashForm : Form
	{
		public SplashForm()
		{
			InitializeComponent();

			this.ClientSize = pictureBox1.Size;
		}
	}
}