using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using BatMud.BatClientBase;


// Courier new fontti: underlinet scrollaa v‰‰rin
// paste remove linefeeds optio menuihin
// copy-v‰ritys pois klikkaamalla

namespace BatMud.BatClientWindows
{
	static class Program
	{
#if CONSOLE
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		static extern Boolean AllocConsole();
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		static extern Boolean FreeConsole();
#endif
		[STAThread]
		static void Main(string[] args)
		{
#if CONSOLE
			AllocConsole();
#endif
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				Application.ThreadException += Application_ThreadException;
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			SplashForm splash = null;
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				splash = new SplashForm();
				splash.Show();
				Application.DoEvents();
			}

			ClientCore core = new ClientCore();
			core.Initialize(args);

			if (!System.Diagnostics.Debugger.IsAttached)
			{
				splash.Close();
				splash = null;
			}

			Application.Run(core.MainWindow);

			core.Uninitialize();

			Properties.Settings.Default.Save();

#if CONSOLE
			FreeConsole();
#endif
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ShowErrorDialog(e.ExceptionObject);
		}

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			ShowErrorDialog(e.Exception);
		}

		static void ShowErrorDialog(object e)
		{
			try
			{
				ErrorDialog dlg = new ErrorDialog(e);
				DialogResult res = dlg.ShowDialog();
				if(res == DialogResult.Abort)
					Application.Exit();
			}
			catch
			{
				MessageBox.Show("Fatal Error in error reporting"); 
				Application.Exit();
			}
		}
	}
}
