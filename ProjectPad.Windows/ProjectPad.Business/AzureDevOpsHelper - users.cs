using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;

namespace ProjectPad.Business
{
    partial class AzureDevOpsHelper
    {


        public class GetTeamMembersResponse
        {
            public Value[] value { get; set; }
            public int count { get; set; }
        }

        public class Value
        {
            public TeamMemberIdentity identity { get; set; }
        }

        public class TeamMemberIdentity
        {
            public string displayName { get; set; }
            public string url { get; set; }
            public _Links _links { get; set; }
            public string id { get; set; }
            public string uniqueName { get; set; }
            public string imageUrl { get; set; }
            public string descriptor { get; set; }
        }

        public class _Links
        {
            public Avatar avatar { get; set; }
        }

        public class Avatar
        {
            public string href { get; set; }
        }


        public static TeamMemberIdentity[] GetTeamMembers(string baseAdress, string project, string teamName)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(baseAdress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = GetCredentials();

                    HttpResponseMessage response = client.GetAsync($"_apis/projects/"+ project +"/teams/" + teamName + "/members?api-version=5.1").Result;


                    if (response.IsSuccessStatusCode)
                    {

                        var value = response.Content.ReadAsStringAsync().Result;

                        var qr = JsonConvert.DeserializeObject<GetTeamMembersResponse>(value);
                        return (from z in qr.value
                                select z.identity).ToArray();
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;

        }


        public static User[] GetUsers()
        {
            try
            {


                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = GetCredentials();

                    HttpResponseMessage response = client.GetAsync($"https://vssps.dev.azure.com/simplement-e/_apis/graph/users?api-version=5.1-preview.1&subjectTypes=aad,msa,imp").Result;


                    if (response.IsSuccessStatusCode)
                    {

                        var value = response.Content.ReadAsStringAsync().Result;

                        var qr = JsonConvert.DeserializeObject<GetUserResponse>(value);
                        return qr.value;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }


        private class GetUserResponse
        {
            public int count { get; set; }
            public User[] value { get; set; }
        }

        public class User
        {
            public string subjectKind { get; set; }
            public string metaType { get; set; }
            public string directoryAlias { get; set; }
            public string domain { get; set; }
            public string principalName { get; set; }
            public string mailAddress { get; set; }
            public string origin { get; set; }
            public string originId { get; set; }
            public string displayName { get; set; }
            public UserLinks _links { get; set; }
            public string url { get; set; }
            public string descriptor { get; set; }
        }

        public class UserLinks
        {
            public UserLinkValue self { get; set; }
            public UserLinkValue memberships { get; set; }
            public UserLinkValue membershipState { get; set; }
            public UserLinkValue storageKey { get; set; }
            public UserLinkValue avatar { get; set; }
        }

        public class UserLinkValue
        {
            public string href { get; set; }
        }





    }
}
