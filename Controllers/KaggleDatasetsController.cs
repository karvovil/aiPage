using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Dynamic;

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
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://www.kaggle.com/api/datasets/list?search={search}");

            request.Headers.Add("Authorization",
            "Bearer bd20ffda00471552397df6e57c89fefd"
            ); // Replace [Your Kaggle API Token] with your actual Kaggle API token

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            dynamic? data = new ExpandoObject();

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<dynamic?>(responseString);
                if (data?.datasets != null)
                {
                    return Ok(data.datasets);
                }
            }

            if (data?.datasets != null)
            {
                return Ok(data.datasets);
            }
            else
            {
                return BadRequest("The response from the Kaggle API doesn't contain a 'datasets' property.");
            }
        }
    }
}