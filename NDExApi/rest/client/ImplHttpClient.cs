using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NDExApi.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NDExApi.rest.client
{
    internal class ImplHttpClient : RestClientInterface
    {
        private HttpClient client { get; set; }
        private string baseUrl { get; set; }
        
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd H:mm:ss,fff",
            NullValueHandling = NullValueHandling.Ignore
        };
        
        internal ImplHttpClient(string baseUrl, IWebProxy proxy, Authentication auth)
        {
            this.baseUrl = baseUrl;
            HttpClientHandler httpClientHandler;

            if (proxy != null)
            {
                httpClientHandler = new HttpClientHandler
                {
                    UseProxy = true,
                    Proxy = proxy,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
            }
            else
            {
                httpClientHandler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
            }
            
            client = new HttpClient(httpClientHandler);

            // Stop here if client does not use authentication
            if (auth == null) return;
            
            if (auth.OAuth != null)
            {
                // Use Google OAuth
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", auth.OAuth);
            }
            else
            {
                // Use default login data
                byte[] authByteArray = Encoding.UTF8.GetBytes(auth.Username + ':' + auth.Password);
                string authString = Convert.ToBase64String(authByteArray);
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Basic", authString);                    
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
            MediaTypeWithQualityHeaderValue jsonHeader = new MediaTypeWithQualityHeaderValue("application/json");
            
            try
            {
                client.DefaultRequestHeaders.Accept.Add(jsonHeader);
                return await ExecuteAsync<T>(restRequest);
            }
            finally
            {
                client.DefaultRequestHeaders.Accept.Remove(jsonHeader);
            }
        }
        
        private async Task<HttpResponseMessage> SendRequest(RestRequest rest)
        {
            HttpResponseMessage response;
            HttpContent content = rest.contentBody != null
                ? new StringContent(rest.contentBody, Encoding.UTF8, "application/json")
                : null;
            switch (rest.method)
            {
                default:
                    response = await client.GetAsync(baseUrl + rest.url);
                    break;
                case RestMethod.POST:
                    response = await client.PostAsync(baseUrl + rest.url, content);
                    break;
                case RestMethod.PUT:
                    response = await client.PutAsync(baseUrl + rest.url, content);
                    break;
                case RestMethod.DELETE:
                    if (content != null) response = await DeleteWithBodyWorkaround(baseUrl + rest.url, content);
                    else response = await client.DeleteAsync(baseUrl + rest.url);
                    
                    break;
            }

            return response;
        }

        /// HTTP DELETE requests should not have bodies,
        /// but /networkset/{networkid}/members requires it to remove networks from a network set
        private async Task<HttpResponseMessage> DeleteWithBodyWorkaround(string restUrl, HttpContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Delete,
                RequestUri = new Uri(restUrl, UriKind.RelativeOrAbsolute)
            };
            return await client.SendAsync(request);
        }

        private async Task<T> HandleResponse<T>(RestRequest request, HttpResponseMessage response)
        {
            string responseJson = await response.Content.ReadAsStringAsync();
            HandleErrors(request, response, responseJson);
            return JsonConvert.DeserializeObject<T>(responseJson, JsonSettings);
        }
        
        private async Task<RestResponse> HandleResponse(RestRequest request, HttpResponseMessage response)
        {
            string responseJson = await response.Content.ReadAsStringAsync();
            HandleErrors(request, response, responseJson);
            return new RestResponse
            {
                statusCode = response.StatusCode,
                wasSuccess = response.IsSuccessStatusCode,
                contentType = response.Content.Headers.ContentType != null ? response.Content.Headers.ContentType.MediaType : null,
                json = responseJson
            };
        }

        private void HandleErrors(RestRequest request, HttpResponseMessage response, string responseJson)
        {
            if (response.IsSuccessStatusCode) return;
            string type = response.Content.Headers.ContentType.MediaType;
            if (type != "application/json")
            {
                throw new NDExException("Error on " + request.method + " - " + baseUrl + request.url +
                                        ": HTTP " + (int) response.StatusCode + " ---> " + response.ReasonPhrase);
            }
            
            IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd H:mm:ss,fff"
            };

            NDExException exception = JsonConvert.DeserializeObject<NDExException>(responseJson, dateTimeConverter);
            exception = new NDExException(exception, "Error on " + request.method + " - " + baseUrl + 
                                                     request.url + ": HTTP " + (int) response.StatusCode);
            throw exception;
        }

        /*
        public async Task<RestResponse<NetworkCx>> GetStreamedAsync(RestRequest rest)
        {
            HttpResponseMessage response = await client.GetAsync(
                baseUrl + '/' + rest.url, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                return new RestResponse<T>
                {
                    statusCode = response.StatusCode,
                    content = await response.Content.ReadAsStringAsync()
                };
            }
            
            StringBuilder output = new StringBuilder(10000);

            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.Value != null)
                    {
                        output.AppendFormat("\nToken: {0}, Value: {1}", jsonReader.TokenType, jsonReader.Value);
                    }
                    else
                    {
                        output.AppendFormat("\nToken: {0}", jsonReader.TokenType);
                    }
                }
            }

            RestResponse restResponse = new RestResponse
            {
                statusCode = response.StatusCode,
                content = output.ToString()
            };
            return restResponse;
        }*/
    }
}