using MongoDB.Entities;
using System.ComponentModel;

namespace Atlantic.Data.Audit
{
    public class AuditableEntity : Entity
    {
        public string? CreatedById { get; set; }
        public string? CreatedByNme { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        public string? ModifiedById { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;

        public string? DeletedById { get; set; }
        public string? DeletedByName { get; set; }
        public DateTime? DeletedDate { get; set; }
        [DefaultValue(false)]
        public bool Delete { get; set; } = false;
    }

    public class AuditableEntitySerial : AuditableEntity
    {
        public ulong Serial { get; set; }
    }
}
