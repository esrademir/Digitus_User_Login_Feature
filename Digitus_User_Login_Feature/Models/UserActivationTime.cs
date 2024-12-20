using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digitus_User_Login_Feature.Models
{
    public class UserActivationTime
    {
        public int ID { get; set; }
        public TimeSpan time { get; set; }
        public DateTime date { get; set; }
        public virtual User User { get; set; }
    }
}