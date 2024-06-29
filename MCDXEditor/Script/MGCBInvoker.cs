using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MCDX.Editor
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

        public static void BuildContent(string dicrectory)
        {
            string cmd = $"dotnet mgcb /@: {dicrectory}";
            Console.WriteLine($"Executing command {cmd}");
            string result = ExecuteInCmd(cmd);
            Console.WriteLine($"Result {result}");
        }
        public static void BuildContent(string contentCommandDirectory,string outputDir,string intermediateDir,string workingDir)
        {
            //remark: 天哪!你疯了吗? 居然用File.ReadAllText对一个文件夹
            //remark: 超级不建议加这个的, 因为每一个人的项目路径都不一样
            string result = ExecuteInCmd("dotnet mgcb"+" /@:" + "\"" + contentCommandDirectory + "\"" + " /platform:Windows /outputDir:" + "\"" + outputDir + "\"" + " /intermediateDir:" + "\"" + intermediateDir + "\"" + " /workingDir:" + "\"" + workingDir + "\"");

            Debug.WriteLine(result);
        }
    }
}
