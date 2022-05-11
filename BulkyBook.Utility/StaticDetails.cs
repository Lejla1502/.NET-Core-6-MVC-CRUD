using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public static class StaticDetails
    {
        //end user
        public const string Role_User_Indi = "Individual";
        public const string Role_User_Comp = "Company";

        //management
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";


        //order status
        public const string StatusPending = "Pending";          //initial status when order is created
        public const string StatusApproved = "Approved";        //after that, if its a customer, when order is approved, it is changed to this
        public const string StatusInProcess = "Processing";     //in process will be updated by admin when they are processing the order
        public const string StatusShipped = "Shipped";          //when processing is done, order is shipped and that is final status
        public const string StatusCancelled = "Cancelled";      //except for the order is cancelled
        public const string StatusRefunded = "Refunded";        //or needs to be refunded

        //payment status
        public const string PaymentStatusPending = "Pending";   //initially, payment is pending
        public const string PaymentStatusApproved = "Approved"; //once the payment is done, it will be approved
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";  //if its a company account; they will have 30 days to make payment after order is shipped
        public const string PaymentStatusRejected = "Rejected"; //

        //session int count
        public const string SessionCart = "SessionShoppingCart";
    }
}
