using Business.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheMarvel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            PersonagemBusiness personagemBusiness = new PersonagemBusiness();
            

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}