using Atlantic.Data.Audit;

namespace Atlantic.Data.Models
{
    public class Product : AuditableEntitySerial
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortCode { get; set; }
        public string ProductType { get; set; }
        public double Cost { get; set; }
        public double Sell { get; set; }
    }
}
