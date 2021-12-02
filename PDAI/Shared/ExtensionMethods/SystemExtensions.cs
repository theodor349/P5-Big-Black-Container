using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ExtensionMethods
{
    public static class SystemExtensions
    {
        private static Settings _settings => Settings.Current;

        #region Process
        public static void PrintProcessName(string identifier, Process process)
        {
            Logger.Log("'" + identifier + "' has id: " + process.Id);
        }

        public static void RunnInParallel<T>(List<T> inputs, Action<T> action, int cores, bool inParallel = true)
        {
            var threads = new List<Task>();
            foreach (var item in inputs)
            {
                if(inParallel)
                    threads.Add(RunInparallel(item, action));
                else
                    action(item);

                if (threads.Where(t => !t.IsCompleted).Count() >= cores)
                    Task.WaitAll(threads.ToArray());
            }
            Task.WaitAll(threads.ToArray());
        }
        private static async Task RunInparallel<T>(T item, Action<T> action)
        {
            await Task.Run(() =>
            {
                action(item);
            });
        }
        #endregion

        #region Paths
        public static List<string> GetActionFolders(string rootBbcFolder, string domainName)
        {
            string domainFolderPath = Path.Combine(rootBbcFolder, "domainfiles", domainName);
            return Directory.GetDirectories(domainFolderPath).ToList();
        }

        public static List<string> GetTrainingFolders(string actionFolderPath, bool includeTest = false)
        {
            var res = Directory.GetDirectories(actionFolderPath).ToList();
            if (includeTest)
                return res;
            else
                return res.Where(x => !new DirectoryInfo(x).Name.ToLower().Equals("test")).ToList();
        }

        public static List<string> GetAllActions()
        {
            List<string> res = new List<string>();
            var domains = GetDomains();

            foreach (var domain in domains)
            {
                res.AddRange(GetActions(domain));
            }

            return res;
        }

        private static List<string> GetDomains()
        {
            return Directory.GetDirectories(Path.Combine(_settings.TargetFolder, "domainfiles")).ToList();
        }

        private static List<string> GetActions(string domain)
        {
            string[] actions = Directory.GetDirectories(domain);
            if (_settings.ActionsToRun is null)
                return actions.ToList();
            else
                return actions.Where(x => _settings.ActionsToRun.Contains(new FileInfo(x).Name.ToLower())).ToList();
        }
        #endregion

        #region Get Python Path
        public static string GetPythonPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetPythonExePathWindows();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "/usr/local/bin/python3.9";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "/usr/bin/python";
            else
                throw new Exception("Could not find python path");
        }

        private static string GetPythonExePathWindows()
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            string pythonPath = "/usr/local/bin/python3.9";

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
        #endregion
    }
}
