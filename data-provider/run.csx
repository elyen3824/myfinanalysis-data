#r "System.Web.Http"

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        .Value;

    string symbol = GetValueFromQuery(req, "symbol");

    string start =  GetValueFromQuery(req, "start");

    string end =  GetValueFromQuery(req, "end");

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    string[] symbols = data?.symbols;


    HttpResponseMessage result = GetResponse(req, symbol, symbols, start, end);

    return result;
}

private static string GetValueFromQuery(HttpRequestMessage req, string key)
{
    return req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, key, true) == 0)
        .Value;
}

private static HttpResponseMessage GetResponse(HttpRequestMessage req, string symbol, string[] symbols, string start, string end)
{
    if(symbol == null && symbols == null)
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body");
    
    dynamic urls = GetUrls(symbol, symbols, start, end);

    return req.CreateResponse(HttpStatusCode.OK, urls);
}

private static dynamic GetUrls(string symbol, string[] symbols, string start, string end)
{
    if(symbol !=null)
        return $"https://www.quandl.com/api/v3/datasets/WIKI/{symbol}/data.json?start_date={start}&end_date={end}&collapse=daily&transform=cumul";   
    
    return string.Empty;
}