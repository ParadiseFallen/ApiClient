using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient.Http
{
    public class ExperementalHttpClient : HttpMessageInvoker
    {
        public ExperementalHttpClient(HttpMessageHandler handler) : base(handler)
        {
        }
        public ExperementalHttpClient(HttpMessageHandler handler,bool disposeHandler) : base(handler,disposeHandler) 
        {

        }
    }
}
