using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Mapping
{
    public class ValidationMappingMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMappingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var response = new BaseResponse<object>
                {
                    Success = false,
                    Message = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed",
                    Data = null
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
