using NDExApi.model;
using Newtonsoft.Json;

namespace NDExApi.rest
{
    internal class RestRequest
    {
        internal RestMethod method { get; private set; }
        internal string url { get; private set; }
        internal string contentBody { get; private set; }
        private bool _isUrlAlreadyAppended;

        internal RestRequest(RestMethod httpMethod, string url)
        {
            method = httpMethod;
            this.url = url;
        }

        public void AddUrlSegment(string property, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            url += _isUrlAlreadyAppended ? "&" : "?";
            url += property + '=' + value;
            _isUrlAlreadyAppended = true;
        }
        
        public void AddUrlSegment(string property, object value)
        {
            if (value != null) AddUrlSegment(property, value.ToString());
        }

        public void SetContentBody(object body)
        {
            if (body == null) throw new NDExException("Request body cannot be null.");
            contentBody = JsonConvert.SerializeObject(body);
        }
        
        public void SetContentBody(string body)
        {
            contentBody = body;
        }
    }
}