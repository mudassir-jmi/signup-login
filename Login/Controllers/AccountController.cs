using Login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Login.Controllers
{
    [AllowAnonymous]

    public class AccountController : Controller
    {
        public ActionResult SignUp()
        {
            return View();
        }


        [HttpPost]

        public ActionResult SignUp(User model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new OfficeEntities())
                {
                    // Hash the password
                    model.Password = HashPassword(model.Password);

                    context.User.Add(model);
                    context.SaveChanges();
                }

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // Method to hash the password
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            // Check if both Username and Password are provided
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                // Add a validation error message for missing input
                ModelState.AddModelError("", "Username and Password are required.");
                return View();
            }

            using (var context = new OfficeEntities())
            {
                // Hash the entered password
                string hashedPassword = HashPassword(model.Password);

                // Check if the username exists and the password matches
                bool isValid = context.User.Any(x => x.Username == model.Username && x.Password == hashedPassword);

                if (isValid)
                {
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                    ViewBag.issucces = "Login SuccessFull!";
                    return RedirectToAction("Index", "Employees");
                }

                ModelState.AddModelError("", "Invalid Username and Password");
                return View();
            }
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}