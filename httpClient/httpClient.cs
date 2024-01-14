using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace HttpClientExtension
{
    public class HttpClientExt
    {
        static HttpClient _client = new HttpClient();
        const string _authorization = "Authorization";
        const string _soapAction = "SOAPAction";
        const string _contentType = "application/json";

        public static void InitializeHttpClient(string baseURI, List<KeyValuePair<string, string>> headers)
        {
            _client.BaseAddress = new Uri(baseURI);
            foreach(KeyValuePair<string, string> header in headers)
            {
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
        public static async Task<HttpResponseMessage> SendRequest (HttpMethod method, string route)
        {
            return await SendRequest(method, route, null);
        }
        public static async Task<HttpResponseMessage> SendRequestData (HttpMethod method, string route, object requestData)
        {
            return await SendRequest(method, route, requestData);
        }
        private static async Task<HttpResponseMessage> SendRequest (HttpMethod method, string route, object requestData)
        {
            try
            {
                switch (method)
                {
                    case HttpMethod get when get == HttpMethod.Get:
                        return _client.GetAsync(route).Result;
                    case HttpMethod post when post == HttpMethod.Post:
                        HttpContent postData = GetRequestData(requestData);
                        return await _client.PostAsync(route, postData);
                    case HttpMethod put when put == HttpMethod.Put:
                        HttpContent putData = GetRequestData(requestData);
                        return _client.PutAsync(route, putData).Result;
                    case HttpMethod patch when patch == HttpMethod.Patch:
                        HttpContent patchData = GetRequestData(requestData);
                        return _client.PatchAsync(route, patchData).Result;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error with response " + route + ". " + e.Message);
            }
        }
        static HttpContent GetRequestData (object requestData)
        {
            try
            {
                HttpContent httpData;
                string data = JsonConvert.SerializeObject(requestData);
                httpData = new StringContent(data, Encoding.UTF8, _contentType);
                httpData.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
                return httpData;
            }
            catch (Exception e)
            {
                throw new Exception("Could not serialize request data " + requestData + ". " + e.Message);
            }
        }
    }
}