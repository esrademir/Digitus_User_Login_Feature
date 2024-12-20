using Digitus_User_Login_Feature.Areas.AdminPanel.Filters;
using Digitus_User_Login_Feature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Digitus_User_Login_Feature.Controllers
{
    //[LoginControll]
    public class HomeController : Controller
    {
        UserLoginModel db = new UserLoginModel();
        // GET: Home
        public ActionResult Index()
        {
            List<UserActivationTime> activationTimes = db.UserActivationTimes.Where(x => x.time != TimeSpan.Zero).ToList();
            TimeSpan SumTime = new TimeSpan();
            foreach (UserActivationTime item in activationTimes)
            {
                SumTime += item.time;
            }
            BaseModel bm = new BaseModel();
            bm.SuccessRegistirationsCount = Math.Round(SumTime.TotalSeconds / activationTimes.Count, 3);


            return View(bm);
        }
    }
}