using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Atlantic.Data.Models.Auth
{
    [CollectionName("Auth_Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public DateTime? PasswordValidity { get; set; }
        public Boolean? Status { get; set; }
        public string? Password { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? UserProfileId { get; set; }
        public string? UserGroup { get; set; }
        public string? UserType { get; set; }
        public string? UserRole { get; set; }
        public bool? IsSystemAdmin { get; set; } = false;
        public string? CreatedById { get; set; }
        public string? Pin { get; set; } = string.Empty;


    }
}
