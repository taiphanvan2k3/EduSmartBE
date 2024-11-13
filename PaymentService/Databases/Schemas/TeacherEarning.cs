namespace PaymentService.Databases.Schemas
{
    public class TeacherEarning
    {
        public int UserId { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal TotalEarnings { get; set; }
    }
}