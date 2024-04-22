using System;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KaggleDatasetsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public KaggleDatasetsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string search)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"https://www.kaggle.com/api/datasets/list?search={search}");

                request.Headers.Add("Authorization",
                "Bearer bd20ffda00471552397df6e57c89fefd"
                ); // Replace [Your Kaggle API Token] with your actual Kaggle API token

                var client = _clientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    ExpandoObject? data = JsonConvert.DeserializeObject<ExpandoObject>(responseString);
                    if (data != null)
                    {
                        IDictionary<string, object>? dictionary = data as IDictionary<string, object>;
                        if (dictionary != null && dictionary.ContainsKey("datasets"))
                        {
                            return Ok(dictionary["datasets"]);
                        }
                    }
                }
                else
                {
                    return BadRequest("The response from the Kaggle API doesn't contain a 'datasets' property.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception message
                Console.WriteLine(ex.Message);
                // Return a 500 Internal Server Error response
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
            return BadRequest("An unexpected error occurred.");
        }
    }
}