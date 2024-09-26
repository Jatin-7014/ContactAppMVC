using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContactAppMVC.Data;
using ContactAppMVC.Models;
using NHibernate.Mapping.ByCode.Impl;

namespace ContactAppMVC.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetContactById()
        {
            // Check if the cookie exists and has a value
            HttpCookie cookie = Request.Cookies["Cookie"];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Cookie not found or invalid.");
            }

            try
            {
                using (var session = NHibernateHelper.CreateSession())
                {
                    Guid userId = Guid.Parse(cookie.Value);
                    var contacts = session.Query<Contact>().Where(c => c.User.Id == userId).ToList();

                    if (contacts.Count == 0)
                    {
                        return HttpNotFound("No contacts found for this user.");
                    }

                    // Create DTOs from the contacts
                    var target = contacts.Select(c => new
                    {
                        c.Id,
                        c.FName,
                        c.LName,
                        c.IsActive,
                        // You can include other properties as necessary
                        ContactDetails = c.ContactDetails.Select(cd => new
                        {
                            cd.Id, // Replace with actual properties of ContactDetail
                            cd.Type, // Replace with actual properties of ContactDetail
                            cd.Value       // Add other properties as needed
                        }).ToList()
                    }).ToList();

                    return Json(target, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here) and return a server error
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }
        public ActionResult GetContact(int id)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var target = session.Query<Contact>().FirstOrDefault(c => c.Id == id);

                if (target == null)
                {
                    return HttpNotFound("Contact not found.");
                }

                if (!target.IsActive)
                {
                    return Json(new { success = false, message = "Contact is not active." }, JsonRequestBehavior.AllowGet);
                }

                // Manually create the DTO
                var convertedTarget = new
                {
                    target.Id,
                    target.FName,
                    target.LName,
                    target.IsActive,
                    // Include ContactDetails if needed
                    ContactDetails = target.ContactDetails.Select(cd => new
                    {
                        // Replace with actual properties of ContactDetail
                        cd.Id, // e.g. cd.PhoneNumber
                        cd.Type,
                        cd.Value// e.g. cd.Email
                                      // Add other properties as needed
                    }).ToList()
                };

                return Json(convertedTarget, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetContacts(Guid userId)
        {

            using (var session = NHibernateHelper.CreateSession())
            {
                var contacts = session.Query<Contact>().Where(c => c.User.Id == userId).ToList();

                return View(contacts);
            }
        }


        public ActionResult Create(Contact contact)
        {
            HttpCookie Id = Request.Cookies["Cookie"];
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    contact.User.Id = Guid.Parse(Id.Value);
                    session.Save(contact);
                    txn.Commit();
                    return Json(new { success = true });
                }
            }
        }
        public ActionResult Edit(int id)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var contact = session.Get<Contact>(id);
                return View(contact);
            }
        }
        [HttpPost]
        public ActionResult Edit(Contact contact)
        {
            HttpCookie Id = Request.Cookies["Cookie"];
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    contact.User.Id = Guid.Parse(Id.Value);
                    session.Update(contact);
                    txn.Commit();
                    return Json(new { success = true });
                }
            }
        }
        [HttpPost]
        public JsonResult EditContactStatus(int userId, bool isActive)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var targetContact = session.Get<Contact>(userId);
                using (var txn = session.BeginTransaction())
                {
                    targetContact.IsActive = isActive;
                    session.Update(targetContact);
                    txn.Commit();
                    return Json(new { success = true });
                }
            }
        }

    }
}