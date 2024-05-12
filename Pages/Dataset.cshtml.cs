using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks; // Add this line
using Microsoft.AspNetCore.Hosting; // Add this line

namespace YourNamespace.Pages
{
    public class DatasetModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public DatasetModel(IWebHostEnvironment env)
        {
            _env = env;
        }
        public string Link { get; set; }
        public int RowCount { get; set; }
        public string ImagePath { get; set; } // Add this line
        public async Task OnGetAsync(string link)
        {
            Link = link.Replace(".", "/");

            // Run the Kaggle command to download the dataset
            var pythonProcess = new System.Diagnostics.Process() // Change variable name from 'process' to 'pythonProcess'
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"kaggle datasets download -d {Link} -p ./\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            pythonProcess.Start();

            // Wait for the download to finish
            await pythonProcess.WaitForExitAsync();

            // Rename the downloaded file to dataset.zip
            var fileName = Link.Split('/').Last() + ".zip";
            System.IO.File.Move(fileName, "dataset.zip", true);

            // The dataset is downloaded as a zip file, so you'll need to unzip it to access the data
            // This code assumes that the dataset contains a single CSV file
            var zipPath = "dataset.zip";
            var csvPath = "dataset.csv";
            if (System.IO.File.Exists(csvPath))
            {
                System.IO.File.Delete(csvPath);
            }
            using (var archive = System.IO.Compression.ZipFile.OpenRead(zipPath))
            {
                var entry = archive.Entries.First();
                using (var entryStream = entry.Open())
                using (var fileStream = System.IO.File.Create(csvPath))
                {
                    entryStream.CopyTo(fileStream);
                }
            }

            // Count the number of rows in the CSV file
            RowCount = System.IO.File.ReadLines(csvPath).Count();

            // Path to the Python interpreter
            string pythonPath = "/usr/bin/python3"; // Change this to the path of your Python interpreter

            // Path to the Python script
            string scriptPath = "script.py"; // Change this to the path of your Python script

            // Create a new process start info
            var startInfo = new ProcessStartInfo(pythonPath)
            {
                Arguments = $"\"{scriptPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            // Start the process
            using (var pythonProcess2 = Process.Start(startInfo)) // Rename the variable 'process' to 'pythonProcess2'
            {
                // Read the output of the script
                string result = pythonProcess2.StandardOutput.ReadToEnd();

                // Wait for the process to finish
                pythonProcess2.WaitForExit();
            }
            ImagePath = "/correlation_matrix.png"; // Update the ImagePath to the correct path

        }
    }
}