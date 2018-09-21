using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using SchedulingBlocks.ContactsApi;
using SchedulingBlocks.Email;
using SchedulingBlocks.Models;
using SchedulingBlocks.Models.AppDb;
using Newtonsoft.Json;
using SchedulingBlocks.Interfaces;

namespace SchedulingBlocks.Services
{
    public class ReservationService
    {
        //Ontraport API 
        private ContactManager ContactManager { get; set; }
        private Emailer EmailHelper { get; set; }
        //Dependecy Injection
        private IAppDbRepository _appdb;
        private int _sellingUnitMinutes;

        public int SellingUnitMiutes
        {
            get
            {
                if (_sellingUnitMinutes != 0)
                {
                    return _sellingUnitMinutes;
                }

                _sellingUnitMinutes = Int32.Parse(ConfigurationManager.AppSettings["SellingUnitMinutes"]);
                return _sellingUnitMinutes;
            }
        }

        public ReservationService()
        {
            ContactManager = new ContactManager();
            EmailHelper = new Emailer();
            _appdb = MvcApplication.Container.GetInstance<IAppDbRepository>();
        }

        public double GetUnitPrice(int numberOfUnits)
        {
            //TODO: make a call to a backing store for this stuff
            if (numberOfUnits >= 2)
            {
                return 60.00;
            }
            else
            {
                return 80.00;
            }
        }

        //Helper to count the number of units within a group of time slots
        public int GetNumberOfSellingUnits(IEnumerable<ReservedSlot> slots, int sellingUnitMinutes)
        {
            var minutes = slots.Sum(s => s.TotalSlotMinutes);
            return minutes/sellingUnitMinutes;
        }

        public Reservation GetReservation(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return _appdb.GetReservationById(id);
        }

        public IEnumerable<ReservedSlot> GetAllExistingReservations()
        {
            return _appdb.GetAllValidReservedSlots();
        }

        public IEnumerable<ReservedSlot> GetExistingReservationsForFacility(string facility)
        {
            return _appdb.GetValidReservedSlotsByFacility(facility);
        }

        public Reservation CreateReservation(CartModel cart, Customer customer)
        {
            var reservation = new Reservation();
            var customerInfo = new Customer();
            customerInfo.FirstName = customer.FirstName;
            customerInfo.LastName = customer.LastName;
            customerInfo.Email = customer.Email;
            customerInfo.Sport = customer.Sport;
            customerInfo.Phone = customer.Phone;

            var slots = cart.Slots;

            reservation.ReservedSlots = slots;
            reservation.CustomerInfo = customer;
            reservation.PricePer = GetUnitPrice(reservation.TotalUnits);
            reservation.Status = "Submitted";
            reservation.SubmittedTimestamp = DateTime.Now;
            reservation.PaymentCompletedTimestamp = DateTime.MaxValue;

            _appdb.AddReservation(reservation);
            _appdb.SaveChanges();

            return reservation;
        }

        public void DeleteOrphanedReservations()
        {
            _appdb.DeleteOrphanedReservations();
        }

        public void SaveReservationSubmission(int id, string email)
        {
            var reservation = _appdb.GetReservationByIdAndEmail(id, email);
            reservation.Status = _appdb.PaymentRequestedStatus;
            reservation.SubmittedTimestamp = DateTime.Now;
            _appdb.SaveChanges();
        }

        public bool ProcessReservationPayment(PayPalCheckoutInfo info, byte[] parameters)
        {
            var accepted = false; 

            var model = new PayPalListenerModel();
            model.PayPalCheckoutInfo = info;

            if (parameters != null)
            {
                model.ProcessParameters(parameters);
            }

            if (model.IsVerified && model.IsPaymentCompleted)
            {
                int reservationId;
                if (!Int32.TryParse(info.custom, out reservationId))
                {
                    SendOrderNotFoundNotification(info);
                    return accepted;
                }

                var reservation = GetReservation(reservationId);

                if (reservation == null)
                {
                    SendOrderNotFoundNotification(info);
                    return accepted;
                }
                reservation.TransactionId = info.txn_id;
                reservation.AmountPaid = info.Total;
                var isPaymentValid = reservation.AmountPaid == reservation.OrderTotal;


                if (!isPaymentValid)
                {
                    reservation.Status = _appdb.RejectedStatus;
                    SendInvalidPaymentNotifications(reservation);
                }
                else
                {
                    reservation.Status = _appdb.AcceptedStatus;
                    accepted = true;
                    reservation.PaymentCompletedTimestamp = info.TrxnDate;
                    ContactManager.AddContact(reservation.CustomerInfo);
                    //TODO: Brivo API
                }

                _appdb.SaveChanges();
            }

            return accepted;
        }

        public bool IsSlotInCartAlreadyTaken(CartModel cart)
        {
            DeleteOrphanedReservations();
            var reserved = _appdb.GetAllValidReservedSlots();
            foreach (var slot in cart.Slots)
            {
                foreach (var r in reserved)
                {
                    if ((slot.StartTime <= r.EndTime) && (slot.EndTime >= r.StartTime) && slot.Facility.CompareTo(r.Facility) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SendOrderNotFoundNotification(PayPalCheckoutInfo info)
        {
            //TODO: Add actual business email
            var email = EmailTemplates.OrderNotFoundNotification(info);
            EmailHelper.SendEmail("admin@moorebattingcage.com", "Moore Batting Cage", "Order Not Found Notification", email);
        }

        public void SendInvalidPaymentNotifications(Reservation reservation)
        {
            var customerEmail = EmailTemplates.InvalidPaymentCustomerNotification(reservation);
            var businessEmail = EmailTemplates.InvalidPaymentBusinessNotification(reservation);
            var customerName = reservation.CustomerInfo.FirstName + " " + reservation.CustomerInfo.LastName;
            EmailHelper.SendEmail(reservation.CustomerInfo.Email, customerName, "Invalid Payment", customerEmail);
            EmailHelper.SendEmail("admin@moorebattingcage.com", "Moore Batting Cage", "Invalid Payment", businessEmail);

            //TODO: Add actual business email address
        }
    }
}