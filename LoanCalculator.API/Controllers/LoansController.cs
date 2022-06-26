using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
        public LoanPayment Get([FromQuery] double loanAmount, int loanTerm)
        {
            int totalNumberOfPayments = loanTerm * 12;
            float interestRate = JsonSerializer.Deserialize<float>(Configuration["MontlyInterest"]);
            double montlyPayments = calculateMontlyPayment(loanAmount, interestRate, totalNumberOfPayments);
            double totalPayment = Math.Round((totalNumberOfPayments * montlyPayments), 2);
            double interestPayment = Math.Round((totalPayment - loanAmount), 2);
            return new LoanPayment{TotalPayment = totalPayment, InterestPayment = interestPayment};
        }

        private double calculateMontlyPayment(double loanAmount, float interestRate, int totalNumberOfPayments)
        {
            return (loanAmount * interestRate * Math.Pow((1 + interestRate), totalNumberOfPayments)) /
                    (Math.Pow((1 + interestRate), totalNumberOfPayments) - 1);
        }

        
    }
}
