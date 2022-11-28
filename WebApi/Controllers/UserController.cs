using Auth.DTOs;
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

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                var result = await _unitOfWork.UserRepository.InsertAsync(request.MapToPoco());
                if (result is null)
                {
                    return BadRequest();
                }

                return Created($"api/{result.Id}", result);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
