using Microsoft.AspNetCore.Mvc;

public class ExceptionMiddleware : IMiddleware
{
  private readonly ILogger<ExceptionMiddleware> _logger;
  public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) => _logger = logger;

  public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
  {
    try { await next(ctx); }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled exception");
      ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
      ctx.Response.ContentType = "application/problem+json";
      var pd = new ProblemDetails
      {
        Status = 500,
        Title = "unexpected",
        Detail = "Ha ocurrido un error inesperado.",
        Instance = ctx.TraceIdentifier,
        Type = "500"
      };
      await ctx.Response.WriteAsJsonAsync(pd);
    }
  }
}
