using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PaymentService.Enumerations
{
    [JsonConverter(typeof(JsonStringEnumConverter))] // Dùng StringEnumConverter để tránh lỗi gửi request dạng string
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