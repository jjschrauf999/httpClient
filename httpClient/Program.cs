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
            HttpClientExtension.HttpClientExt.InitializeHttpClient("https://www.google.com", headers);
            await HttpClientExtension.HttpClientExt.SendRequest(HttpMethod.Get, "");
         } 
     } 
}
