using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication8.Controllers
{
    public class SecondController : Controller
    {
        // GET: Second
        public ActionResult Index()
        {
            return View();
        }
          public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SubmitData(string name)
        {
            // Pass data back to the view using ViewBag
            ViewBag.Name = name;
            return View("Index");
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
    }
}