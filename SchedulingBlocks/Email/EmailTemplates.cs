using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SchedulingBlocks.Models;
using SchedulingBlocks.Models.AppDb;

namespace SchedulingBlocks.Email
{
    public static class EmailTemplates
    {
        public static string InvalidPaymentCustomerNotification(Reservation reservation)
        {
            //TODO: Add business contact info
            var sb = new StringBuilder();
            sb.Append(@"<h4>Dear ");
            sb.Append(reservation.CustomerInfo.FirstName);
            sb.Append(@" ");
            sb.Append(reservation.CustomerInfo.LastName);
            sb.Append(@"</h4>");
            sb.Append(@"<p>Your recent payment to Moore Batting Cage has been flagged as invalid, ");
            sb.Append(@"due to a mismatch between the order total and the PayPal payment that was received.");
            sb.Append(@"Please contact Moore Batting Cage at [Phone number] to resolve this issue. ");
            sb.Append(@"You may need to submit your order again to gain access to the facility. ");
            sb.Append(@"Your order information is below.</p>");
            sb.Append(@"<h4>Order Items</h4>");
            foreach (var slot in reservation.ReservedSlots)
            {
                sb.Append(slot.ToLineItemString());
                sb.Append(" Price: $");
                sb.Append(slot.GetSlotPrice(reservation.PricePer));
                sb.Append("<br/>");
            }
            sb.Append(@"<h4>Order Total</h4>");
            sb.Append("$");
            sb.Append(reservation.OrderTotal);
            sb.Append(@"<h4>Payment Submitted</h4>");
            sb.Append("$");
            sb.Append(reservation.AmountPaid);
            sb.Append(@"<h4>Transaction ID</h4>");
            sb.Append(reservation.TransactionId);
            sb.Append(@"<p>Moore Batting Cage</p>");
            return sb.ToString();
        }

        public static string InvalidPaymentBusinessNotification(Reservation reservation)
        {
            var sb = new StringBuilder();
            sb.Append(@"<h4>Invalid Payment Notification</h4>");
            sb.Append(@"<p>A recent payment to Moore Batting Cage has been flagged as invalid, ");
            sb.Append(@"due to a mismatch between the order total and the PayPal payment amount that was relayed to the site.");
            sb.Append(@"An email has also been sent to the customer. No access to the facility will be created at this time.</p>");
            sb.Append(@"<h4>Customer Information</h4><p>");
            sb.Append(reservation.CustomerInfo.FirstName);
            sb.Append(@" ");
            sb.Append(reservation.CustomerInfo.LastName);
            sb.Append(@"<br/>");
            sb.Append(reservation.CustomerInfo.Email);
            sb.Append(@"</p><h4>Order Items</h4><p>");
            foreach (var slot in reservation.ReservedSlots)
            {
                sb.Append(slot.ToLineItemString());
                sb.Append(" Price: $");
                sb.Append(slot.GetSlotPrice(reservation.PricePer));
                sb.Append("<br/>");
            }
            sb.Append(@"</p>");
            sb.Append(@"<h4>Order Total</h4>");
            sb.Append("$");
            sb.Append(reservation.OrderTotal);
            sb.Append(@"<h4>Payment Submitted</h4>");
            sb.Append("$");
            sb.Append(reservation.AmountPaid);
            sb.Append(@"<h4>Transaction ID</h4>");
            sb.Append(reservation.TransactionId);
            return sb.ToString();
        }

        public static string OrderNotFoundNotification(PayPalCheckoutInfo info)
        {
            var sb = new StringBuilder();
            sb.Append(@"<h4>Order Not Found Notification</h4>");
            sb.Append(@"<p>A recent PayPal payment notification recieved by the Moore Batting Cage website was not able to be matched with a pending order. ");
            sb.Append(@"This is most likely due to a problem with the web site, and the system administrator should be notified. ");
            sb.Append(@"The data received in the payment notification is listed below.</p>");
            sb.Append(@"<h4>PayPal Information</h4><p>");
            sb.Append(@"Transaction ID: ");
            sb.Append(info.txn_id);
            sb.Append(@"<br/>");
            sb.Append(@"Reservation/Order ID: ");
            sb.Append(info.custom);
            sb.Append(@"<br/>");
            sb.Append(@"Customer First Name: ");
            sb.Append(info.first_name);
            sb.Append(@"<br/>");
            sb.Append(@"Customer Last Name: ");
            sb.Append(info.last_name);
            sb.Append(@"<br/>");
            sb.Append(@"Customer Email: ");
            sb.Append(info.payer_email);
            sb.Append(@"<br/>");
            sb.Append(@"Payment Total: ");
            sb.Append(info.Total);
            sb.Append(@"<br/>");
            return sb.ToString(); 
        }
    }
}