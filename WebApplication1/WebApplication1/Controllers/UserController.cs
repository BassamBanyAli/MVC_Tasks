using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.IO;
using Newtonsoft.Json;
using System.Data.Entity;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        Users_InformationEntities1 db = new Users_InformationEntities1();
        // GET: User
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            return View(users);
        }
        public ActionResult Login()
        {
            return View();
        }


        public ActionResult Create() {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User model)
        {
            if (ModelState.IsValid)
            {
                var users = db.Users.ToList();
                // Replace `PasswordHash` with actual hashed password validation logic
                foreach (var user in users)
                {
                    // Check if email matches
                    if (user.Email == model.Email && user.PasswordHash == model.PasswordHash)
                    {
                        Session["name"] = "yes";
                        var userInfo = new User
                        {
                            UserID = user.UserID,
                            UserName = user.UserName,
                            PasswordHash = user.PasswordHash,
                            Email = user.Email,
                            ImagePath = user.ImagePath // Adjust if necessary
                        };

                        // Serialize UserInfo to JSON
                        var userInfoJson = JsonConvert.SerializeObject(userInfo);

                        // Create and configure the cookie
                        var authCookie = new HttpCookie("AuthCookie")
                        {
                            Value = userInfoJson,
                            Expires = DateTime.Now.AddHours(1),
                            Secure = true, // Requires HTTPS
                            HttpOnly = true // Prevents JavaScript access to the cookie
                        };
                        Response.Cookies.Add(authCookie);

                        return RedirectToAction("Index");
                    }

                }


            }
            return View(model);
        }



        public ActionResult LogOut()
        {
            Session.Remove("name");
            return RedirectToAction("Index");
        }




        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User model, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                // Check if password and repeat password match
                var repeatPassword = Request.Form["RepeatPassword"];
                if (model.PasswordHash != repeatPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View(model);
                }

                // Save image if uploaded
                if (Image != null && Image.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    Image.SaveAs(path);
                    model.ImagePath = fileName; // Store image path or filename
                }

                // Save user data to database
                db.Users.Add(model);
                db.SaveChanges();

                // Redirect to login page or another appropriate action
                return RedirectToAction("Login");
            }

            return View(model);
        }


        [HttpGet]
        public ActionResult Profile()
        {
            // Retrieve the cookie
            var cookie = Request.Cookies["AuthCookie"];
            if (cookie != null)
            {
                // Deserialize the cookie value into a User object
                var userInfoJson = cookie.Value;
                var user = JsonConvert.DeserializeObject<User>(userInfoJson);

                // Pass the user information to the view
                return View(user);
            }

            // Redirect to login if no cookie is found
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Profile([Bind(Include = "UserID,UserName,Email,PasswordHash")] User model, HttpPostedFileBase ImagePath)
        {
            if (ModelState.IsValid)
            {
                // Retrieve user information from the cookie
                var cookie = Request.Cookies["AuthCookie"];
                if (cookie != null)
                {
                    var userInfoJson = cookie.Value;
                    var cookieUser = JsonConvert.DeserializeObject<User>(userInfoJson);

                    // Fetch the user record from the database
                    var user = db.Users.Find(cookieUser.UserID);

                    if (user != null)
                    {
                        // Update user properties with the new data from the form
                        user.UserName = model.UserName;
                        user.Email = model.Email;
                        user.PasswordHash = model.PasswordHash;

                        // Handle file upload
                        if (ImagePath != null && ImagePath.ContentLength > 0)
                        {
                            // Define the path where the file will be saved
                            var fileName = Path.GetFileName(ImagePath.FileName);
                            var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                            ImagePath.SaveAs(path);

                            // Update the image path in the user model
                            user.ImagePath = fileName;
                        }

                        // Mark modified properties
                        db.Entry(user).Property(u => u.UserName).IsModified = true;
                        db.Entry(user).Property(u => u.Email).IsModified = true;
                        db.Entry(user).Property(u => u.PasswordHash).IsModified = true;
                        db.Entry(user).Property(u => u.ImagePath).IsModified = true;

                        // Save changes to the database
                        db.SaveChanges();

                        // Optionally, update the cookie if needed
                        var updatedUserInfoJson = JsonConvert.SerializeObject(user);
                        cookie.Value = updatedUserInfoJson;
                        Response.Cookies.Set(cookie);

                        // Redirect to a success page or back to profile page
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User session has expired.");
                }
            }

            // If model state is invalid or other issues occur, return the view with the model
            return View(model);
        }


        public ActionResult Password()
        {

        return View();
        }
        [HttpPost]
        public ActionResult Password(string PasswordHash, string NewPassword, string ConfirmPassword)
        {
            // Retrieve the cookie
            var cookie = Request.Cookies["AuthCookie"];
            if (cookie != null)
            {
                // Deserialize the cookie value into a User object
                var userInfoJson = cookie.Value;
                var user = JsonConvert.DeserializeObject<User>(userInfoJson);

                var UserRecord = db.Users.Find(user.UserID);

                if (UserRecord == null)
                {
                    return HttpNotFound();
                }
                else if (UserRecord.PasswordHash == PasswordHash && NewPassword == ConfirmPassword)
                {
                    // Update the password in the database
                    UserRecord.PasswordHash = NewPassword;

                    // Mark the entity as modified
                    db.Entry(UserRecord).Property(u => u.PasswordHash).IsModified = true;

                    // Save changes to the database
                    db.SaveChanges();

                    // Update the cookie with the new password
                    user.PasswordHash = NewPassword;
                    var updatedUserInfoJson = JsonConvert.SerializeObject(user);
                    var newCookie = new HttpCookie("AuthCookie", updatedUserInfoJson)
                    {
                        Expires = DateTime.Now.AddHours(1) // Set the cookie expiration as needed
                    };
                    Response.Cookies.Set(newCookie);

                    return RedirectToAction("Profile");
                }
            }

            // Redirect to login if no cookie is found
            return RedirectToAction("Login");
        }

        }
}

