using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace rdp_ncentral_tool_custom
{
    internal class WebClient
    {
        public static string PutRequest(string UriPath, string content, string contentType)
        {
            HttpClient Hclient = new HttpClient();
            //Hclient.Timeout = TimeSpan.FromMilliseconds(2000);

            HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Put, "https://" + LaunchArgs.serverID + "/dms/rest/customProtocol/acknowledge?tokenId=" + LaunchArgs.ackToken)
            {
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = Hclient.Send(webRequest);
            response.EnsureSuccessStatusCode();
            StreamReader sReader = new StreamReader(response.Content.ReadAsStream());
            string responseMessage = sReader.ReadToEnd();
            sReader.Close();
            return responseMessage;
        }

        public static string GetRequest(string UriPath, int Timeout = 5000)
        {
            HttpClient Hclient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            Hclient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            Hclient.DefaultRequestHeaders.UserAgent.ParseAdd("CPH C#");
            Hclient.DefaultRequestHeaders.ConnectionClose = false;
            Hclient.Timeout = TimeSpan.FromMilliseconds(Timeout);

            HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Get, UriPath);
            HttpResponseMessage response = Hclient.Send(webRequest);
            response.EnsureSuccessStatusCode();

            StreamReader sReader = new StreamReader(response.Content.ReadAsStream());
            string responseMessage = sReader.ReadToEnd();
            sReader.Close();
            return responseMessage;

        }
    }
}
