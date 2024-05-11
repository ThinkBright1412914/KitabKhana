using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Utility
{
    public static class RoleDefine
    {
        public const string Role_User_Only = "Individual";
        public const string Role_Company_User = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipping = "Shipped";
        public const string StatusCancel = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayPayment";
        public const string PaymentRejected = "Rejected";

        public const string SessionCart = "ShoppingCart";
    }
}
