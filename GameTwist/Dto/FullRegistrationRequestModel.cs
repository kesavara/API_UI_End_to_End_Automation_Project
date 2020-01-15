using System;
using System.Collections.Generic;
using System.Text;

namespace GameTwistAssignment.Dto
{
    class FullRegistrationRequestModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool isMale { get; set; }
        public string countryCode { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string street { get; set; }
        public string phonePrefix { get; set; }
        public string phoneNumber { get; set; }
        public string securityQuestionTag { get; set; }
        public string securityAnswer { get; set; }
    }
}
