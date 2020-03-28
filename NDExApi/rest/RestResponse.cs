using System.Net;

namespace NDExApi.rest
{
    /// <summary>
    /// <para>Generic response object in case no fitting object to return is available.</para>
    /// The most common use is to check response.wasSuccess or get a value from response.json.
    /// </summary>
    public class RestResponse
    {
        /// <summary>
        /// The response message, can contain CX data, GUIDs or be empty.
        /// </summary>
        public string json;
        
        /// <summary>
        /// The HTTP response code. To check if request was successful, there is also a bool parameter wasSuccess
        /// </summary>
        public HttpStatusCode statusCode;
        
        /// <summary>
        /// Is true if 200 &lt;= statusCode &lt;= 299
        /// </summary>
        public bool wasSuccess;
        
        /// <summary>
        /// Defines the content type of the json object. In most cases it is "application/json"
        /// </summary>
        public string contentType;
    }
}
