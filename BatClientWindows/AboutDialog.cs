using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BatMud.BatClientWindows
{
	public partial class AboutDialog : Form
	{
		public AboutDialog()
		{
			InitializeComponent();

			Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

			versionLabel.Text = "Version " + currentVersion.ToString();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.bat.org/~tomba/batclient.html");
		}
	}
}