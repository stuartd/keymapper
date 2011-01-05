using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;


namespace KeyMapper
{
	[RunInstaller(true)]
	public class Ngen : Installer
	{

		// From http://dotnetperls.com/Content/Ngen-Installer-Class.aspx
		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);
		
			string runtimeStr = RuntimeEnvironment.GetRuntimeDirectory();
			string ngenStr = Path.Combine(runtimeStr, "ngen.exe");

			Process process = new Process();
			process.StartInfo.FileName = ngenStr;

			string assemblyPath = Context.Parameters["assemblypath"];

			process.StartInfo.Arguments = "install \"" + assemblyPath + "\"";

			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.Start();
			process.WaitForExit();
		
		}
	}
}
