using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using SchedulingBlocks.ContactsApi;
using Newtonsoft.Json;
using SchedulingBlocks.Models;
using SchedulingBlocks.Models.AppDb;
using SchedulingBlocks.Services;
using System.IO;
using SchedulingBlocks.Models.ViewModels;
using System.Configuration;

namespace SchedulingBlocks.Controllers
{
    public class ReservationController : Controller
    {
        private FacilityService _facilities;
        private ReservationService _reservations;
        private string _cartCookieName = "shoppingcart";
        public ReservationController()
        {
            _facilities = new FacilityService();
            _reservations = new ReservationService();
        }

        public ActionResult Index()
        {
            var cart = GetCart();
            var IsIntroModalShown = Session["IntroModalShown"] as String;
            if (IsIntroModalShown == null)
            {
                Session["IntroModalShown"] = "yes";
            }
            else
            {
                ViewBag.IntroModalShown = "yes";
            }
            return View(cart);
        }

        public ActionResult Days()
        {
            List<Day> days = new List<Day>();

            var StartDay = DateTime.Today;
            var EndDay = StartDay.AddDays(30);

            int DifferenceFromLastSunday = DayOfWeek.Sunday - StartDay.DayOfWeek;
            var LastSunday = StartDay.AddDays(DifferenceFromLastSunday).Date;
            for (var date = LastSunday; date.Date < StartDay.Date; date = date.AddDays(1))
            {
                var allReserved = new List<ReservedSlot>();
                allReserved.Add(new ReservedSlot
                {
                    StartTime = date.Date,
                    EndTime = date.Date.AddHours(24)
                });
                var locationDay = new LocationDay()
                {
                    Day = date.Date,
                    ReservedSlots = allReserved,
                };
                days.Add(new Day { Locations = new List<LocationDay> { locationDay } });
            }

            var cart = GetCart();

            for (DateTime date = StartDay; date.Date <= EndDay.Date; date = date.AddDays(1))
            {
                var locations = new List<LocationDay>();
                foreach (var facility in _facilities.GetAllFacilities())
                {
                    var dbSlots = _reservations.GetExistingReservationsForFacility(facility.FacilityName).ToList();
                    var cartReserved = cart.Slots != null && cart.Slots.Count() > 0 ?
                                       cart.Slots.Where(s => s.DayOfReservation == date.Date)
                                                 .Where(s => s.Facility.ToLower().CompareTo(facility.FacilityName.ToLower()) == 0)
                                                 .ToList() :
                                       new List<ReservedSlot>();
                    var dbReserved = dbSlots != null && dbSlots.Count() > 0 ?
                                     dbSlots.Where(s => s.DayOfReservation == date.Date).ToList() :
                                     new List<ReservedSlot>();
                    var reserved = cartReserved.Concat(dbReserved).ToList();
                    var locationDay = new LocationDay()
                    {
                        Location = facility,
                        Day = date.Date,
                        ReservedSlots = reserved
                    };
                    locations.Add(locationDay);
                }
                days.Add(new Day { Locations = locations });
            }
            return View("~/Views/Reservation/DaysPartial.cshtml", days);
        }

        public ActionResult Slots(string date)
        {
            var day = DateTime.Parse(date);
            var cart = GetCart();

            List<LocationDay> locations = new List<LocationDay>();
            foreach (var facility in _facilities.GetAllFacilities())
            {
                var dbSlots = _reservations.GetExistingReservationsForFacility(facility.FacilityName).ToList();
                var cartReserved = cart.Slots != null && cart.Slots.Count() > 0 ?
                                   cart.Slots.Where(s => s.DayOfReservation == day.Date)
                                             .Where(s => s.Facility.ToLower().CompareTo(facility.FacilityName.ToLower()) == 0)
                                             .ToList() :
                                   new List<ReservedSlot>();
                var dbReserved = dbSlots != null && dbSlots.Count() > 0 ?
                                 dbSlots.Where(s => s.DayOfReservation == day.Date).ToList() :
                                 new List<ReservedSlot>();
                var reserved = cartReserved.Concat(dbReserved).ToList();
                var locationDay = new LocationDay()
                {
                    Location = facility,
                    Day = day,
                    ReservedSlots = reserved
                };
                locations.Add(locationDay);
            }

            ViewBag.SellingUnitMinutes = _reservations.SellingUnitMiutes;
            return View("~/Views/Reservation/SlotsPartial.cshtml", locations);
        }

