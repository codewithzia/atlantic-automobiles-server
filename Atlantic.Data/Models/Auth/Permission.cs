using Atlantic.Data.Audit;

namespace Atlantic.Data.Models.Auth
{
    public class Permission : AuditableEntitySerial
    {
        public string? UserId { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty; //systemadmin,systememployee,agentadmin,agentemployee,subagent
        public AccountsPermission Accounts { get; set; } = new AccountsPermission();
        public PermissionTable Permissions { get; set; } = new PermissionTable();
    }

    public class Crud
    {
        public bool Create { get; set; } = false;
        public bool Update { get; set; } = false;
        public bool View { get; set; } = false;
        public bool Remove { get; set; } = false;
    }
    public class AccountsPermission
    {
        public bool SellReports { get; set; } = false;
        public bool ViewAllAccounts { get; set; } = false;
    }
    public class PermissionTable : Crud
    {
    }
}
