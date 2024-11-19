using Microsoft.EntityFrameworkCore;
using PaymentService.Enumerations;

namespace PaymentService.Databases.Schemas
{
    public class ExtendStorage
    {
        public Guid Id { get; set; }

        [Comment("The amount of storage that is bought")]
        public long StorageAmount { get; set; }

        [Comment("The price of the storage at the time of purchase")]
        public decimal Price { get; set; }

        public CurrencyType Currency { get; set; }

        public DateTimeOffset BoughtAt { get; set; }

        public int StorageInfoId { get; set; }

        public virtual StorageInfo StorageInfo { get; set; }
    }
}