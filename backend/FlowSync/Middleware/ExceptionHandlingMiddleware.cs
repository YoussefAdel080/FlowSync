using FlowSync.Application.Exceptions;
using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Middleware;

/// <summary>
/// Middleware that catches application exceptions and maps them to consistent HTTP error responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger used for unexpected failures.</param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware and handles any thrown exceptions.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var message = ex.Errors.FirstOrDefault()?.ErrorMessage ?? ex.Message;
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, message);
        }
        catch (BadRequestException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new BaseResponse<object>
        {
            Success = false,
            Message = message,
            Data = null
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
