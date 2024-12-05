namespace UserService.Services.Users.Schemas
{
    public class DashboardDto
    {
        public List<MonthlyData> MonthlyDrawingRequests { get; set; }

        public List<MonthlyData> MonthlyNewUsers { get; set; }

        public List<MonthlyData> MonthlyPurchaseCourses { get; set; }

        public List<MonthlyData> MonthlyDiscussions { get; set; }

        public List<MonthlyData> MonthlyRevenues { get; set; }

        public List<MonthlyData> MonthlyProfits { get; set; }
    }
}