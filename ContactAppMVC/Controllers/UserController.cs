using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactAppMVC.Data;
using ContactAppMVC.Models;

namespace ContactAppMVC.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        [Authorize]
        public ActionResult Index()
        {
            using(var session = NHibernateHelper.CreateSession())
            {
                return View();
            }
        }
        public ActionResult Create(User user)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    user.Password=BCrypt.Net.BCrypt.HashPassword(user.Password);
                    user.Role.User = user;
                    user.Role.RoleName = user.IsAdmin ? "Admin" : "Staff";
                    session.Save(user);
                    txn.Commit();
                    return RedirectToAction("Index");
                }
            }
        }
        [HttpPost]
        public JsonResult EditStatus(Guid userId, bool isActive)
        {
            try
            {
                using (var session = NHibernateHelper.CreateSession())
                {
                    var targetUser = session.Query<User>().FirstOrDefault(u => u.Id == userId);
                    using (var txn = session.BeginTransaction())
                    {
                        targetUser.IsActive = isActive;
                        session.Update(targetUser);
                        txn.Commit();
                        return Json(new { success = true });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult Edit(Guid userId)
        {
            using(var session= NHibernateHelper.CreateSession())
            {
                var user=session.Get<User>(userId);
                return View(user);
            }
        }
        [HttpPost]
        public ActionResult Edit(User user)
        {
            using(var session=NHibernateHelper.CreateSession())
            {
                using(var txn = session.BeginTransaction())
                {
                    var targetUser = session.Get<User>(user.Id);
                    var targetRole = session.Get<Role>(targetUser.Role.Id);
                    targetRole.RoleName = user.IsAdmin ? "Admin" : "Staff";
                    targetUser.FName = user.FName;
                    targetUser.LName = user.LName;
                    targetUser.IsAdmin = user.IsAdmin;
                    session.Update(targetUser);
                    txn.Commit();
                    return RedirectToAction("GetAllUsers");
                }
            }
        }
        public ActionResult GetAllUsers()
        {
            using(var session=NHibernateHelper.CreateSession())
            {
                var users=session.Query<User>().ToList();
                return View(users);
            }
        }
        public ActionResult GetAdmins()
        {
            using(var session = NHibernateHelper.CreateSession())
            {
                var admins=session.Query<User>().Where(u=>u.IsAdmin==true).ToList();
                return View(admins);
            }
        }
    }
}