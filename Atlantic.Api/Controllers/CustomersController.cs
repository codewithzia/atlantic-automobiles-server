using Atlantic.Data.Models;
using Atlantic.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlantic.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var Customers = await _unitOfWork.CustomerRepository.GetAll();
            return Ok(Customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var Customer = await _unitOfWork.CustomerRepository.GetById(id);
            return Ok(Customer);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            await _unitOfWork.CustomerRepository.Insert(customer);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Customer customer)
        {
            await _unitOfWork.CustomerRepository.Update(customer);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _unitOfWork.CustomerRepository.Delete(id);
            return Ok();
        }
    }
}