        public CartModel GetCart()
        {
            var c = Request.Cookies[_cartCookieName];
            var cart = c != null ? JsonConvert.DeserializeObject<CartModel>(c.Value) : new CartModel();
            if (cart.Slots != null)
            {
                cart.Slots.RemoveAll(s => s.StartTime < DateTime.Now);
            }
            return cart;
        }

        public void DeleteCart()
        {
            var c = Request.Cookies[_cartCookieName];
            c.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c);
        }

        public void SaveCart(CartModel model)
        {
            model.Slots = model.Slots.OrderBy(x => x.EndTime).ToList();
            var json = JsonConvert.SerializeObject(model);
            var c = new HttpCookie(_cartCookieName, json);
            c.Expires = DateTime.UtcNow.AddDays(365);
            Response.SetCookie(c);
        }

        [HttpPost]
        public ActionResult AddSlotsToCart(CartModel model)
        {
            var existingCart = GetCart();

            //Merge the added slots with the existing slots and recalculate prices
            if (existingCart.Slots != null)
            {
                model.Slots.AddRange(existingCart.Slots);
            }

            SaveCart(model);

            return Json(new {result = "Redirect", url = Url.Action("Cart", "Reservation")});
        }

        public ActionResult Cart()
        {
            var cart = GetCart();
            var reservation = new Reservation();
            reservation.ReservedSlots = cart.Slots != null ? cart.Slots : new List<ReservedSlot>();
            if (cart.Slots != null && cart.Slots.Any())
            {
                reservation.PricePer = _reservations.GetUnitPrice(_reservations.GetNumberOfSellingUnits(cart.Slots, _reservations.SellingUnitMiutes));
            }
            return View(reservation);
        }

        public ActionResult CustomerInfo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CustomerInfo(string firstName, string lastName, string phone, string sport, string email)
        {
            var customer = new Customer();
            customer.FirstName = firstName;
            customer.LastName = lastName;
            customer.Sport = sport;
            customer.Email = email;
            customer.Phone = phone;

            var cart = GetCart();

            if (_reservations.IsSlotInCartAlreadyTaken(cart))
            {
                DeleteCart();
                TempData["InvalidCart"] = "Invalid";
                return RedirectToAction("Index");
            }

            var reservation = _reservations.CreateReservation(cart, customer);

            TempData["Reservation"] = reservation;

            return RedirectToAction("confirm");
        }

        public ActionResult Confirm()
        {
            var model = TempData["Reservation"] as Reservation;
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteSlotsFromCart(List<string> slotIds )
        {
            var cart = GetCart();
            foreach (var id in slotIds)
            {
                var itemToRemove = cart.Slots.FirstOrDefault(s => s.CartItemId == id);
                if (itemToRemove != null)
                {
                    cart.Slots.Remove(itemToRemove);
                }
            }

            SaveCart(cart);

            return new EmptyResult();

        }

        [HttpPost]
        public JsonResult OrderSubmission(OrderSubmissionViewModel submission)
        {
            var cart = GetCart();
            if (_reservations.IsSlotInCartAlreadyTaken(cart))
            {
                DeleteCart();
                var successResult = new {success = "false"};
                return Json(successResult);
            }
            _reservations.SaveReservationSubmission(submission.ReservationId, submission.ReservationEmail);
            DeleteCart();
            var result = new {success = "true"};
            return Json(result);
        }

        [HttpPost]
        public EmptyResult OrderNotification(PayPalCheckoutInfo info)
        {
            byte[] parameters = Request.BinaryRead(Request.ContentLength);
            var accepted = _reservations.ProcessReservationPayment(info, parameters);

            return new EmptyResult();
        }
    }
}
