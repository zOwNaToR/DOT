using Auth.Abstractions;
using Auth.DTOs.Requests;
using DataManager.Common.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;

        public UserController(IUnitOfWork unitOfWork, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var result = await _unitOfWork.UserRepository.GetAsync();

                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            try
            {
                var result = await _unitOfWork.UserRepository.GetByIdAsync(id);
                if (result is null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] RegisterUserRequest request)
        {
            try
            {
                var result = await _authenticationService.RegisterAsync(request);
                if (!result.Success)
                {
                    return BadRequest();
                }

                return Created($"api/{request.Id}", result);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
