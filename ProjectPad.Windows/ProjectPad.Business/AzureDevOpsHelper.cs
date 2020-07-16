using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ProjectPad.Business
{
    public static partial class AzureDevOpsHelper
    {

        public static string[] Prenoms = { "Matthieu", "Dylan", "Michael", "Thomas", "Alexandre", "Zoïa", "Camille" };

        public static string GetIdentityFromPrenom(string prenom)
        {
            switch (prenom.ToLowerInvariant())
            {
                case "matthieu":
                case "mathieu":
                    return "matthieu@simplement-e.fr";
                case "dylan":
                    return "dylan@simplement-e.fr";
                case "michael":
                    return "michael@commerce-hub.io";
                case "alexandre":
                    return "alexandre@altazion.com";
                case "thomas":
                    return "thomas@altazion.com";
            }

            return null;
        }



        private class QueryResponse
        {

            public WorkItemInQueryResponse[] workItems;
            public WorkItemRelation[] workItemRelations;
        }

        private class WorkItemInQueryResponse
        {
            public int id;
            public string url;
        }

        private class WorkItemRelation
        {
            [JsonProperty("source")]
            public WorkItemRelationItem Source { get; set; }

            [JsonProperty("target")]
            public WorkItemRelationItem Target { get; set; }
        }

        private class WorkItemRelationItem
        {
            [JsonProperty("id")]
            public int Id { get; set; }
        }

        public class ItemResponse
        {
            public int id { get; set; }
            public string url { get; set; }
            public int rev { get; set; }
            public ItemFields fields { get; set; }
            public ItemRelation[] relations { get; set; }
            public string Etat { get; set; }
            public Links _links { get; set; }

            public string[] GetTags()
            {
                if (fields == null || string.IsNullOrEmpty(fields.tags))
                    return new string[0];
                string[] ret = fields.tags.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = ret[i].Trim();
                }
                return ret;
            }
        }



        public class Links
        {
            public Self self { get; set; }
            public Workitemupdates workItemUpdates { get; set; }
            public Workitemrevisions workItemRevisions { get; set; }
            public Workitemcomments workItemComments { get; set; }
            public Html html { get; set; }
            public Workitemtype workItemType { get; set; }
            public Fields fields { get; set; }

            public class Self
            {
                public string href { get; set; }
            }

            public class Workitemupdates
            {
                public string href { get; set; }
            }

            public class Workitemrevisions
            {
                public string href { get; set; }
            }

            public class Workitemcomments
            {
                public string href { get; set; }
            }

            public class Html
            {
                public string href { get; set; }
            }

            public class Workitemtype
            {
                public string href { get; set; }
            }

            public class Fields
            {
                public string href { get; set; }
            }
        }




        /// <summary>
        /// Objet de description du champ "value" de l'objet WorkItemDataMore
        /// </summary>
        public class ItemRelation
        {
            public string rel { get; set; }
            public string url { get; set; }
            public Attributes attributes { get; set; }
        }

        /// <summary>
        /// Objet de description du champ "attributes" de l'objet value
        /// </summary>
        public class Attributes
        {
            public string authorizedDate { get; set; }
            public int id { get; set; }
            public string resourceCreatedDate { get; set; }
            public string resourceModifiedDate { get; set; }
            public string revisedDate { get; set; }
            public string comment { get; set; }

        }




        public class ItemFields
        {
            [JsonProperty("System.AreaPath")]
            public string areaPath { get; set; }

            [JsonProperty("System.TeamProject")]
            public string teamProject { get; set; }

            [JsonProperty("System.IterationPath")]
            public string iterationPath { get; set; }

            [JsonProperty("System.WorkItemType")]
            public string workItemType { get; set; }


            [JsonProperty("System.AssignedTo")]
            public Profil assignedTo { get; set; }

            [JsonProperty("System.State")]
            public string state { get; set; }

            [JsonProperty("System.Reason")]
            public string reason { get; set; }


            [JsonProperty("System.CreatedDate")]
            public DateTimeOffset createdDate { get; set; }

            [JsonProperty("System.CreatedBy")]
            public Profil createdBy { get; set; }

            [JsonProperty("System.ChangedDate")]
            public DateTimeOffset changedDate { get; set; }

            [JsonProperty("System.ChangedBy")]
            public Profil changedBy { get; set; }

            [JsonProperty("System.Title")]
            public string title { get; set; }

            [JsonProperty("System.BoardColumn")]
            public string boardColumn { get; set; }

            [JsonProperty("System.BoardColumnDone")]
            public string boardColumnDone { get; set; }

            [JsonProperty("Microsoft.VSTS.Common.Severity")]
            public string severity { get; set; }

            [JsonProperty("Microsoft.VSTS.Common.Priority")]
            public string priority { get; set; }

            [JsonProperty("Microsoft.VSTS.Common.StateChangeDate")]
            public DateTimeOffset stateChangeDate { get; set; }

            [JsonProperty("Microsoft.VSTS.Common.ValueArea")]
            public string valueArea { get; set; }

            [JsonProperty("Microsoft.VSTS.TCM.ReproSteps")]
            public string reproSteps { get; set; }

            [JsonProperty("simplemente.ProcessCreo.RoadmapTitle")]
            public string roadmapTitle { get; set; }

            [JsonProperty("ProcessCreo.RoadmapType")]
            public string roadmapType { get; set; }

            [JsonProperty("ProcessCreo.RoadmapEstPublie")]
            public string roadmapEstPublie { get; set; }

            [JsonProperty("ProcessCreo.RoadmapEstValide")]
            public string roadmapEstValide { get; set; }

            [JsonProperty("ProcessCreo.RoadmapRelease")]
            public string roadmapRelease { get; set; }

            [JsonProperty("ProcessCreo.RoadmapHtmlContent")]
            public string roadmapHtmlContent { get; set; }

            [JsonProperty("ProcessCreo.RoadmapUrlImage")]
            public string roadmapUrlImage { get; set; }

            [JsonProperty("ProcessCreo.NumeroDevis")]
            public string numeroDevis { get; set; }

            [JsonProperty("System.Description")]
            public string description { get; set; }

            [JsonProperty("Custom.DescriptionLongue")]
            public string descriptionLongue { get; set; }

            [JsonProperty("ProcessCreo.DocExplications")]
            public string resume { get; set; }

            [JsonProperty("System.Tags")]
            public string tags { get; set; }

            [JsonProperty("Microsoft.VSTS.Common.BacklogPriority")]
            public long BacklogPriority { get; set; }

            [JsonProperty("Custom.TypeDecouverte")]
            public string TypeDecouverte { get; set; }

            [JsonProperty("Custom.InRelease")]
            public string inRelease { get; set; }

            [JsonProperty("Custom.InReleaseIsHighlighted")]
            public string inReleaseHighlighted { get; set; }

            [JsonProperty("Custom.InReleaseImageUrl")]
            public string inReleaseImageUrl { get; set; }
        }


        public class Profil
        {
            public string displayName { get; set; }
            public string url { get; set; }
            public _Links _links { get; set; }
            public string id { get; set; }
            public string uniqueName { get; set; }
            public string imageUrl { get; set; }
            public string descriptor { get; set; }

            public class _Links
            {
                public Avatar avatar { get; set; }
            }

            public class Avatar
            {
                public string href { get; set; }
            }
        }




        private class GetIterationResponse
        {
            public int count { get; set; }
            public Iteration[] value { get; set; }
        }

        public class Iteration
        {
            public string id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public IterationAttributes attributes { get; set; }
            public string url { get; set; }
        }

        public class IterationAttributes
        {
            public DateTime startDate { get; set; }
            public DateTime finishDate { get; set; }
            public string timeFrame { get; set; }
        }


        public static AuthenticationHeaderValue GetCredentials()
        {
            return new AuthenticationHeaderValue("Bearer", "");
        }

        public class PatchOperation
        {
            public string op { get; set; }
            public string path { get; set; }
        }

        public class SimpleValuePathOperation : PatchOperation
        {
            public string value { get; set; }
        }

        public class ObjectValuePathOperation : PatchOperation
        {
            public RelationsItem value { get; set; }

        }
        public class RelationsItem
        {
            public string rel { get; set; }
            public string url { get; set; }
        }

        public static ItemResponse[] GetAllItemsFromQuery(string baseAdress, string idQuery)
        {
            try
            {
                List<int> idsToGet = new List<int>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAdress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = GetCredentials();

                    HttpResponseMessage response = client.GetAsync("_apis/wit/wiql/" + idQuery + "?$expand=relations&api-version=5.1").Result;

                    if (response.IsSuccessStatusCode)
                    {

                        var value = response.Content.ReadAsStringAsync().Result;

                        QueryResponse qr = JsonConvert.DeserializeObject<QueryResponse>(value);


                        if (qr.workItems != null)
                        {
                            for (int i = 0; i < qr.workItems.Length; i++)
                            {
                                if (!idsToGet.Contains(qr.workItems[i].id))
                                    idsToGet.Add(qr.workItems[i].id);
                            }
                        }

                        if (qr.workItemRelations != null)
                        {
                            for (int i = 0; i < qr.workItemRelations.Length; i++)
                            {
                                if (qr.workItemRelations[i].Target != null)
                                {
                                    if (!idsToGet.Contains(qr.workItemRelations[i].Target.Id))
                                        idsToGet.Add(qr.workItemRelations[i].Target.Id);
                                }

                                if (qr.workItemRelations[i].Source != null)
                                {
                                    if (!idsToGet.Contains(qr.workItemRelations[i].Source.Id))
                                        idsToGet.Add(qr.workItemRelations[i].Source.Id);
                                }

                            }
                        }

                        if (idsToGet.Count == 0)
                            return null;

                        return GetAllItems(baseAdress, idsToGet.ToArray());

                    }

                } // end first using

            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }

        public static ItemResponse[] GetAllItems(string baseAdress, int[] idItemsListe)
        {
            List<ItemResponse> tmp = new List<ItemResponse>();

            StringBuilder blr = new StringBuilder();
            int count = 0;
            foreach (var i in idItemsListe)
            {
                if (blr.Length > 0)
                    blr.Append(",");
                blr.Append(i.ToString());
                count++;
                if (count >= 50)
                {
                    count = 0;
                    tmp.AddRange(GetAllItems(baseAdress, blr.ToString()));
                    blr.Clear();
                }
            }

            if (blr.Length > 0)
                tmp.AddRange(GetAllItems(baseAdress, blr.ToString()));

            return tmp.ToArray();

        }

        public static ItemResponse[] GetAllItems(string baseAdress, string idItemsListe)
        {

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAdress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = GetCredentials();

                    HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + idItemsListe + "&api-version=5.1&$expand=all").Result;

                    if (response.IsSuccessStatusCode)
                    {

                        var value = response.Content.ReadAsStringAsync().Result;


                        GlobalItem gi = JsonConvert.DeserializeObject<GlobalItem>(value);

                        return gi.value;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static int? CreerItem(string baseAdress, string projet, string type, string titreItem, string description, List<string> relations)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAdress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = GetCredentials();

                    List<PatchOperation> patch = new List<PatchOperation>();
                    patch.Add(new SimpleValuePathOperation()
                    {
                        op = "add",
                        path = "/fields/System.Title",
                        value = titreItem
                    });
                    patch.Add(new SimpleValuePathOperation()
                    {
                        op = "add",
                        path = type.Equals("Bug", StringComparison.InvariantCultureIgnoreCase) ? "/fields/Microsoft.VSTS.TCM.ReproSteps" : "/fields/System.Description",
                        value = description
                    });
                    foreach (var r in relations)
                    {
                        patch.Add(new ObjectValuePathOperation()
                        {
                            op = "add",
                            path = "/relations/-",
                            value = new RelationsItem()
                            {
                                rel = "System.LinkTypes.Related",
                                url = "https://simplement-e.visualstudio.com/_apis/wit/workItems/" + r
                            }
                        });
                    }

                    var content = new StringContent(JsonConvert.SerializeObject(patch), Encoding.UTF8, "application/json-patch+json");
                    HttpResponseMessage response = client.PostAsync("/" + projet + "/_apis/wit/workitems/$" + type + "?api-version=5.1", content).Result;


                    if (response.IsSuccessStatusCode)
                    {
                        var value = response.Content.ReadAsStringAsync().Result;
                        var qr = JsonConvert.DeserializeObject<ItemResponse>(value);
                        return qr.id;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private class GlobalItem
        {
            public int count { get; set; }
            public ItemResponse[] value { get; set; }
        }

    }
}
