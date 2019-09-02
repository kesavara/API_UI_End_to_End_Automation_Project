using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace GameTwistAssignment.Utilities
{
    [Binding]
    class ResponseClass
    {
        public string paymentRedirectUrl { get; set; }
        public string Content { get; set; }

        public string StatusCode { get; set; }
        public string token { get; set; }
    }
}
