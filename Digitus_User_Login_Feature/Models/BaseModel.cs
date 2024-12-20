using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digitus_User_Login_Feature.Models
{
    public class BaseModel
    {
        public User user { get; set; }
        public string verificationCode { get; set; }
        public string id { get; set; }
        public double SuccessRegistirationsCount { get; set; }
    }
}