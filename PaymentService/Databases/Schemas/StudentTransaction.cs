using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PaymentService.Enumerations;

namespace PaymentService.Databases.Schemas
{
    public class StudentTransaction
    {
        public Guid Id { get; set; }

        [MaxLength(12)]
        public string Code { get; set; }

        public int StudentId { get; set; }

        public decimal Amount { get; set; }

        public Guid CourseId { get; set; }

        [MaxLength(1000)]
        public string Error { get; set; }

        /// <summary>
        /// The teacher who the student paid to
        /// </summary>
        [Comment("The teacher who the student paid to")]
        public int BelongToTeacherId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? ResolvedAt { get; set; }

        public OrderStatus OrderStatus { get; set; }
    }
}