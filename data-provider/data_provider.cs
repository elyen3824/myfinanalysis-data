using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace myfinanalysis.data
{
    public static class Data_provider
    {
        [FunctionName("GetTickData")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string symbol = await GetValueFromQuery(req, "symbol");
            string start = await GetValueFromQuery(req, "start");
            string end = await GetValueFromQuery(req, "end");

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            string[] symbols = data?.symbols;

            dynamic urls = await GetUrls(symbol, symbols, start, end);

            return symbol == null && symbols == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, urls as object);
        }

        private static async Task<string> GetValueFromQuery(HttpRequestMessage req, string key)
        {
            return req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, key, true) == 0)
                .Value;
        }

        private static async Task<dynamic> GetUrls(string symbol, string[] symbols, string start, string end)
        {
            if (symbol != null)
                return $"https://www.quandl.com/api/v3/datasets/WIKI/{symbol}/data.json?start_date={start}&end_date={end}&collapse=daily&transform=cumul";

            return string.Empty;
        }
    }
}