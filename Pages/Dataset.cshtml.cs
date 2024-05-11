using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.IO;
using System.Linq;

namespace YourNamespace.Pages
{
    public class DatasetModel : PageModel
    {
        public string Link { get; set; }
        public int RowCount { get; set; }
        public async Task OnGetAsync(string link)
        {
            Link = link.Replace(".", "/");

            // Run the Kaggle command to download the dataset
            var process = new System.Diagnostics.Process()
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
            process.Start();

            // Wait for the download to finish
            await process.WaitForExitAsync();

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
        }
    }
}