using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPad.Business
{
    public static class GraphApiHelper
    {

        public class GetUserResponse
        {
            public string odatacontext { get; set; }
            public object[] businessPhones { get; set; }
            public string displayName { get; set; }
            public string givenName { get; set; }
            public string jobTitle { get; set; }
            public string mail { get; set; }
            public string mobilePhone { get; set; }
            public object officeLocation { get; set; }
            public string preferredLanguage { get; set; }
            public string surname { get; set; }
            public string userPrincipalName { get; set; }
            public string id { get; set; }


            public byte[] ImageData { get; set; }
        }

        



        public static async Task<GetUserResponse> GetMe()
        {
            var token = await ProjectPadApplication._tokenProvider.GetGraphApiToken();   


            using(HttpClient cli = new HttpClient())
            {
                cli.DefaultRequestHeaders.Clear();
                cli.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                cli.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var tmp = await cli.GetAsync("https://graph.microsoft.com/v1.0/me");
                var str = tmp.Content.ReadAsStringAsync().Result;

                GetUserResponse rps = JsonConvert.DeserializeObject<GetUserResponse>(str);

                tmp = await cli.GetAsync("https://graph.microsoft.com/v1.0/me/photo/$value");
                using (var stream = await tmp.Content.ReadAsStreamAsync())
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    rps.ImageData = bytes;
                }

                return rps;
            }
        }

    }
}
