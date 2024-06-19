using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyCalculator
{
    public class ApiClient
    {
        private static readonly HttpClient client = new HttpClient();
        public string Response { get; private set; }

        public ApiClient(string url = "http://api.nbp.pl/api/exchangerates/tables/A/?format=json")
        {
            Task.Run(() => FetchData(url)).Wait();
        }

        private async Task FetchData(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Response = await response.Content.ReadAsStringAsync();
            }
        }
    }
}
