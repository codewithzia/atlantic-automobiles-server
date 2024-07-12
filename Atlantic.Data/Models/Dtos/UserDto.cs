namespace Atlantic.Data.Models.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? ProfileId { get; set; }
        public string? UserGroup { get; set; }
        public string? UserType { get; set; }
        public string? UserRole { get; set; }
    }
}
