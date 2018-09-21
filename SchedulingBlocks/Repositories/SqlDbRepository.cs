using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchedulingBlocks.Interfaces;
using SchedulingBlocks.Models;
using SchedulingBlocks.Models.AppDb;
using System.Data.Entity;

namespace SchedulingBlocks.Repositories
{
    public class SqlDbRepository : IAppDbRepository
    {
        private SqlDbContext _context;

        public string SubmittedStatus { get { return "Submitted"; } }
        public string PaymentRequestedStatus { get { return "Payment Requested"; } }
        public string AcceptedStatus { get { return "Accepted"; } }
        public string RejectedStatus { get { return "Rejected"; } }

        public SqlDbRepository(SqlDbContext context)
        {
            _context = context;
        }

        public Reservation GetReservationById(int id)
        {
            var result =
                from r in _context.Reservations
                where r.Id == id
                select r;

            return result.FirstOrDefault();
        }

        public Reservation GetReservationByIdAndEmail(int id, string email)
        {
            var result =
                from r in _context.Reservations
                where r.Id == id
                where r.CustomerInfo.Email.CompareTo(email) == 0
                select r;

            return result.FirstOrDefault();
        }

        public void DeleteOrphanedReservations()
        {
            var old = DateTime.Now.AddMinutes(-30);
            var result =
                from r in _context.Reservations
                where r.Status != AcceptedStatus
                where r.Status != RejectedStatus
                where r.SubmittedTimestamp <= old
                select r;

            var reservations = result.ToList();

            foreach (var reservation in reservations)
            {
                if (reservation.ReservedSlots == null || !reservation.ReservedSlots.Any())
                {
                    continue;
                }
                foreach (var slot in reservation.ReservedSlots)
                {
                    DeleteEntity(slot);
                }

                var custId = reservation.CustomerInfo == null ? 0 : reservation.CustomerInfo.Id;
                DeleteEntity(reservation);
                SaveChanges();
                if (custId != 0)
                {
                    DeleteCustomer(custId);
                }
            }
            SaveChanges();
        }

        public void DeleteCustomer(int id)
        {
            var result =
                from c in _context.Customers
                where c.Id == id
                select c;

            DeleteEntity(result);
            SaveChanges();
        }

        public void DeleteSlots(Reservation reservation)
        {
            var result =
                from s in _context.ReservedSlots
                where s.Reservation.Id == reservation.Id
                select s;

            foreach (var slot in result.ToList())
            {
                DeleteEntity(slot);
                SaveChanges();
            }
        }

        public void AddReservation(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
        }

        public void DeleteReservation(Reservation reservation)
        {
            DeleteEntity(reservation);
            SaveChanges();
        }

        public List<ReservedSlot> GetAllValidReservedSlots()
        {
            var results = _context.Reservations
                            .Where(r => r.Status.CompareTo(PaymentRequestedStatus) == 0 || 
                                r.Status.CompareTo(AcceptedStatus) == 0)
                            .SelectMany(x => x.ReservedSlots);

            return results.ToList();
        }

        public List<ReservedSlot> GetValidReservedSlotsByFacility(string facility)
        {
            var results = _context.Reservations
                            .Where(r => r.Status.CompareTo(PaymentRequestedStatus) == 0 ||
                                r.Status.CompareTo(AcceptedStatus) == 0)
                            .SelectMany(x => x.ReservedSlots)
                            .Where(x => x.Facility.ToLower().CompareTo(facility.ToLower()) == 0);

            return results.ToList();
        }

        public List<Facility> GetAllFacilities()
        {
            var results =
                from f in _context.Facilities
                select f;

            return results.ToList();
        }

        public int SaveChanges()
        {
            var errors = _context.GetValidationErrors();
            return _context.SaveChanges();
        }

        private void DeleteEntity(object entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
        }
    }
}