using Microsoft.AspNetCore.Mvc;
using FitnessTracker.Application.DTO;
using FitnessTracker.Core.Abstractions;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
namespace FitnessTracker.RequestService.Controllers
{
    [ApiController]
    [Route("api")]
    public class RequestController : ControllerBase
    {
        private readonly IClient _client;
        private readonly IValidator<IAddWorkoutData> _addValidator;
        private readonly IValidator<IGetWorkoutData> _getValidator;
        private readonly IValidator<IDeleteWorkoutData> _deleteValidator;
        private readonly IValidator<IEditWorkoutData> _editValidator;
        private readonly IValidator<IGetStats> _statsValidator;

        public RequestController(IClient client, IValidator<IAddWorkoutData> addValidator, IValidator<IGetWorkoutData> getValidator
            , IValidator<IDeleteWorkoutData> deleteValidator, IValidator<IEditWorkoutData> editValidator, IValidator<IGetStats> statsValidator)
        {
            _client = client;
            _addValidator = addValidator;
            _getValidator = getValidator;
            _deleteValidator = deleteValidator;
            _editValidator = editValidator;
            _statsValidator = statsValidator;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddWorkoutAsync([FromBody] AddWorkoutDto newWorkout)
        {
            try
            {
                CancellationTokenSource ct_src = new CancellationTokenSource();
                ct_src.CancelAfter(TimeSpan.FromSeconds(10));
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                newWorkout.UserName = jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value;
                var res = _addValidator.Validate(newWorkout);
                if (res.IsValid)
                {
                    string msg = JsonSerializer.Serialize(newWorkout);
                    var response = await _client.CallAsync("workouts-add", msg, ct_src.Token);
                    return Content(response);
                }
                foreach (var error in res.Errors)
                    ModelState.TryAddModelError(error.ErrorCode, error.ErrorMessage);
                return BadRequest(ModelState);
            }
            catch (TimeoutException)
            {
                return StatusCode(504, "Request timed out while waiting for response.");
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateWorkoutAsync([FromBody] EditWorkoutDto updatedWorkout)
        {
            try
            {
                CancellationTokenSource ct_src = new CancellationTokenSource();
                ct_src.CancelAfter(TimeSpan.FromSeconds(10));
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                updatedWorkout.UserName = jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value;
                var res = _editValidator.Validate(updatedWorkout);
                if (res.IsValid)
                {
                    string msg = JsonSerializer.Serialize(updatedWorkout);
                    var response = await _client.CallAsync("workouts-update", msg, ct_src.Token);
                    return Content(response);
                }
                foreach (var error in res.Errors)
                    ModelState.TryAddModelError(error.ErrorCode, error.ErrorMessage);
                return BadRequest(ModelState);
            }
            catch (TimeoutException)
            {
                return StatusCode(504, "Request timed out while waiting for response.");
            }
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteWorkoutAsync([FromBody] DeleteWorkoutDto targetWorkout)
        {
            CancellationTokenSource ct_src = new CancellationTokenSource();
            ct_src.CancelAfter(TimeSpan.FromSeconds(10));
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            targetWorkout.UserName = jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            var res = _deleteValidator.Validate(targetWorkout);
            if (res.IsValid)
            {
                string msg = JsonSerializer.Serialize(targetWorkout);
                var response = await _client.CallAsync("workouts-delete", msg, ct_src.Token);
                return Content(response);
            }
            foreach (var error in res.Errors)
                ModelState.TryAddModelError(error.ErrorCode, error.ErrorMessage);
            return BadRequest(ModelState);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetWorkoutAsync([FromBody] GetWorkoutDto targetWorkout)
        {
            CancellationTokenSource ct_src = new CancellationTokenSource();
            ct_src.CancelAfter(TimeSpan.FromSeconds(10));
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            targetWorkout.UserName = jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            var res = _getValidator.Validate(targetWorkout);
            if (res.IsValid)
            {
                string msg = JsonSerializer.Serialize(targetWorkout);
                var response = await _client.CallAsync("workouts-get", msg, ct_src.Token);
                return Content(response);
            }
            foreach (var error in res.Errors)
                ModelState.TryAddModelError(error.ErrorCode, error.ErrorMessage);
            return BadRequest(ModelState);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetUserStatsAsync([FromBody] GetStatsDto statsDto)
        {
            CancellationTokenSource ct_src = new CancellationTokenSource();
            ct_src.CancelAfter(TimeSpan.FromSeconds(10));
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            statsDto.UserName = jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            var res = _statsValidator.Validate(statsDto);
            if (res.IsValid)
            {
                string msg = JsonSerializer.Serialize(statsDto);
                var response = await _client.CallAsync("workouts-stat", msg, ct_src.Token);
                return Content(response);
            }
            foreach (var error in res.Errors)
                ModelState.TryAddModelError(error.ErrorCode, error.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}
