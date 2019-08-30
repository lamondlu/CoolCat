using System;
using System.Diagnostics;

namespace MystiquePluginGlobalTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("dotnet");
            
            
            var startInfo = new ProcessStartInfo("dotnet");
            startInfo.WorkingDirectory = @"C:\Users\Lamond Lu\source\repos\DynamicPlugins\DemoPlugin1";
            startInfo.Arguments = "publish -o app";
            startInfo.UseShellExecute = true;
           
            

            process.Start();
            process.WaitForExit();

        }
    }
}
