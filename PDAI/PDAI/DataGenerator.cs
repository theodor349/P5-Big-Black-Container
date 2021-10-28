using System;
using System.Collections.Generic;
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

        private async Task RunPopper()
        {
            await Task.Run(() =>
            {
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
    }
}
