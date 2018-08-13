using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace APD.Aspire.Client
{
   public static class ServerInstance 
    {
        public static HttpClient GetServerInstance()
        {
            var configuration = ConfigurationManager.AppSettings["ServerEndPoint"];
            var client = new HttpClient
            {
                BaseAddress = new Uri(configuration)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
