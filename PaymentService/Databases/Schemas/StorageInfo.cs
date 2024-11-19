namespace PaymentService.Databases.Schemas
{
    public class StorageInfo
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public long MaximumStorage { get; set; }

        public long UsedStorage { get; set; }

        public virtual ICollection<ExtendStorage> ExtendStorages { get; set; } = [];
    }
}