using Atlantic.Data.Audit;

namespace Atlantic.Data.Models
{
    public class Service : AuditableEntitySerial
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortCode { get; set; }
        public double Cost { get; set; }
        public double Sell { get; set; }

    }
}
