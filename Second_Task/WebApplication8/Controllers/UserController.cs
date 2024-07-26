using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserInfo()
        {
            var users = new List<User>
            {
                new User { Id = 1, Name = "dima", Email = "dima@gmail.com" },
                new User { Id = 2, Name = "bassam", Email = "bassam@gmail.com" },
                new User { Id = 3, Name = "suha", Email = "suha@gmail.com" },
                new User { Id = 4, Name = "noor", Email = "noor@gmail.com" }
            };

            return View(users);
        }
    }
}
