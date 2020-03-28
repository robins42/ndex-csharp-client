using System.Collections;
using System.Collections.Generic;
using NDExApi.rest;

namespace NDExApiTests.utils
{
    public class RestClientTheories : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { RestImplementation.HttpClient };
            //yield return new object[] { RestImplementation.WebClient };
            //yield return new object[] { RestImplementation.HttpWebRequest };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}