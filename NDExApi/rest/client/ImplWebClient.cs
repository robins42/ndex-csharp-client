using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NDExApi.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NDExApi.rest.client
{
    internal class ImplWebClient : RestClientInterface
    {
        private MyWebClient client { get; set; }
        private string baseUrl { get; set; }
        
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd H:mm:ss,fff",
            NullValueHandling = NullValueHandling.Ignore
        };
        
        internal ImplWebClient(string baseUrl, IWebProxy proxy, Authentication auth)
        {
            this.baseUrl = baseUrl;

            if (proxy != null)
            {
                client = new MyWebClient
                {
                    Proxy = proxy,
                    Encoding = Encoding.UTF8
                };
            }
            else
            {
                client = new MyWebClient
                {
                    Encoding = Encoding.UTF8
                };
            }

            client.Headers[HttpRequestHeader.ContentType] = "application/json";
//            client.Headers[HttpRequestHeader.AcceptCharset] = "UTF-8";

            // Stop here if client does not use authentication
            if (auth == null) return;
            client.UseDefaultCredentials = false;
            
            if (auth.OAuth != null)
            {
                // Use Google OAuth
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + auth.OAuth;
            }
            else
            {
                // Use default login data
                byte[] authByteArray = Encoding.UTF8.GetBytes(auth.Username + ':' + auth.Password);
                string authString = Convert.ToBase64String(authByteArray);
                client.Headers[HttpRequestHeader.Authorization] = "Basic " + authString;
            }
        }

        public async Task<T> ExecuteAsync<T>(RestRequest rest)
        {
            HttpResponseMessage response = await SendRequest(rest);
            return await HandleResponse<T>(rest, response);
        }
        
        public async Task<RestResponse> ExecuteAsync(RestRequest rest)
        {
            HttpResponseMessage response = await SendRequest(rest);
            return await HandleResponse(rest, response);
        }
        
        /// This JSON header breaks other calls for some reason so the JSON header is only used temporarily 
        public async Task<T> ExecuteForJsonAsync<T>(RestRequest restRequest)
        {
            string oldValue = client.Headers[HttpRequestHeader.Accept];
            
            try
            {
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                return await ExecuteAsync<T>(restRequest);
            }
            finally
            {
                client.Headers[HttpRequestHeader.Accept] = oldValue;
            }
        }
        
        private async Task<HttpResponseMessage> SendRequest(RestRequest rest)
        {
            try
            {
                string response;
                switch (rest.method)
                {
                    case RestMethod.GET:
                        response = await client.DownloadStringTaskAsync(baseUrl + rest.url);
                        break;
                    default:
                        // POST, PUT, DELETE
                        response = await client.UploadStringTaskAsync(baseUrl + rest.url, rest.method.ToString(), rest.contentBody ?? string.Empty);
                        break;
                }

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response, Encoding.UTF8, "application/json"),
                };
            }
            catch (WebException ex)
            {
                string responseFromServer = ex.Message + " ";
                
                if (ex.Response == null)
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = responseFromServer
                    };
                }

                using (WebResponse r = ex.Response)
                {
                    Stream dataRs = r.GetResponseStream();
                    if (dataRs == null)
                    {
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ReasonPhrase = responseFromServer
                        };
                    }
                    using (StreamReader reader = new StreamReader(dataRs))
                    {
                        responseFromServer = reader.ReadToEnd();
                    }
                }
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = responseFromServer
                };
            }
        }

        private async Task<T> HandleResponse<T>(RestRequest request, HttpResponseMessage response)
        {
            HandleErrors(request, response);
            string json = await response.Content.ReadAsStringAsync();
//            string newJson = Decompress(json);
            
            return JsonConvert.DeserializeObject<T>(json, JsonSettings);
        }
        
        private async Task<RestResponse> HandleResponse(RestRequest request, HttpResponseMessage response)
        {
            HandleErrors(request, response);
            return new RestResponse
            {
                statusCode = response.StatusCode,
                wasSuccess = response.IsSuccessStatusCode,
                contentType = response.Content.Headers.ContentType != null ? response.Content.Headers.ContentType.MediaType : null,
                json = await response.Content.ReadAsStringAsync()
            };
        }

        private void HandleErrors(RestRequest request, HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;
//            string type = response.Content.Headers.ContentType.MediaType;
            if (!response.ReasonPhrase.StartsWith("{"))
            {
                throw new NDExException("Error on " + request.method + " - " + baseUrl + request.url +
                                        ": HTTP " + (int) response.StatusCode + " ---> " + response.ReasonPhrase);
            }
            
            IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd H:mm:ss,fff"
            };

            NDExException exception = JsonConvert.DeserializeObject<NDExException>(response.ReasonPhrase, dateTimeConverter);
            exception = new NDExException(exception, "Error on " + request.method + " - " + baseUrl + 
                                                     request.url + ": HTTP " + (int) response.StatusCode);
            throw exception;
        }
    }

    internal class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            if (request != null) request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return request;
        }
    }
}