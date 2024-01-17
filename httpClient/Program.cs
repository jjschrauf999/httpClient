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
               BaseAddress = new Uri("https://www.google.com")
            };

            foreach(KeyValuePair<string, string> header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            HttpClientExtension httpClientExtension = new HttpClientExtension()
            {
               _client = httpClient
            };
            await httpClientExtension.SendRequest(HttpMethod.Get, "");
         } 
     } 
}
