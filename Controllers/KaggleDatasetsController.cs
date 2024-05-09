using System;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DotNetEnv;
using System.Web; // Add this line to import the HttpUtility class
using Microsoft.Extensions.Logging; // Add this line to import the ILogger class
using HtmlAgilityPack;
using System.IO; // Add this line to import the System.IO namespace
using System.Diagnostics; // Add this line to import the System.Diagnostics namespace
using System.Collections.Generic; // Add this line to import the System.Collections.Generic namespace

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KaggleDatasetsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<KaggleDatasetsController> _logger; // Add this line to import the ILogger class

        public KaggleDatasetsController(IHttpClientFactory clientFactory, ILogger<KaggleDatasetsController> logger) // Add the ILogger<KaggleDatasetsController> logger parameter to the constructor
        {
            _clientFactory = clientFactory;
            _logger = logger; // Assign the logger parameter to the _logger field
        }

        [HttpGet]
        public async Task<IActionResult> Get(string search)
        {
            try
            {
                string cmd = $"-c \"kaggle datasets list -s {search} | grep {search}\"";

                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = cmd,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };

                process.Start();

                var datasets = new List<string>();
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    datasets.Add(line);
                }

                process.WaitForExit();

                return Ok(datasets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to search for datasets.");
                return StatusCode(500, "An error occurred while trying to search for datasets." + ex);
            }
        }
    }
}