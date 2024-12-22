using PaymentService.Commons.Schemas;

namespace PaymentService.Services.WithdrawalRequests.Schemas
{
    public class WithdrawalRequestSearchCondition : ParamsSearch
    {
        public DateTimeOffset? FromDate { get; set; }

        public DateTimeOffset? ToDate { get; set; }
    }

    public class WithdrawalRequestSearchConditionForAdmin : WithdrawalRequestSearchCondition
    {
        public string SearchInput { get; set; } = string.Empty;
    }
}