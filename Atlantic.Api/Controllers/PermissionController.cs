
using Atlantic.Data.Models.Auth;
using Atlantic.Data.Repositories;
using Atlantic.Data.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Atlantic.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionService _permissionService;

        public PermissionController(IUnitOfWork unitOfWork, IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _permissionService = permissionService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Permission permission)
        {
            return Ok(await _unitOfWork.PermissionRepository.Insert(permission));
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] Permission permission)
        {
            return Ok(await _unitOfWork.PermissionRepository.Update(permission));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Permission>> Get(string id)
        {
            var BankAccount = _unitOfWork.PermissionRepository.GetById(id);
            return Ok(BankAccount);
        }

        [HttpGet("GetByUserId/{userId}")]
        public async Task<ActionResult> GetByUserId(string userId)
        {
            return Ok( _unitOfWork.PermissionRepository.Search().Where(x => x.UserId.Equals(userId)).FirstOrDefault());
        }

        [HttpGet("GetByRole/{role}")]
        public async Task<ActionResult> GetByRole(string role)
        {
            return Ok(_unitOfWork.PermissionRepository.Search().Where(x => x.Role.Equals(role)).FirstOrDefault());
        }

        [HttpGet("MyPermission")]
        public async Task<ActionResult> MyPermission()
        {
            var userId = _unitOfWork.GetUserId();
            var permission = _unitOfWork.PermissionRepository.Search().Where(x => x.UserId.Equals(userId)).FirstOrDefault();
            if (permission == null)
            {
                permission = await _permissionService.SetMyPermissions();
            }
            return Ok(permission);
        }
    }
}
