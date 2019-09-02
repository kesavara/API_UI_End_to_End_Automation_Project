using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;

namespace GameTwistAssignment.Utilities
{
    public static class Libraries
    {

        public static Dictionary<String,String> DeserializeResponse(this IRestResponse restResponse)
        {
            var JSONObj = new JsonDeserializer().Deserialize<Dictionary<string, String>>(restResponse);

            return JSONObj;
        }

        public static string GetResponseObject(this IRestResponse response, string responseObject)
        {
            JObject obs = JObject.Parse(response.Content);
            return obs[responseObject].ToString();
        }
    }
}
