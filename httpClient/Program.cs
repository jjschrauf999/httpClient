using Newtonsoft.Json;

namespace Program 
{ 
    class Program 
    { 
         private static void Main() 
         { 
              RunAsync().GetAwaiter().GetResult();
         } 
         private static async Task RunAsync() 
         {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> header1 = new KeyValuePair<string, string>("a","1");
            KeyValuePair<string, string> header2 = new KeyValuePair<string, string>("b","2");
            headers.Add(header1);
            headers.Add(header2);

            HttpClient httpClient = new HttpClient()
            {
               BaseAddress = new Uri("https://www.google.com/")
            };

            foreach(KeyValuePair<string, string> header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            HttpClientExtension httpClientExtension = new HttpClientExtension()
            {
               _client = httpClient
            };

            string json = "{\"id\": null,\"league\": 1,\"season\": 2023,\"name\": null,\"code\": null,\"search\": null}";
            object obj = JsonConvert.DeserializeObject<Parameters>(json);

            await httpClientExtension.SendRequest(HttpMethod.Get, "request", obj);
            await httpClientExtension.SendRequestData(HttpMethod.Get, "requestData", obj);
         }
         private class Parameters
         {
               public int? id {get;set;}
               public int? league {get;set;}
               public int? season {get;set;}
               public string name {get;set;}
               public string code {get;set;}
               public string search {get;set;}
         }
     } 
}
