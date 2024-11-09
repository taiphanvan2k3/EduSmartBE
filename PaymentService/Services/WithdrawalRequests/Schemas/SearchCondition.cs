using PaymentService.Commons.Schemas;

namespace PaymentService.Services.WithdrawalRequests.Schemas
{
    public class SearchCondition : ParamsSearch
    {
        public string SearchInput { get; set; } = string.Empty;

        public bool? IsActive { get; set; }     
    }
}