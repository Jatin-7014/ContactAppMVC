using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactAppMVC.Data;
using ContactAppMVC.Models;
using NHibernate.Mapping.ByCode.Impl;

namespace ContactAppMVC.Controllers
{
    public class ContactDetailController : Controller
    {
        // GET: ContactDetail
        public ActionResult Index(int id)
        {
            Session["contactId"] = id;
            return View();
        }

        public ActionResult GetDetails(int id)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var details = session.Query<ContactDetail>()
                    .Where(cd => cd.Contact.Id == id)
                    .ToList();

                // Check if any details were found
                if (!details.Any())
                {
                    return HttpNotFound("No details found for the specified contact.");
                }

                // Manually create the DTO
                var targetDetails = details.Select(cd => new
                {
                    cd.Id,           // Assuming ContactDetail has an Id property
                    cd.Type,    // Replace with actual properties
                    cd.Value,    // Replace with actual properties
                                     // Add other properties as needed
                }).ToList();

                return Json(targetDetails, JsonRequestBehavior.AllowGet);
            }
        }


        //public ActionResult GetDetails(int id)
        //{
        //    using (var session = NHibernateHelper.CreateSession())
        //    {
        //        var Detail = session.Query<ContactDetail>().Where(cd => cd.Contact.Id == id).ToList();
        //        var targetDetail = Detail.Select(cd => Mapper.ToDetailDTO(cd)).ToList();
        //        return Json(targetDetail, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpGet]
        public ActionResult GetData(int page, int rows, string sidx, string sord, bool _search,
            string searchField, string searchString, string searchOper)
        {

            var id = (int)Session["contactId"];
            using (var session = NHibernateHelper.CreateSession())
            {
                var details = session.Query<ContactDetail>().Where(cd => cd.Contact.Id == id).ToList();

                var contactDetailList = details;

                if (_search && searchField == "Type" && searchOper == "eq")
                {
                    contactDetailList = details.Where(p => p.Type == searchString).ToList();
                }
                int totalCount = details.Count();

                int totalPages = (int)Math.Ceiling((double)totalCount / rows);

                switch (sidx)
                {
                    case "Type":
                        contactDetailList = sord == "asc" ? contactDetailList.OrderBy(p => p.Type).ToList()
                            : contactDetailList.OrderByDescending(p => p.Type).ToList();
                        break;
                    case "Description":
                        contactDetailList = sord == "asc" ? contactDetailList.OrderBy(p => p.Value).ToList()
                            : contactDetailList.OrderByDescending(p => p.Value).ToList();
                        break;

                    default:
                        break;

                }

                var jsonData = new
                {
                    total = totalPages,
                    page = page,
                    records = totalCount,
                    rows = contactDetailList.Select(detail => new
                    {
                        id = detail.Id,
                        cell = new string[]
                                {
                                    detail.Id.ToString(),
                                    detail.Type,
                                    detail.Value,
                                }
                    }).Skip((page - 1) * rows).Take(rows).ToArray()
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetContactDetails(int contactId)
        {
            Session["contactId"] = contactId;
            using (var session = NHibernateHelper.CreateSession())
            {
                var details = session.Query<ContactDetail>().Where(cd => cd.Contact.Id == contactId).ToList();
                return View(details);
            }
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public ActionResult Add(ContactDetail contactDetail)
        {
            int id = (int)Session["contactId"];
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    contactDetail.Contact.Id = id;
                    session.Save(contactDetail);
                    txn.Commit();
                    return Json(new { success = true, message = "Contact Detail Added Successfully" });
                }
            }

        }

        public ActionResult Edit(int id)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var detail = session.Get<ContactDetail>(id);
                return View(detail);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public ActionResult Edit(ContactDetail contactDetail)
        {
            int id = (int)Session["contactId"];
            using (var session = NHibernateHelper.CreateSession())
            {
                var targetDetail = session.Query<ContactDetail>().FirstOrDefault(cd => cd.Id == contactDetail.Id);
                using (var txn = session.BeginTransaction())
                {
                    if (targetDetail != null)
                    {
                        targetDetail.Type = contactDetail.Type;
                        targetDetail.Value = contactDetail.Value;
                        session.Update(targetDetail);
                        txn.Commit();
                    }
                    return Json(new { success = true, message = "Contact Details edited successfully" });
                }
            }

        }



        [Authorize(Roles = "Staff")]
        public ActionResult Delete(int id)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    var details = session.Get<ContactDetail>(id);
                    session.Delete(details);
                    txn.Commit();
                    return Json(new { success = true, message = "Product Deleted Successfully" });
                }
            }
        }
    }
}