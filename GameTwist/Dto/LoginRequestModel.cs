using System;
using System.Collections.Generic;
using System.Text;

namespace GameTwistAssignment.Hooks
{
    class LoginRequestModel
    {
        public string nickname { get; set; }
        public string password { get; set; }
        public bool autologin { get; set; }

    }
}
