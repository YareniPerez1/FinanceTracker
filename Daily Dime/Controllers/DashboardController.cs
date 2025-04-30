using Daily_Dime.Data;
using FTDataAccess.Interface;
using FTDataAccess.Models;
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

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            
            var userId = _userManager.GetUserId(User);

            //startDate ??= DateTime.Today.AddDays(-29);
            //endDate ??= DateTime.Today;

            var start = startDate ?? DateTime.Today.AddDays(-29);
            var end = endDate ?? DateTime.Today;

            ViewBag.StartDate = start;
            ViewBag.EndDate = end;

            var allTransactions = await _transactionRepository.GetAllAsync(userId);

            //var selectedTransactions = allTransactions
            //    .Where(t => t.Date >= startDate && t.Date <= endDate)
            //    .ToList();

            var selectedTransactions = (await _transactionRepository.GetAllAsync(userId))
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

            ViewBag.TotalIncome = totalIncome.ToString("C2");
            ViewBag.TotalExpense = totalExpense.ToString("C2");

            var culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = string.Format(culture, "{0:C2}", balance);

            // Doughnut Chart Data (Expense by Category)
            ViewBag.DoughnutLabels = selectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.CategoryId)
                .Select(g => g.First().Category.Icon + " " + g.First().Category.Title)
                .ToArray();

            ViewBag.DoughnutData = selectedTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.CategoryId)
                .Select(g => g.Sum(t => t.Amount))
                .ToArray();

            // Line Chart Data (Income vs Expense)
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

            // Use full selected range for x-axis labels
            var allDays = Enumerable.Range(0, (end - start).Days + 1)
                .Select(i => start.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            var splineChartData = allDays.Select(day => new
            {
                Day = day,
                Income = incomeSummary.FirstOrDefault(i => i.Day == day)?.Income ?? 0,
                Expense = expenseSummary.FirstOrDefault(e => e.Day == day)?.Expense ?? 0
            }).ToList();

            ViewBag.SplineLabels = allDays;
            ViewBag.SplineIncomeData = splineChartData.Select(d => d.Income).ToArray();
            ViewBag.SplineExpenseData = splineChartData.Select(d => d.Expense).ToArray();

            // Recent Transactions
            ViewBag.RecentTransactions = selectedTransactions
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            return View();







            //   var userId = _userManager.GetUserId(User);
            //   //DateTime start = DateTime.Today.AddDays(-29);
            //   //DateTime end = DateTime.Today;

            //   //// Ensure start date is not later than end date
            //   //if (start > end)
            //   //{
            //   //    (start, end) = (end, start); // Swap values if needed
            //   //}



            //   // If dates aren't provided, default to the last 30 days
            //   DateTime start = startDate ?? DateTime.Today.AddDays(-29);
            //   DateTime end = endDate ?? DateTime.Today;

            //   ViewBag.StartDate = start;
            //   ViewBag.EndDate = end;

            //   var selectedTransactions = (await _transactionRepository.GetAllAsync(userId))
            //       .Where(t => t.Date.Date >= start.Date && t.Date.Date <= end.Date)
            //       .ToList();

            //   Console.WriteLine("Filtered Transactions Count: " + selectedTransactions.Count);
            //   foreach (var tx in selectedTransactions)
            //   {
            //       Console.WriteLine("Transaction Date: " + tx.Date.ToString("yyyy-MM-dd"));
            //   }
            //   // Log to console for debugging purposes
            //   Console.WriteLine("Start Date: " + startDate?.ToString("yyyy-MM-dd"));
            //   Console.WriteLine("End Date: " + endDate?.ToString("yyyy-MM-dd"));

            //   ViewBag.StartDate = start;
            //   ViewBag.EndDate = end;


            //   //     var selectedTransactions = (await _transactionRepository.GetAllAsync(userId))
            //   //.Where(t => t.Date.Date >= start && t.Date.Date <= end)
            //   //.ToList();
            ////   var selectedTransactions = (await _transactionRepository.GetAllAsync(userId))
            ////.Where(t => t.Date.Date >= start.Date && t.Date.Date <= end.Date) // Compare only the date part
            ////.ToList();

            ////   Console.WriteLine("Transactions within the date range:");
            ////   foreach (var tx in selectedTransactions)
            ////   {
            ////       Console.WriteLine($"Transaction Date: {tx.Date.ToString("yyyy-MM-dd")}");
            ////   }

            //   // Calculate totals (income, expense, balance)
            //   decimal totalIncome = selectedTransactions
            //       .Where(t => t.Category.Type == "Income")
            //       .Sum(t => t.Amount);
            //   decimal totalExpense = selectedTransactions
            //       .Where(t => t.Category.Type == "Expense")
            //       .Sum(t => t.Amount);
            //   decimal balance = totalIncome - totalExpense;

            //   ViewBag.TotalIncome = totalIncome.ToString("C2");
            //   ViewBag.TotalExpense = totalExpense.ToString("C2");

            //   var culture = CultureInfo.CreateSpecificCulture("en-US");
            //   culture.NumberFormat.CurrencyNegativePattern = 1;
            //   ViewBag.Balance = string.Format(culture, "{0:C2}", balance);

            //   // Doughnut Chart Data (Expense by Category)
            //   ViewBag.DoughnutLabels = selectedTransactions
            //       .Where(t => t.Category.Type == "Expense")
            //       .GroupBy(t => t.Category.CategoryId)
            //       .Select(g => g.First().Category.Icon + " " + g.First().Category.Title)
            //       .ToArray();

            //   ViewBag.DoughnutData = selectedTransactions
            //       .Where(t => t.Category.Type == "Expense")
            //       .GroupBy(t => t.Category.CategoryId)
            //       .Select(g => g.Sum(t => t.Amount))
            //       .ToArray();

            //   // Line Chart Data (Income vs Expense)
            //   var incomeSummary = selectedTransactions
            //       .Where(t => t.Category.Type == "Income")
            //       .GroupBy(t => t.Date)
            //       .Select(g => new SplineChartData
            //       {
            //           Day = g.Key.ToString("dd-MMM"),
            //           Income = g.Sum(t => t.Amount)
            //       }).ToList();

            //   var expenseSummary = selectedTransactions
            //       .Where(t => t.Category.Type == "Expense")
            //       .GroupBy(t => t.Date)
            //       .Select(g => new SplineChartData
            //       {
            //           Day = g.Key.ToString("dd-MMM"),
            //           Expense = g.Sum(t => t.Amount)
            //       }).ToList();
            //   var last30Days = Enumerable.Range(0, 30)
            //       .Select(i => startDate.AddDays(i).ToString("dd-MMM"))
            //       .ToArray();

            //   var splineChartData = last30Days.Select(day => new
            //   {
            //       Day = day,
            //       Income = incomeSummary.FirstOrDefault(i => i.Day == day)?.Income ?? 0,
            //       Expense = expenseSummary.FirstOrDefault(e => e.Day == day)?.Expense ?? 0
            //   }).ToList();

            //   ViewBag.SplineLabels = last30Days;
            //   ViewBag.SplineIncomeData = splineChartData.Select(d => d.Income).ToArray();
            //   ViewBag.SplineExpenseData = splineChartData.Select(d => d.Expense).ToArray();

            //   ViewBag.RecentTransactions = selectedTransactions
            //       .OrderByDescending(t => t.Date)
            //       .Take(5)
            //       .ToList();
            //   //var splineChartData = incomeSummary.Select(day => new
            //   //{
            //   //    Day = day.Day,
            //   //    Income = day.Income,
            //   //    Expense = expenseSummary.FirstOrDefault(e => e.Day == day.Day)?.Expense ?? 0
            //   //}).ToList();

            //   //ViewBag.SplineLabels = splineChartData.Select(d => d.Day).ToArray();
            //   //ViewBag.SplineIncomeData = splineChartData.Select(d => d.Income).ToArray();
            //   //ViewBag.SplineExpenseData = splineChartData.Select(d => d.Expense).ToArray();

            //   //ViewBag.RecentTransactions = selectedTransactions
            //   //    .OrderByDescending(t => t.Date)
            //   //    .Take(5)
            //   //    .ToList();

            //   return View();

            //og
            //var selectedTransactions = (await _transactionRepository.GetAllAsync(userId))
            //    .Where(t => t.Date >= startDate && t.Date <= endDate)
            //    .ToList();

            // Totals
            //decimal totalIncome = selectedTransactions
            //    .Where(t => t.Category.Type == "Income")
            //    .Sum(t => t.Amount);
            //decimal totalExpense = selectedTransactions
            //    .Where(t => t.Category.Type == "Expense")
            //    .Sum(t => t.Amount);
            //decimal balance = totalIncome - totalExpense;

            //ViewBag.TotalIncome = totalIncome.ToString("C2");
            //ViewBag.TotalExpense = totalExpense.ToString("C2");

            //var culture = CultureInfo.CreateSpecificCulture("en-US");
            //culture.NumberFormat.CurrencyNegativePattern = 1;
            //ViewBag.Balance = string.Format(culture, "{0:C2}", balance);

            //// Doughnut Chart Data (Expense by Category)
            //ViewBag.DoughnutLabels = selectedTransactions
            //    .Where(t => t.Category.Type == "Expense")
            //    .GroupBy(t => t.Category.CategoryId)
            //    .Select(g => g.First().Category.Icon + " " + g.First().Category.Title)
            //    .ToArray();

            //ViewBag.DoughnutData = selectedTransactions
            //    .Where(t => t.Category.Type == "Expense")
            //    .GroupBy(t => t.Category.CategoryId)
            //    .Select(g => g.Sum(t => t.Amount))
            //    .ToArray();

            //// Line Chart Data (Income vs Expense)
            //var incomeSummary = selectedTransactions
            //    .Where(t => t.Category.Type == "Income")
            //    .GroupBy(t => t.Date)
            //    .Select(g => new SplineChartData
            //    {
            //        Day = g.Key.ToString("dd-MMM"),
            //        Income = g.Sum(t => t.Amount)
            //    }).ToList();

            //var expenseSummary = selectedTransactions
            //    .Where(t => t.Category.Type == "Expense")
            //    .GroupBy(t => t.Date)
            //    .Select(g => new SplineChartData
            //    {
            //        Day = g.Key.ToString("dd-MMM"),
            //        Expense = g.Sum(t => t.Amount)
            //    }).ToList();

            //var last7Days = Enumerable.Range(0, 7)
            //    .Select(i => startDate.AddDays(i).ToString("dd-MMM"))
            //    .ToArray();

            //var splineChartData = last7Days.Select(day => new
            //{
            //    Day = day,
            //    Income = incomeSummary.FirstOrDefault(i => i.Day == day)?.Income ?? 0,
            //    Expense = expenseSummary.FirstOrDefault(e => e.Day == day)?.Expense ?? 0
            //}).ToList();

            //ViewBag.SplineLabels = last7Days;
            //ViewBag.SplineIncomeData = splineChartData.Select(d => d.Income).ToArray();
            //ViewBag.SplineExpenseData = splineChartData.Select(d => d.Expense).ToArray();

            //ViewBag.RecentTransactions = selectedTransactions
            //    .OrderByDescending(t => t.Date)
            //    .Take(5)
            //    .ToList();

            //return View();
        }

        public class SplineChartData
        {
            public string Day { get; set; }
            public decimal Income { get; set; }
            public decimal Expense { get; set; }
        }
    }
    }
