using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using LoanCalculator.API.Models;

namespace LoanCalculator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILogger<LoansController> _logger;
        private readonly IConfiguration Configuration;

        public LoansController(ILogger<LoansController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        // Toplam ödenecek ücret ve faiz miktarı bilgisini içeren yanıtı döndürür 
        [HttpGet]
        public CommonResponse<LoanPayment> Get([FromQuery] uint loanAmount, byte loanTermYear)
        {
            if (loanAmount < 1000 || loanTermYear == 0)
            {
                var res = new CommonResponse<LoanPayment>("Loan amount must be greater than 999 and term must be greater than 0");
                return res;
            }
            
            uint totalNumberOfPayments = loanTermYear * JsonSerializer.Deserialize<uint>(Configuration["MonthsPerYear"]);
            float interestRate = JsonSerializer.Deserialize<float>(Configuration["MontlyInterest"]);
            
            double montlyPayments = calculateMontlyPayment(loanAmount, interestRate, totalNumberOfPayments);
            double totalPayment = Math.Round((totalNumberOfPayments * montlyPayments), 2);
            double interestPayment = Math.Round((totalPayment - loanAmount), 2);
            
            return new CommonResponse<LoanPayment>(new LoanPayment{TotalPayment = totalPayment, InterestPayment = interestPayment});
        }

        // Aylık detaylı ödeme planını içeren yanıtı döndürür
        [HttpGet]
        [Route("GetPaymentPlan")]
        public CommonResponse<List<MonthlyPayment>> GetPaymentPlan([FromQuery] uint loanAmount, byte loanTermYear)
        {
            if (loanAmount < 1000 || loanTermYear == 0)
            {
                var res = new CommonResponse<List<MonthlyPayment>>("Loan amount must be greater than 999 and term must be greater than 0");
                return res;
            }
            
            uint totalNumberOfPayments = loanTermYear * JsonSerializer.Deserialize<uint>(Configuration["MonthsPerYear"]);
            float interestRate = JsonSerializer.Deserialize<float>(Configuration["MontlyInterest"]);
            
            double montlyPayments = calculateMontlyPayment(loanAmount, interestRate, totalNumberOfPayments);
            var bakiye = (double)loanAmount;
            var payments = new List<MonthlyPayment>();
            
            for (int i = 1; i <= totalNumberOfPayments; i++)
            {
                var interestPayment = Math.Round(bakiye * interestRate, 2);
                var anaparaOdeme = Math.Round((montlyPayments - interestPayment), 2);
                bakiye = Math.Round((bakiye - anaparaOdeme), 2);
                if (bakiye < 1) bakiye = 0; 
                var payment = new MonthlyPayment {Taksit=montlyPayments, InterestPayment=interestPayment, Anapara=anaparaOdeme, Bakiye=bakiye};
                payments.Add(payment);
            }
            
            return new CommonResponse<List<MonthlyPayment>>(payments);
        }

        private double calculateMontlyPayment(uint loanAmount, float interestRate, uint totalNumberOfPayments)
        {
            return Math.Round((loanAmount * interestRate * Math.Pow((1 + interestRate), totalNumberOfPayments)) /
                    (Math.Pow((1 + interestRate), totalNumberOfPayments) - 1), 2);
        }

        
    }
}
