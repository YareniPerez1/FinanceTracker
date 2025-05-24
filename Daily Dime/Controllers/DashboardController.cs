using Daily_Dime.Data;
using Daily_Dime.Models;
using FTDataAccess.Interface;
using FTDataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Globalization;

namespace Daily_Dime.Controllers
{
    public class DashboardController : Controller
    {




        private readonly ITransaction _transactionRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ITransaction transactionRepository, UserManager<IdentityUser> userManager)
        {
            _transactionRepository = transactionRepository;
            _userManager = userManager;
        }


        [Authorize]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var userId = _userManager.GetUserId(User);

            var start = startDate ?? DateTime.Today.AddDays(-29);
            var end = endDate ?? DateTime.Today;

            var allTransactions = await _transactionRepository.GetAllAsync(userId);

            var selectedTransactions = allTransactions
                .Where(t => t.Date >= start && t.Date <= end)
                .ToList();

            // Totals
            decimal totalIncome = selectedTransactions
                .Where(t => t.Category.Type == "Income")
                .Sum(t => t.Amount);

            decimal totalExpense = selectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .Sum(t => t.Amount);

            decimal balance = totalIncome - totalExpense;

            var disposableIncome = balance;
            var recommendedSavings = disposableIncome > 0 ? disposableIncome * 0.2m : 0;

            var culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;

            // Doughnut Chart
            var doughnutLabels = selectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.CategoryId)
                .Select(g => g.First().Category.Icon + " " + g.First().Category.Title)
                .ToArray();

            var doughnutData = selectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.CategoryId)
                .Select(g => g.Sum(t => t.Amount))
                .ToArray();

            // Line Chart
            var incomeSummary = selectedTransactions
                .Where(t => t.Category.Type == "Income")
                .GroupBy(t => t.Date)
                .Select(g => new SplineChartData
                {
                    Day = g.Key.ToString("dd-MMM"),
                    Income = g.Sum(t => t.Amount)
                }).ToList();

            var expenseSummary = selectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Date)
                .Select(g => new SplineChartData
                {
                    Day = g.Key.ToString("dd-MMM"),
                    Expense = g.Sum(t => t.Amount)
                }).ToList();

            var allDays = Enumerable.Range(0, (end - start).Days + 1)
                .Select(i => start.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            var splineChartData = allDays.Select(day => new
            {
                Day = day,
                Income = incomeSummary.FirstOrDefault(i => i.Day == day)?.Income ?? 0,
                Expense = expenseSummary.FirstOrDefault(e => e.Day == day)?.Expense ?? 0
            }).ToList();

            // Build ViewModel
            var model = new DashboardViewModel
            {
                StartDate = start,
                EndDate = end,
                TotalIncome = totalIncome.ToString("C2"),
                TotalExpense = totalExpense.ToString("C2"),
                RecommendedSavings = recommendedSavings.ToString("C2"),
                Balance = string.Format(culture, "{0:C2}", balance),

                DoughnutLabels = doughnutLabels,
                DoughnutData = doughnutData,

                SplineLabels = allDays,
                SplineIncomeData = splineChartData.Select(d => d.Income).ToArray(),
                SplineExpenseData = splineChartData.Select(d => d.Expense).ToArray(),

                RecentTransactions = selectedTransactions
                    .OrderByDescending(t => t.Date)
                    .Take(5)
                    .ToList()
            };

            return View(model);
        }

        public class SplineChartData
        {
            public string Day { get; set; }
            public decimal Income { get; set; }
            public decimal Expense { get; set; }
        }
    }
}
