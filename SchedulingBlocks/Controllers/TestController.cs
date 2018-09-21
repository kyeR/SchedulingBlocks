using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using SchedulingBlocks.Models;

namespace SchedulingBlocks.Controllers
{
    public class TestController : Controller
    {
        public ActionResult OntraportTest()
        {
            return View();
        }

        [System.Web.Http.HttpPost]
        public JsonResult FormSubmit(Customer customer)
        {
            var contactManager = new ContactsApi.ContactManager();
            var succeeded = contactManager.AddContact(customer) ? "true" : "false";
            var result = new { success = succeeded };
            return Json(result, "json");
        }
    }
}
