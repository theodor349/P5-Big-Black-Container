using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAI
{
    // John 
    public class DataGenerator
    {
        public void GenerateData(string inputFolderPath, string outputFolderPath)
        {
            string domainFolderPath = Path.Combine(outputFolderPath, "domainfiles", Path.GetFileName(inputFolderPath));
            List<string> actionsPaths = Directory.GetDirectories(domainFolderPath).ToList();

            foreach (var actionPath in actionsPaths)
            {
                GenerateForAction(outputFolderPath, domainFolderPath, actionPath);
            }
        }

        private void GenerateForAction(string rootPath, string domainPath, string actionPath)
        {
            for (int i = 0; i < 10; i++)
            {
                SetInput();
                Train();
                Test();
                SaveResults();
            }
        }

        private void SetInput()
        {
            
        }

        private void Train()
        {
            var threads = new List<Task>();
            for (int j = 0; j < 10; j++)
                threads.Add(RunPopper());
            Task.WaitAll(threads.ToArray());
        }

        private async Task RunPopper(string trainPath, string rootPath, int beta, int maxRuntime)
        {
            await Task.Run(() =>
            {
                string popperPath = Path.Combine(rootPath, "popper\\popper.py");

                Process popperProcess = new();
                popperProcess.StartInfo.FileName = GetPythonExePath();
                popperProcess.StartInfo.CreateNoWindow = true;
                popperProcess.StartInfo.Arguments = popperPath + " " + trainPath + " " + beta;

                popperProcess.Start();
                popperProcess.WaitForExit(maxRuntime);
            });
        }

        private void Test()
        {
            var threads = new List<Task>();
            for (int j = 0; j < 10; j++)
                threads.Add(RunTest());
            Task.WaitAll(threads.ToArray());
        }

        private async Task RunTest()
        {
            await Task.Run(() =>
            {
            });
        }

        private void SaveResults()
        {

        }

        private string GetPythonExePath()
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            string pythonPath = null;
            foreach (string p in path.Split(new char[] { ';' }))
            {
                string fullPath = Path.Combine(p, "python.exe");
                if (File.Exists(fullPath))
                {
                    pythonPath = fullPath;
                    break;
                }
            }

            if (pythonPath == null)
                throw new Exception("Unable to find python exe in Environment variables :(");
            else
                return pythonPath;
        }
    }
}
