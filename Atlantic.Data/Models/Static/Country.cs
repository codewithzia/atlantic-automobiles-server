using Atlantic.Data.Audit;
using Atlantic.Data.Models.Constants;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlantic.Data.Models.Static
{
    public class Country : AuditableEntity
    {
        public string Name { get; set; }
        [Column(TypeName = CommonConstants.StringLengthTwo)]
        public string Code { get; set; }
        public string DialingCode { get; set; }
        [Column(TypeName = CommonConstants.StringLengthThree)]
        public string Currency { get; set; }
        [Column(TypeName = CommonConstants.StringLengthThree)]
        public string CurrencyPrint { get; set; }
        [Column(TypeName = CommonConstants.CurrencyDecimalType)]
        public double Rate { get; set; }
        public bool Active { get; set; } = true;
    }
}
