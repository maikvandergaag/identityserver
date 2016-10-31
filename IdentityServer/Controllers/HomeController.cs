using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace IdentityServer.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            return View();
        }

        [Authorize]
        public ActionResult About() {
            return View((User as ClaimsPrincipal).Claims);
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logout() {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }
    }
}