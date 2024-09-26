using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ContactAppMVC.Data;
using ContactAppMVC.Models;
using ContactAppMVC.ViewModel;

namespace ContactAppMVC.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(LoginVM loginVM)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var user = session.Query<User>().FirstOrDefault(u => u.FName == loginVM.UserName);
                HttpCookie cookie = new HttpCookie("Cookie");
                cookie.Value = (user.Id).ToString();
                cookie.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(cookie);

                if (user != null && BCrypt.Net.BCrypt.Verify(loginVM.Password, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(loginVM.UserName, true);
                    return RedirectToAction("Index", "User");
                }
                ModelState.AddModelError("", "UserName/Password doesn't exist");
                return View();
            }
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(User user)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                if (user.IsAdmin == true)
                {
                    user.Role.User = user;
                    user.Role.RoleName = "Admin";
                }
                else
                {
                    user.Role.User = user;
                    user.Role.RoleName = "Staff";
                }
                using (var txn = session.BeginTransaction())
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    session.Save(user);
                    txn.Commit();
                    return RedirectToAction("LogIn");
                }
            }
        }
        public ActionResult LogOut()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("LogIn");
            }
        }
    }
}