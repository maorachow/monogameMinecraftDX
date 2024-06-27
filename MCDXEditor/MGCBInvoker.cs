using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MCDXEditor
{
    public class MGCBInvoker
    {
        [DllImport("kernel32")]
        public static extern int WinExec(string programPath, int operType);

        public static string mgcbBuilderPath=Directory.GetCurrentDirectory()+"/MGCB/mgcb.exe";

        public static string ExecuteInCmd(string cmdline)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine(cmdline + "&exit");

                //获取cmd窗口的输出信息  
                string output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();
                process.Close();

                return output;
            }

        }

        public static void BuildContent(string contentCommandDirectory,string outputDir,string intermediateDir,string workingDir)
        {

             
                string contentCommand = File.ReadAllText(contentCommandDirectory);
                var fn = mgcbBuilderPath;
            //     System.Diagnostics.Process.Start("explorer.exe", "D:\\");
            //  var result = WinExec(mgcbBuilderPath, 1/*" /@:" + "\"" + contentCommandDirectory + "\"" + " /platform:Windows /outputDir:" + "\"" + outputDir + "\"" + " /intermediateDir:" + "\"" + intermediateDir + "\"" + " /workingDir:" + "\"" + workingDir + "\""*/);
            string result = ExecuteInCmd("dotnet mgcb"+" /@:" + "\"" + contentCommandDirectory + "\"" + " /platform:Windows /outputDir:" + "\"" + outputDir + "\"" + " /intermediateDir:" + "\"" + intermediateDir + "\"" + " /workingDir:" + "\"" + workingDir + "\"");

            Debug.WriteLine(result);



            //    process.WaitForExit();
            //        Debug.WriteLine("exitcode:"+process.ExitCode);
            //  


        }
    }
}
