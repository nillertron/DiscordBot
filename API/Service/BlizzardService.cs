using API.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class BlizzardService
    {
        private string Token;
        public BlizzardService()
        {
            Token = ApiHelper.GetAccesToken();
        }
        public async Task<List<int>> GetCharacterIlvl1(string realm, string characterName)
        {
            var client = new RestClient("https://eu.api.blizzard.com/profile/wow/character/"+realm.Trim()+"/"+characterName.Trim()+"/equipment?namespace=profile-eu&locale=en_EU&access_token="+Token);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {Token}");
            var response = client.Execute(request);
            var answer = response.Content;
            var items = JsonConvert.DeserializeObject<ResponseBody>(response.Content);

            //Metode specifik logik
            var sumIlvl = 0;
            var trækFra = 0;
            items.equipped_items.ForEach(o => {
                if (Convert.ToInt32(o.level["value"]) != 1)
                {
                    sumIlvl += Convert.ToInt32(o.level["value"]);
                }
                else
                    trækFra++;
            }
            );
            var list = new List<int>();
            list.Add(sumIlvl / (items.equipped_items.Count-trækFra));
            list.Add(Convert.ToInt32(items.equipped_items[13].level["value"]));
            return list;
        }
        public async Task<ResponseBody> GetCharacter(string realm, string characterName)
        {
            var client = new RestClient("https://raider.io/api/v1/characters/profile?region=eu&realm="+realm+"&name="+characterName+"&fields=gear%2Cmythic_plus_scores");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            var response = client.Execute(request);
            var answer = response.Content;
            var items = JsonConvert.DeserializeObject<ResponseBody>(response.Content);

            return items;
        }
    }
}
