using Atlantic.Data.Audit;
using MongoDB.Bson.Serialization.Attributes;

namespace Atlantic.Data.Models
{
    public class VisaCard : AuditableEntitySerial
    {
        public string LastDigits { get; set; }
        public string SerialNumber { get; set; }
        public string Barcode { get; set; }
        public string IssuedTo { get; set; } // CustomerID
        public string IssuedToName { get; set; } // customerName
        public string IssuedByAgent { get; set; } // AgentID
        public string IssuedByAgentName { get; set; } // agentName
        public DateTime? Expiry { get; set; }
        public string Status { get; set; } // 'INACTIVE', 'ACTIVE', 'BLOCKED', 'LOST' ,"Pending Verification"

        public string? AgentId { get; set; }
        public string? AgentName { get; set; }

        public string? InvoiceId { get; set; }
        public string? DeliveryId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? CustomerDeliveryDate { get; set; }

        [BsonIgnore]
        public Customer? Customer { get; set; }
    }


}
