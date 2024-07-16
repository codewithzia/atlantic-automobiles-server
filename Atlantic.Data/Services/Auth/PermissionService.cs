using Atlantic.Data.Models.Auth;
using Atlantic.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Reflection;

namespace Atlantic.Data.Services.Auth
{
    public interface IPermissionService
    {
        Task<Permission> SetAdminPermissions(string userId);
        Task<Permission> SetMyPermissions();

    }

    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<ApplicationUser> _userManager;

        private readonly string systemadmin = "systemadmin";
        private readonly string systememployee = "systememployee";
        private readonly string agentadmin = "agentadmin";
        private readonly string agentemployee = "agentemployee";
        private readonly string subagent = "subagent";

        public PermissionService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public Task<Permission> SetAdminPermissions(string userId)
        {

            Permission permissionNew = new Permission
            {
                UserId = userId,
                Role = "systemadmin"
            };
            SetAllPermissionsToTrue(permissionNew);

            return Task.FromResult(permissionNew);
        }

        public async Task<Permission> SetMyPermissions()
        {
            var userId = _unitOfWork.GetUserId();
            var permission = new Permission();
            var permissionQuery = _unitOfWork.PermissionRepository;
            //systemadmin,systememployee,agentadmin,agentemployee,subagent
            if (_unitOfWork.HttpAccessor().IsAgentAdmin())
            {
                var permissionByrole = permissionQuery.Search().Where(x => x.Role.Equals(agentadmin)).FirstOrDefault();
                permissionByrole.UserId = userId;
                return await permissionQuery.Insert(permissionByrole);

            }
            /*            if (_unitOfWork.HttpAccessor().IsSubAgent())
           {
               var permissionByrole = permissionQuery.Search().Where(x => x.Role.Equals(subagent)).FirstOrDefault();
               permissionByrole.UserId = userId;
               return await permissionQuery.Insert(permissionByrole);
           }*/
            else if (_unitOfWork.HttpAccessor().IsAgentEmployee())
            {
                var permissionByrole = permissionQuery.Search().Where(x => x.Role.Equals(agentemployee)).FirstOrDefault();
                permissionByrole.UserId = userId;
                return await permissionQuery.Insert(permissionByrole);

            }
            /*            else if (_unitOfWork.HttpAccessor().IsSystemEmployee())
                        {
                            var getAgent = await _unitOfWork.AgentRepository.GetById(_unitOfWork.GetAgentId());
                            var permissionByrole = permissionQuery.Search().Where(x => x.Role.Equals(getAgent.Role)).FirstOrDefault();
                            permissionByrole.UserId = userId;
                            return await permissionQuery.Insert(permissionByrole);
                        }*/
            else if (_unitOfWork.HttpAccessor().IsSystemAdmin())
            {
                var permissionByrole = permissionQuery.Search().Where(x => x.Role.Equals(systemadmin)).FirstOrDefault();
                if (permissionByrole != null)
                {
                    permissionByrole.UserId = userId;
                    permission = await permissionQuery.Insert(permissionByrole);
                }
                else
                {
                    var defaultPermission = await SetAdminPermissions(userId);
                    permission = await permissionQuery.Insert(defaultPermission);
                    // add a system admin default permission
                    defaultPermission.Role = systemadmin;
                    defaultPermission.UserId = "";
                    await permissionQuery.Insert(defaultPermission);
                }

            }
            else
            {
                Permission permissionNew = new Permission
                {
                    UserId = userId,
                };
                permission = await permissionQuery.Insert(permissionNew);
            }
            return permission;
        }

        private void SetAllPermissionsToTrue(object obj)
        {
            if (obj == null) return;

            // Get all properties of the current object
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(bool) && property.Name != "Delete")
                {
                    // Set boolean property to true
                    property.SetValue(obj, true);
                }
                else if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string))
                {
                    // Recursively set permissions on nested objects
                    var nestedObject = property.GetValue(obj);
                    if (nestedObject == null)
                    {
                        // Initialize the nested object if it is null
                        nestedObject = Activator.CreateInstance(property.PropertyType);
                        property.SetValue(obj, nestedObject);
                    }
                    SetAllPermissionsToTrue(nestedObject);
                }
            }
        }
    }
}
