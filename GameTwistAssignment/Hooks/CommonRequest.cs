using GameTwistAssignment.Setting;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTwistAssignment.Hooks
{
    public class CommonRequest
    {
        enum consentType
        {
            GeneralTermsAndConditions,
            DataPrivacyPolicy,
            MarketingProfiling
        }
        private Settings _settings;
        public CommonRequest(Settings settings)
        {
            _settings = settings;
        }
        public void Post_Request(string url)
        {
            _settings.Request = new RestRequest(url, Method.POST);
            _settings.Request.AddHeader("Content-type", "application/json");
            _settings.Request.RequestFormat = DataFormat.Json;
        }
        public void Post_Request_With_QueryParams(string url)
        {
            _settings.Request = new RestRequest(url, Method.POST);
            _settings.Request.AddQueryParameter("consentType", consentType.GeneralTermsAndConditions.ToString());
            _settings.Request.AddQueryParameter("accepted", "true");
        }
        public void Get_Request_With_QueryParams(string url)
        {
            _settings.Request = new RestRequest(url, Method.GET);
            _settings.Request.AddQueryParameter("consentType", consentType.GeneralTermsAndConditions.ToString());
        }



    }
}
