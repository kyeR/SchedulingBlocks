using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchedulingBlocks.Models.AppDb;

namespace SchedulingBlocks.Interfaces
{
    public interface IAppDbRepository
    {
        string SubmittedStatus { get; }
        string PaymentRequestedStatus { get; }
        string AcceptedStatus { get; }
        string RejectedStatus { get; }
        Reservation GetReservationById(int id);
        Reservation GetReservationByIdAndEmail(int id, string email);
        void DeleteOrphanedReservations(); 
        void AddReservation(Reservation reservation);
        void DeleteReservation(Reservation reservation);

        List<ReservedSlot> GetAllValidReservedSlots();
        List<ReservedSlot> GetValidReservedSlotsByFacility(string facility);

        List<Facility> GetAllFacilities();
        
        int SaveChanges();
    }
}
