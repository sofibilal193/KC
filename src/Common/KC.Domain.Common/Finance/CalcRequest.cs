using System;

namespace KC.Domain.Common.Finance
{
    public class CalcRequest
    {
        public DateTime ContractDate { get; set; }

        public decimal Amount { get; set; }

        public decimal Rate { get; set; }

        public short DaysToFirstPayment { get; set; }

        public short NumPayments { get; set; }

        public TermFrequency PaymentFrequency { get; set; }

        public CalcMethod CalcMethod { get; set; }

        public CalcRequest()
        {
            ContractDate = DateTime.UtcNow.Date;
            DaysToFirstPayment = 30;
            PaymentFrequency = TermFrequency.Monthly;
            CalcMethod = CalcMethod.Thirty360;
            NumPayments = 60;
        }
    }
}
