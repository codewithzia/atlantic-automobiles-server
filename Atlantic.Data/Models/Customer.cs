using Atlantic.Data.Audit;

namespace Atlantic.Data.Models
{
    public class CustomerUpdateRequest
    {
        public bool? InternalCheck { get; set; }
        public bool? HighRiskNationality { get; set; }
        public bool? Pep { get; set; }
        public bool? OverseasPurchase { get; set; }
        public bool? OverseasAtmWithdrawal { get; set; }
        public bool? OnlineTransfer { get; set; }
        public decimal MonthlySpending { get; set; }
        public decimal AnnualSpending { get; set; }
    }
    public class Customer : AuditableEntitySerial
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string MothersMaidenName { get; set; }
        public string Gender { get; set; }
        public string IdType { get; set; }
        public string IdNumber { get; set; }
        public DateTime? IdExpiry { get; set; }
        public string? ResidentialAddress { get; set; }
        public string? MailingAddress { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; }
        public string Occupation { get; set; }
        public string NatureOfBusiness { get; set; }
        public string ContactNumber { get; set; }
        public string PurposeOfApplication { get; set; }
        public string Email { get; set; }
        public string Employer { get; set; }
        public bool? InternalCheck { get; set; }
        public bool? HighRiskNationality { get; set; }
        public bool? Pep { get; set; }
        public bool? OverseasPurchase { get; set; }
        public bool? OverseasAtmWithdrawal { get; set; }
        public bool? OnlineTransfer { get; set; }
        public string Selfie { get; set; }
        public string IdDocument { get; set; }
        public string UtilityBill { get; set; }
        public decimal MonthlySpending { get; set; }
        public decimal AnnualSpending { get; set; }
        public string Status { get; set; } = "PendingKYC";

        public string? VisaCardId { get; set; }
        public string? VisaCardLastDigits { get; set; }     
        public DateTime? VisaCardExpiry { get; set; }
        public string? VisaCardStatus { get; set; }
    }

    public class Address
    {
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
