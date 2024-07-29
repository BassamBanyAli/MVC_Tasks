using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication9.Models;

namespace WebApplication9.Controllers
{
    public class UsersController : Controller
    {

        private Users_InformationEntities db = new Users_InformationEntities();
        // GET: User
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            return View(users);

        }
        public ActionResult Create() {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user, HttpPostedFileBase ImagePath)
        {
            if (ModelState.IsValid)
            {
                if (ImagePath != null && ImagePath.ContentLength > 0)
                {
                    // Generate a unique filename to avoid conflicts
                    var fileName = Path.GetFileName(ImagePath.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);

                    // Save the file
                    ImagePath.SaveAs(path);

                    // Optionally, store the file path in the database
                    user.ImagePath = fileName;
                }

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        public ActionResult Delete()
        {
            return View();

        }


        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }


        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,UserName,Email,PasswordHash,ImagePath")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}