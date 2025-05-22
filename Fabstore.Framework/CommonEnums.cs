namespace Fabstore.Framework
    {
    public static class CommonEnums
        {
        public enum OrderStatusType
            {
            Placed = 1,
            Shippped = 2,
            Delivered = 3,
            Cancelled = 4
            }

        public enum PaymentStatusType
            {
            Pending = 1,
            Completed = 2,
            Failded = 3
            }

        public enum PaymentMethodType
            {
            UPI = 1,
            CreditCard = 2,
            DebitCard = 3,
            COD = 4
            }

        public enum GenderType
            {
            Men = 1,
            Women = 2
            }
        }
    }
