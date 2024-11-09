using Microsoft.EntityFrameworkCore;

namespace PaymentService.Enumerations
{
    public enum RequestStatus
    {
        [Comment("The request is pending")]
        Pending = 0,
        [Comment("The request is approved")]
        Approved = 1,
        [Comment("The request is rejected")]
        Rejected = 2
    }
}