using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NDExApi.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NDExApi.rest.client
{
    internal class ImplHttpWebRequest : RestClientInterface
    {
        private string baseUrl { get; set; }
        private IWebProxy proxy { get; set; }
        private Authentication auth { get; set; }

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd H:mm:ss,fff",
            NullValueHandling = NullValueHandling.Ignore
        };

        internal ImplHttpWebRequest(string baseUrl, IWebProxy proxy, Authentication auth)
        {
            this.baseUrl = baseUrl;
            this.proxy = proxy;
            this.auth = auth;
        }

        private HttpWebRequest Init(string url)
        {
            HttpWebRequest client = (HttpWebRequest) WebRequest.Create(url);
            ServicePointManager.Expect100Continue = false;

            client.ContentType = "application/json";
            if(proxy != null) client.Proxy = proxy;

            // Stop here if client does not use authentication
            if (auth == null) return client;

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

            return client;
        }

        public async Task<T> ExecuteAsync<T>(RestRequest rest)
        {
            HttpWebRequest client = Init(baseUrl + rest.url);
            HttpResponseMessage response = await SendRequest(rest, client);
            return await HandleResponse<T>(rest, response);
        }

        public async Task<RestResponse> ExecuteAsync(RestRequest rest)
        {
            HttpWebRequest client = Init(baseUrl + rest.url);
            HttpResponseMessage response = await SendRequest(rest, client);
            return await HandleResponse(rest, response);
        }

        /// This JSON header breaks other calls for some reason so the JSON header is only used temporarily 
        public async Task<T> ExecuteForJsonAsync<T>(RestRequest restRequest)
        {
            HttpWebRequest client = Init(baseUrl + restRequest.url);
            client.Headers[HttpRequestHeader.Accept] += "application/json";
            return await ExecuteAsync<T>(restRequest);
        }

        private static async Task<HttpResponseMessage> SendRequest(RestRequest rest, HttpWebRequest client)
        {
            client.Method = rest.method.ToString();
            //client.ContentLength = rest.contentBody != null ? rest.contentBody.Length : 0;

            //client.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            try
            {
                if (rest.contentBody != null)
                {
                    using (Stream requestBody = await client.GetRequestStreamAsync())
                    {
                        byte[] byteBody = StringToByteArray(rest.contentBody);
                        await requestBody.WriteAsync(byteBody, 0, byteBody.Length);
                    }
                }

                string responseText;
                using (HttpWebResponse response = (HttpWebResponse) await client.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseText = await reader.ReadToEndAsync();
                }

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseText, Encoding.UTF8, "application/json")
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

        private static byte[] StringToByteArray(string str)
        {
            return new UTF8Encoding().GetBytes(str);
        }

        private async Task<T> HandleResponse<T>(RestRequest request, HttpResponseMessage response)
        {
            HandleErrors(request, response);
            string json = await response.Content.ReadAsStringAsync();
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
}