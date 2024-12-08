using PaymentService.Enumerations;

namespace PaymentService.Databases.Schemas
{
    public class UploadHistory
    {
        public Guid Id { get; set; }

        public long StorageAmount { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public FileType FileType { get; set; }

        public MediaStorageProvider MediaStorageProvider { get; set; }

        public int StorageInfoId { get; set; }

        public virtual StorageInfo StorageInfo { get; set; }
    }
}