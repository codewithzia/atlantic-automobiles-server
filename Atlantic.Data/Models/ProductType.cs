using Atlantic.Data.Audit;

namespace Atlantic.Data.Models
{
    public class ProductType : AuditableEntitySerial
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }


}
