using System;

namespace KC.Domain.Common.Finance
{
    public class CalcResponse
    {
        public decimal PaymentAmount { get; set; }

        public decimal FinalPaymentAmount { get; set; }

        public decimal APR { get; set; }

        public decimal TotalOfPaymentsAmount { get; set; }

        public decimal RegZAmount { get; set; }

        public decimal TotalFinanceChargeAmount { get; set; }

        public DateTime FirstPaymentDate { get; set; }

        public DateTime FinalPaymentDate { get; set; }
    }
}
