using DTOs.Response;
using System.Net;
using System.Text.Json;

namespace PRN232_SU25_SE_HE176690.api.ExceptionMiddleware
{
    public class StatusCodeHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public StatusCodeHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            var response = context.Response;

            if (response.HasStarted)
            {
                return;
            }

            ErrorResponse error = null;

            switch (response.StatusCode)
            {
                case (int)HttpStatusCode.Unauthorized:
                    error = ApiErrorResponses.Unauthorized;
                    break;

                case (int)HttpStatusCode.Forbidden:
                    error = ApiErrorResponses.Forbidden;
                    break;
            }

            if (error != null)
            {
                response.ContentType = "application/json";
                var json = JsonSerializer.Serialize(error);
                await response.WriteAsync(json);
            }
        }
    }
}
