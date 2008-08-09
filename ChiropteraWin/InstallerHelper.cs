using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace Chiroptera.Win
{
	[RunInstaller(true)]
	public partial class InstallerHelper : Installer
	{
		[DllImport("Mscoree.dll")]
		extern static int GetCORSystemDirectory(
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] StringBuilder buffer,
			int bufferLen, out int len);

		public InstallerHelper() : base()
		{
			InitializeComponent();
		}

		int RunNGEN(string args)
		{
			StringBuilder buf = new StringBuilder(1024);
			int len;
			GetCORSystemDirectory(buf, buf.Capacity, out len);

			string cmd = buf.ToString() + "ngen.exe";

			int exitCode = -1;

			try
			{
				//MessageBox.Show(String.Format("Running {0} with args {1}", cmd, args));
				ProcessStartInfo psi = new ProcessStartInfo(cmd, args);
				psi.WindowStyle = ProcessWindowStyle.Hidden;
				psi.ErrorDialog = true;
				Process p = Process.Start(psi);
				p.WaitForExit();
				exitCode = p.ExitCode;
			}
			catch (Exception e)
			{
				throw new InstallException(e.Message);
			}

			return exitCode;
		}

		[SecurityPermission(SecurityAction.Demand)]
		public override void Install(System.Collections.IDictionary stateSaver)
		{
			base.Install(stateSaver);

			string assembly = this.Context.Parameters["Assembly"];
			string appBase = this.Context.Parameters["AppBase"];
			appBase = System.IO.Path.GetDirectoryName(appBase); // Removes the trailing slash

			//MessageBox.Show(String.Format("Args {0}", assembly));
			
			if (assembly == null || assembly.Length == 0 || appBase == null || appBase.Length == 0)
				throw new InstallException("Illegal arguments for ngen uninstall helper");

			string args = String.Format("install {0} /AppBase:\"{1}\"", assembly, appBase);

			//MessageBox.Show(args);

			int exitCode = RunNGEN(args);

			if (exitCode != 0)
			{
				MessageBox.Show("Failed to precompile IronPython. This is not fatal.");
			}
		}

		[SecurityPermission(SecurityAction.Demand)]
		public override void Uninstall(System.Collections.IDictionary savedState)
		{
			base.Uninstall(savedState);

			string assembly = this.Context.Parameters["Assembly"];
			string appBase = this.Context.Parameters["AppBase"];
			appBase = System.IO.Path.GetDirectoryName(appBase); // Removes the trailing slash

			//MessageBox.Show(String.Format("Args {0}", assembly));

			if (assembly == null || assembly.Length == 0 || appBase == null || appBase.Length == 0)
				throw new InstallException("Illegal arguments for ngen install helper");

			string args = String.Format("uninstall {0} /AppBase:\"{1}\"", assembly, appBase);

			//MessageBox.Show(args);

			int exitCode = RunNGEN(args);

			if (exitCode != 0)
			{
				MessageBox.Show("Failed to remove precompiled IronPython. This is not fatal.");
			}
		}
	}
}
