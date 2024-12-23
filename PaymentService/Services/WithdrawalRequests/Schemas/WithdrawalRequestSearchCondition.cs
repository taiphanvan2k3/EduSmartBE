using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Binders;
using PaymentService.Commons.Schemas;
using PaymentService.Enumerations;

namespace PaymentService.Services.WithdrawalRequests.Schemas
{
    public class WithdrawalRequestSearchCondition : ParamsSearch
    {
        public DateTimeOffset? FromDate { get; set; }

        public DateTimeOffset? ToDate { get; set; }

        [EnumDataType(typeof(RequestStatus))] // Cho phép truyền Enum dưới dạng string trong query string 
        [ModelBinder(BinderType = typeof(EnumBinder<RequestStatus>))]
        [FromQuery(Name = "requestStatus")]
        public RequestStatus? RequestStatus { get; set; }
    }

    public class WithdrawalRequestSearchConditionForAdmin : WithdrawalRequestSearchCondition
    {
        public string SearchInput { get; set; } = string.Empty;
    }
}