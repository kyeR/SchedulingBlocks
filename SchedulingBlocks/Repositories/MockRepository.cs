using SchedulingBlocks.Interfaces;
using SchedulingBlocks.Models.AppDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchedulingBlocks.Repositories
{
    public class MockRepository : IAppDbRepository
    {
        public string SubmittedStatus => "Submitted";

        public string PaymentRequestedStatus => "Payment Requested";

        public string AcceptedStatus => "Accepted";

        public string RejectedStatus => "Rejected";

        public void AddReservation(Reservation reservation)
        {
        }

        public void DeleteOrphanedReservations()
        {
        }

        public void DeleteReservation(Reservation reservation)
        {
        }

        public List<Facility> GetAllFacilities()
        {
            var facilities = new List<Facility>();
            var facility = new Facility()
            {
                FacilityName = "Test Facility",
                Id = 1
            };
            facilities.Add(facility);
            return facilities;
        }

        public List<ReservedSlot> GetAllValidReservedSlots()
        {
            var slots = new List<ReservedSlot>();
            var slot = new ReservedSlot()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(3),
                Reservation = new Reservation(),
                Id = 1,
                Facility = "Test Facility"
            };
            slots.Add(slot);
            return slots;
        }

        public Reservation GetReservationById(int id)
        {
            return new Reservation();
        }

        public Reservation GetReservationByIdAndEmail(int id, string email)
        {
            return new Reservation();
        }

        public List<ReservedSlot> GetValidReservedSlotsByFacility(string facility)
        {
            var slots = new List<ReservedSlot>();
            var slot = new ReservedSlot()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(3),
                Reservation = new Reservation(),
                Id = 1,
                Facility = "Test Facility"
            };
            slots.Add(slot);
            return slots;
        }

        public int SaveChanges()
        {
            return 0;
        }
    }
}