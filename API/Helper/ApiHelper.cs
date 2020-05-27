using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace API
{
    public static class ApiHelper
    {
        private static string WowSecret = "Wjq6hL6VdQlGBFEfZqWs2Cf8zsaLgpYM";
        private static string WowClientID = "d8a4545db82a4cdf9255b5ea517c0eee";

        public static string GetAccesToken()
        {
            var client = new RestClient("https://eu.battle.net/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&client_id={WowClientID}&client_secret={WowSecret}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var tokenResponse = JsonConvert.DeserializeObject<AccesTokenResponse>(response.Content);
            return tokenResponse.Access_Token;
        }
    }
}
