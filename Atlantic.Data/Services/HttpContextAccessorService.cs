using Atlantic.Data.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Atlantic.Data.Services
{
    public interface IHttpContextAccessorService
    {
        public string CurrentAgentId();
        public string CurrentUserId();
        public string CurrentUsername();
        public string CurrentUserRole();
        public string Currency();
        public string Role();

        public bool IsSystemAdmin();
        public bool IsAgentAdmin();
        public bool IsAgentEmployee();
        public string CurrentUserEmail { get; set; }
        public List<string> CurrentUserRoles();
        public Task<ApplicationUser> CurrentUser();

    }
    public class HttpContextAccessorService : IHttpContextAccessorService
    {
        private readonly IHttpContextAccessor _httpAccessor;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        public string CurrentUserEmail { get; set; }
        private readonly string _userType;

        private readonly string _role;
        private readonly string LoggedInUserId;
        private readonly string LoggedInDesignationCode;
        private readonly string LoggedInUserRole;
        private readonly List<string> LoggedInUserRoles;
        private readonly string _isSystemAdmin;
        private readonly string _internal;
        private readonly string _userGroup;
        private readonly string AgentId;

        private readonly string ParentAgentId;
        public HttpContextAccessorService(IHttpContextAccessor httpAccessor, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _httpAccessor = httpAccessor;
            if (_httpAccessor.HttpContext?.User?.Claims != null && _httpAccessor.HttpContext.User.Claims.Any())
            {

                LoggedInUserId = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.ToLower().Equals("userid")).FirstOrDefault().Value;
                //LoggedInUserRole = httpAccessor.HttpContext?.User.Claims.Where(c => c.Type.Equals("RoleId")).FirstOrDefault().Value;
                LoggedInUsername = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.ToLower().Equals("username")).FirstOrDefault().Value;
                CurrentUserEmail = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.Contains("emailaddress")).FirstOrDefault().Value;
                LoggedInUserRoles = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")).Select(x => x.Value).ToList();
                LoggedInUserRole = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role"))?.FirstOrDefault()?.Value;
                _currency = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.ToLower().Equals("currency")).FirstOrDefault()?.Value ?? "";
                _isSystemAdmin = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.ToLower().Equals("issystemadmin"))?.FirstOrDefault()?.Value ?? "";
                _role = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.ToLower().Equals("role"))?.FirstOrDefault()?.Value ?? "";
                _internal = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.ToLower().Equals("internal"))?.FirstOrDefault()?.Value ?? "";
                _userType = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.ToLower().Equals("usertype"))?.FirstOrDefault()?.Value ?? "";
                _userGroup = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.ToLower().Equals("usergroup"))?.FirstOrDefault()?.Value ?? "";
                AgentId = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.ToLower().Equals("agentid"))?.FirstOrDefault()?.Value ?? "";
                ParentAgentId = httpAccessor.HttpContext?.User?.Claims.Where(c => c.Type.ToLower().Equals("parentagentid"))?.FirstOrDefault()?.Value ?? "";

            }
            else
            {
                Console.WriteLine("No claims found in the current context.");
            }
        }
        public string CurrentUserId()
        {
            return LoggedInUserId;
        }
        private readonly string LoggedInUsername;
        public string CurrentUsername()
        {
            return LoggedInUsername;
        }
        public string CurrentDesigUser()
        {
            return LoggedInDesignationCode;
        }
        public string CurrentUserRole()
        {
            return LoggedInUserRole;
        }

        public string CurrentAgentId()
        {
            return AgentId;
        }
        public string CurrentParentAgentId()
        {
            return ParentAgentId;
        }

        public List<string> CurrentUserRoles()
        {
            return LoggedInUserRoles;
        }
        private readonly string _currency;
        public string Currency()
        {
            return _currency;
        }
        public bool IsSystemAdmin()
        {
            return _isSystemAdmin.ToLower().Equals("true");
        }

        public bool IsAgentAdmin()
        {
            return (_internal.ToLower().Equals("false") && _role.ToLower().Equals("agent") && _userGroup.ToLower().Equals("agent") && _userType.ToLower().Equals("agent")) ? true : false;
        }
        public bool IsAgentEmployee()
        {
            return _internal.ToLower().Equals("false") && _userGroup.ToLower().Equals("agent") && _userType.ToLower().Equals("employee") ? true : false;
        }
        public async Task<ApplicationUser> CurrentUser()
        {
            return await _userManager.FindByIdAsync(LoggedInUserId);
        }


        public string Role()
        {
            return _role;
        }
    }
}
