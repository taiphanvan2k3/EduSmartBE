namespace PaymentService.Commons.Schemas
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }

        public Paging Paging { get; set; }
    }
}