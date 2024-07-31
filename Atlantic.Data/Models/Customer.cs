using Atlantic.Data.Audit;

namespace Atlantic.Data.Models
{
    public class Customer : AuditableEntitySerial
    {
        public string Name { get; set; }
        public string TaxId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<Vehicle>? Vehicles { get; set; } = new List<Vehicle>();
    }

    public class Vehicle
    {
        public string VehicleTypeId { get; set; }
        public string Number { get; set; }
        public string Color { get; set; }
    }
}
