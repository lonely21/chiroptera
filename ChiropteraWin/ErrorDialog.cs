using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;

namespace Chiroptera.Win
{
	public partial class ErrorDialog : Form
	{
		public ErrorDialog(object e)
		{
			InitializeComponent();

			StringBuilder sb = new StringBuilder();

			Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

			sb.AppendFormat("Chiroptera Version: {0}\r\n", currentVersion);
			sb.AppendFormat("OSVersion: {0}\r\n", Environment.OSVersion);
			sb.AppendFormat("ProcessorCount: {0}\r\n", Environment.ProcessorCount);
			sb.AppendFormat("Version: {0}\r\n", Environment.Version);
			sb.AppendLine("----------");
			sb.Append(e.ToString());

			errorTextBox.Text = sb.ToString();
		}

		private void sendButton_Click(object sender, EventArgs e)
		{
			try
			{
				WebClient webClient = new WebClient();
				webClient.Headers.Add("pragma", "no-cache");
				webClient.Headers.Add("cache-control", "no-cache, must-revalidate, max_age=0");
				webClient.Headers.Add("expires", "0");

				string email = "";
				if (emailTextBox.Text.Length > 0)
				{
					System.Net.Mail.MailAddress mailAddress = new System.Net.Mail.MailAddress(emailTextBox.Text);
					email = mailAddress.ToString();
				}
				else
				{
					email = "batclient@bat.org";
				}

				NameValueCollection data = new NameValueCollection();
				data.Add("report", errorTextBox.Text + "\r\n----------\r\n" + commentsTextBox.Text);
				data.Add("email", email);
				byte[] arr = webClient.UploadValues(new Uri("http://www.bat.org/tomba-batclient.php"), data);

				string resp = ASCIIEncoding.ASCII.GetString(arr);
				if (resp == "OK")
					MessageBox.Show("Error report sent successfully");
				else
					MessageBox.Show("Unable to send the error report.\r\nThe server said:\r\n" + resp);
			}
			catch (Exception exc)
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("Failed to send the error report:");
				sb.AppendLine(exc.Message);
				if (exc.InnerException != null)
					sb.AppendLine(exc.InnerException.Message);
				MessageBox.Show(sb.ToString());
			}
		}
	}
}