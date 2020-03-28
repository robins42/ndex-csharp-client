using System;
using System.Globalization;
using System.Net;

namespace NDExApi.rest
{
    public class NDExProxy : IWebProxy
    {
        private Uri address;
        private ICredentials credentials;
        
        public NDExProxy(string address, int port)
        {
            this.address =
                new Uri("http://" + address + ":" + port.ToString(CultureInfo.InvariantCulture));
        }
        
        public NDExProxy(string address, int port, ICredentials credentials)
        {
            Credentials = credentials;
            this.address =
                new Uri("http://" + address + ":" + port.ToString(CultureInfo.InvariantCulture));
        }
        
        public ICredentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        public Uri GetProxy(Uri destination)
        {
            return address;
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}