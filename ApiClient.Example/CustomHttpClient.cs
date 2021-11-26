using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient.Example
{
    internal class CustomHttpClient : HttpClient
    {
        public int MyProperty { get; set; }
    }
}
