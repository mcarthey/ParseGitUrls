using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ParseGitUrls
{
    public class FileProcessor
    {
        private readonly string _path = Path.Combine(Environment.CurrentDirectory, "Files");

        public FileProcessor()
        {
            ProcessDirectory(_path);
        }

        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            var fileEntries = Directory.GetFiles(targetDirectory);
            foreach (var fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            var subdirectoriesEntries = Directory.GetDirectories(targetDirectory);
            foreach (var subdirectories in subdirectoriesEntries)
                ProcessDirectory(subdirectories);
        }

        // Parse downloaded HTML files to pull out the Git URL and last name
        public static void ProcessFile(string path)
        {
            string url = null;
            string lastName = null;

            using (var sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("url="))
                    {
                        var firstPos = line.IndexOf("https", StringComparison.Ordinal);
                        var lastPos = line.IndexOf("\">", StringComparison.Ordinal);
                        url = line.Substring(firstPos, lastPos - firstPos);
                    }

                    if (line.Contains("title"))
                    {
                        var cleanString = Regex.Replace(line, "<.*?>", string.Empty);
                        lastName = cleanString.Split(' ').Last();
                    }
                }

                Console.WriteLine($"git clone {url} {lastName}");
            }
        }

        public static void ExecuteCommand(string command)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {command}";
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}