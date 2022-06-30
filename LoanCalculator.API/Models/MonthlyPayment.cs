using System;

namespace LoanCalculator.API.Models
{
    public class MonthlyPayment
    {
        public double Taksit { get; set; }

        public double InterestPayment { get; set; }
        public double Anapara { get; set; }
        public double Bakiye { get; set; }

    }
}
