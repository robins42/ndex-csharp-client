using System;
using Newtonsoft.Json;

namespace NDExApi.model
{
    public class NDExException : Exception
    {
        public ErrorCode errorCode;
        public string description;
        public string stackTrace;
        public string threadId;
        public DateTime timeStamp;
        private string _customMessage;
        
        public string Message
        {
            get { return _customMessage + (InnerException != null ? " ---> " + InnerException.Message : ""); }
        }
        
        [JsonConstructor]
        public NDExException(string message) : base(message)
        {
            _customMessage = message;
        }
        
        public NDExException(Exception e, string message) : base(message, e)
        {
            _customMessage = message;
        }
    }
}