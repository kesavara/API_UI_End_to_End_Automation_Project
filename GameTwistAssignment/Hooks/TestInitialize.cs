using BoDi;
using GameTwistAssignment.Setting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
namespace GameTwistAssignment.Hooks
{
    [Binding]
    public class TestInitialize
    {

        private Settings _settings;
        


        public TestInitialize(Settings settings)
        {
            _settings = settings;
           
        }

        [BeforeScenario]
        public void TestSetup()
        {
            var base_url = "https://www.gametwist.com/nrgs/en/api/";
            // _settings.BaseUrl = new Uri(ConfigurationManager.AppSettings["baseUrl"].ToString());
            _settings.BaseUrl = new Uri(base_url);
            _settings.RestClient.BaseUrl = _settings.BaseUrl;
           
        }

    }
}
