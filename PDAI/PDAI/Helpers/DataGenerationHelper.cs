using Shared;
using Shared.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAI.Helpers
{
    public class DataGenerationHelper
    {
        private readonly Settings _settings;

        public DataGenerationHelper(Settings settings)
        {
            _settings = settings;
        }
        public void RunTest(string trainPath)
        {
            string testerPath = Path.Combine(_settings.TargetFolder, "tester.py");
            Process process = new();
            process.StartInfo.FileName = SystemExtensions.GetPythonPath();
            process.StartInfo.Arguments = testerPath + " " + trainPath;
            StartProcess(process, false);
        }

        public void RunPopper(string trainPath, string rootPath, int beta, int maxRuntime)
        {
            string popperPath = Path.Combine(rootPath, "popper/popper.py");
            Process popperProcess = new();
            popperProcess.StartInfo.FileName = SystemExtensions.GetPythonPath();
            popperProcess.StartInfo.Arguments = popperPath + " " + trainPath + " " + beta + " --stats --info";
            StartProcess(popperProcess, false, maxRuntime);
        }

        private static void StartProcess(Process process, bool noConsole, int maxRuntime = int.MaxValue)
        {
            process.StartInfo.CreateNoWindow = noConsole;
            process.Start();
            process.WaitForExit(maxRuntime);
            process.Kill();
        }

    }
}
