using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MACROSCOP_CLIENT
{
    class FileOperations
    {
        // Contains directory adress
        public string DirectoryPath { get; private set; }
        // Number of files in directory
        public int CountFiles { get; private set; }

        public IEnumerable<string> PathCollection { get; private set; }
        public FileOperations(string path)
        {
            DirectoryPath = path;
            PathCollection = GetDirectoryFiles();
        }
        // Returns collection of paths or a null in which case raises an exception
        private IEnumerable<string> GetDirectoryFiles()
        {
            IEnumerable<string> txtFiles;
            try
            {
                // Collection of string paths
                txtFiles = Directory.EnumerateFiles(DirectoryPath);
                CountFiles = txtFiles.Count();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return txtFiles;
        }
        // Returns text from a file
        public IEnumerable<string> GetFile()
        {
            string text = null;
            for (int i = 0; i < PathCollection.Count(); i++)
            {
                var path = PathCollection.ElementAt(i);
                try
                {
                    using var sr = new StreamReader(path);
                    text = sr.ReadToEnd();
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine(path);
                yield return text;
            }
        }
    }
}
