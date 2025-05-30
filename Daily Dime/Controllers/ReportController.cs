using Daily_Dime.Models;
using FTDataAccess.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace Daily_Dime.Controllers
{
    public class ReportController : Controller
    {
        private readonly ITransaction _transactionRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ReportController(ITransaction transactionRepository, UserManager<IdentityUser> userManager)
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

            // Swap if start is after end
            if (start > end)
            {
                var temp = start;
                start = end;
                end = temp;

                ViewBag.DateRangeNotice = "You entered an invalid date range (start was after end). We've corrected it to show results from " +
                           start.ToString("MMM dd, yyyy") + " to " + end.ToString("MMM dd, yyyy") + ".";
            }

            var transactions = (await _transactionRepository.GetAllAsync(userId))
                .Where(t => t.Date >= start && t.Date <= end)
                .ToList();

            var recentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToList();

            var barLabels = transactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.Title)
                .Select(g => g.Key)
                .ToArray();

            var barData = transactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.Title)
                .Select(g => g.Sum(t => t.Amount))
                .ToArray();
            var totalIncome = transactions
.Where(t => t.Category.Type == "Income")
.Sum(t => t.Amount);

            var totalExpense = transactions
                .Where(t => t.Category.Type == "Expense")
                .Sum(t => t.Amount);

            var model = new DashboardViewModel
            {
                StartDate = start,
                EndDate = end,
                RecentTransactions = recentTransactions,
                BarLabels = barLabels,
                BarData = barData,
                TotalIncome = totalIncome.ToString("C2"),
                TotalExpense = totalExpense.ToString("C2")
            };

            return View(model);
        }

     
        [Authorize]
        public async Task<IActionResult> DownloadPdf(DateTime? startDate, DateTime? endDate)
        {
            var userId = _userManager.GetUserId(User);
            var start = startDate ?? DateTime.Today.AddDays(-29);
            var end = endDate ?? DateTime.Today;

            var transactions = (await _transactionRepository.GetAllAsync(userId))
                .Where(t => t.Date >= start && t.Date <= end)
                .ToList();

            var recentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToList();

            var barLabels = transactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.Title)
                .Select(g => g.Key)
                .ToArray();

            var barData = transactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.Title)
                .Select(g => g.Sum(t => t.Amount))
                .ToArray();
            var totalIncome = transactions
    .Where(t => t.Category.Type == "Income")
    .Sum(t => t.Amount);

            var totalExpense = transactions
                .Where(t => t.Category.Type == "Expense")
                .Sum(t => t.Amount);

            var model = new DashboardViewModel
            {
                StartDate = start,
                EndDate = end,
                RecentTransactions = recentTransactions,
                BarLabels = barLabels,
                BarData = barData,
                TotalIncome = totalIncome.ToString("C2"),
                TotalExpense = totalExpense.ToString("C2")
            };

            return new ViewAsPdf("PDFReport", model)
            {
                FileName = "ExpenseReport.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

    }
}
