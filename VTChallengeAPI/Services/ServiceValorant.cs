using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using NugetVTChallenge.Models.Api;

namespace VTChallenge.Services
{
    public class ServiceValorant : IServiceValorant {

        private HttpClient httpClient;
        private string url;

        public ServiceValorant(HttpClient httpClient) {
            this.httpClient = httpClient;
            this.url = "https://api.henrikdev.xyz/";
        }

        public async Task<UserApi> GetAccountAsync(string username, string tagline) {
            string request = "valorant/v1/account/" + username + "/" + tagline;
            string url = this.url + request;

            var response = await httpClient.GetAsync(url);

            string jsonReponse = await response.Content.ReadAsStringAsync();

            if (jsonReponse == null) {
                return null;
            } else {
                return JsonConvert.DeserializeObject<UserApi>(jsonReponse);
            }
        }

        public async Task<UserApi> GetAccountUidAsync(string uid) {
            string request = "valorant/v1/by-puuid/account/" + uid;
            string url = this.url + request;

            var response = await httpClient.GetAsync(url);

            string jsonReponse = await response.Content.ReadAsStringAsync();

            if (jsonReponse == null) {
                return null;
            } else {
                return JsonConvert.DeserializeObject<UserApi>(jsonReponse);
            }
        }

        public async Task<string> GetRankAsync(string username, string tag) {
            string request = "valorant/v1/mmr-history/eu/" + username + "/" + tag;
            string url = this.url + request;

            var response = await httpClient.GetAsync(url);

            string jsonReponse = await response.Content.ReadAsStringAsync();

            if (jsonReponse == null) {
                return "";
            } else {
                // Parse JSON string to a JObject
                JObject jsonObj = JObject.Parse(jsonReponse);

                // Get the "data" array
                JArray dataArray = (JArray)jsonObj["data"];

                // Get the first object in the "data" array
                JObject dataObj = (JObject)dataArray[0];

                // Get the value of the "currenttier_patched" property
                string currentTierPatched = dataObj.GetValue("currenttierpatched").Value<string>();

                return currentTierPatched;
            }
        }
    }
}
