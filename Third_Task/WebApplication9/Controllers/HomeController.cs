using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication9.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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

        public ActionResult SubmitDataContact(FormCollection form)
        {
            // Pass data back to the view using ViewBag
            ViewBag.Name = form["name"];
            ViewBag.Age = form["age"];
            ViewBag.gender = form["gender"];
            ViewBag.country = form["country"];
            ViewBag.languages = form["languages"];

            return View("Contact");
        }

      
        public ActionResult Login(FormCollection form)
        {
            return View();
        
        }
        public ActionResult SubmitLogin(FormCollection form)
        {
            String []arr1 = { "bassambanyali@gmail.com", "banyali" };
            if (form["Email"] == arr1[0] && form["Password"] == arr1[1])
            {
                Session["name"] = "yes";
                 return RedirectToAction("Index");
            }
            else
               return  RedirectToAction("Login");
        }
        public ActionResult LogOut()
        {
            Session["name"] = "no";
            return RedirectToAction("Index");
         
        }

    }
}

