using Azure.Core;
using DTOs.Request;
using DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using System.Net;
using WebAPI.CustomResponse;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/user/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IVNPayService _vnpayService;

        public PaymentController(IVNPayService vnpayService)
        {
            _vnpayService = vnpayService;
        }

        [HttpPost("buy-package")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> BuyPackage([FromBody] BuyPackageRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(ApiErrorResponses.ValidationError(firstError));
            }

            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1" || ipAddress.StartsWith("0.0.0"))
                {
                    ipAddress = "127.0.0.1";
                }

                ipAddress = IPAddress.Parse(ipAddress).MapToIPv4().ToString();

                string url = await _vnpayService.BuyPackageAsync(
                    request.UserId,
                    request.Amount,
                    ipAddress
                );

                return Ok(ApiResponse<string>.Success(url));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiErrorResponses.ValidationError(ex.Message));
            }
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnpayReturn()
        {
            var result = await _vnpayService.ProcessReturnAsync(Request.Query);

            var path = Request.Path.ToString().Replace("/api/v1/user", "");
            var fullPath = $"{path}{Request.QueryString}";

            var redirectUrl = $"http://localhost:3000{fullPath}";

            if (!result.IsSuccess)
                return Redirect(redirectUrl);

            return Redirect(redirectUrl);
        }
    }

}
