using FTDataAccess.Models;

namespace Daily_Dime.Models
{
    public class DashboardViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<Transaction> RecentTransactions { get; set; } = new();

        public string TotalIncome { get; set; }
        public string TotalExpense { get; set; }
        public string RecommendedSavings { get; set; }
        public string Balance { get; set; }

        public string[] DoughnutLabels { get; set; }
        public decimal[] DoughnutData { get; set; }

        public string[] SplineLabels { get; set; }
        public decimal[] SplineIncomeData { get; set; }
        public decimal[] SplineExpenseData { get; set; }
    
}
}
