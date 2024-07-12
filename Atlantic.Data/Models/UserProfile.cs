using Atlantic.Data.Audit;
using System.Security.Claims;

namespace Atlantic.Data.Models
{
    public class UserProfile : AuditableEntitySerial
    {
        public string Name { get; set; }
        public string? CompanyName { get; set; }
        public string Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? Photo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string? IdType { get; set; }
        public string? IdNumber { get; set; }
        public DateTime? IdExpiry { get; set; }
        public string? ResidentialAddress { get; set; }
        public string? MailingAddress { get; set; }
        public string? UserType { get; set; }
        public string? UserId { get; set; }

        public string? PassportImage { get; set; }
        public string? VisaImage { get; set; }

        /*
        public Address? ResidentialAddress { get; set; }
        public Address? MailingAddress { get; set; }
        */
        public BankAccount? BankAccount1 { get; set; }
        public BankAccount? BankAccount2 { get; set; }
        public string? ParentAgentId { get; set; }
    }

    public class BankAccount 
    {
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
    }
}
