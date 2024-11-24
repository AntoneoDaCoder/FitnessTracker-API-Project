using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitnessTracker.Core.Abstractions;
using System.IdentityModel.Tokens.Jwt;
namespace FitnessTracker.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class GatewayController : ControllerBase
    {
        private readonly IRequestRouter _requestRouter;
        public GatewayController(IRequestRouter router) => _requestRouter = router;

        [Route("auth/{any:alpha}")]
        public async Task<IActionResult> HandleAuthRequest(string any)
        {
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                var bodyContent = await reader.ReadToEndAsync();
                var response = await _requestRouter.SendRequestAsync
                ("auth-service", "api/" + any, bodyContent, HttpContext.Request.Method);
                var answer = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return Content(answer);
                return BadRequest(answer);
            }

        }
        [Route("workouts/{any:alpha}")]
        [Authorize]
        public async Task<IActionResult> HandleWorkoutsRequest(string any)
        {
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                var bodyContent = await reader.ReadToEndAsync();
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var response = await _requestRouter.SendRequestAsync
                ("request-service", "api/" + any, bodyContent, HttpContext.Request.Method, token: jwtToken);
                var answer = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return Content(answer);
                return BadRequest(answer);
            }
        }
    }
}
