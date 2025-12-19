using Microsoft.AspNetCore.Mvc;

public static class HttpResultExtensions
{
  public static IActionResult ToActionResult(this Result r, ControllerBase c)
  {
    if (r.Succeeded) return c.NoContent(); // para commands sin body
    return r.Error!.Type switch
    {
      ErrorType.Validation => c.BadRequest(c.ProblemDetails(r.Error!, 400)),
      ErrorType.NotFound => c.NotFound(c.ProblemDetails(r.Error!, 404)),
      ErrorType.Conflict => c.Conflict(c.ProblemDetails(r.Error!, 409)),
      ErrorType.Unauthorized => c.Unauthorized(),
      ErrorType.Forbidden => c.Forbid(),
      _ => c.StatusCode(500, c.ProblemDetails(r.Error!, 500))
    };
  }

  public static IActionResult ToActionResult<T>(this Result<T> r, ControllerBase c)
  {
    if (r.Succeeded) return c.Ok(r.Value);
    return r.Error!.Type switch
    {
      ErrorType.Validation => c.BadRequest(c.ProblemDetails(r.Error!, 400)),
      ErrorType.NotFound => c.NotFound(c.ProblemDetails(r.Error!, 404)),
      ErrorType.Conflict => c.Conflict(c.ProblemDetails(r.Error!, 409)),
      ErrorType.Unauthorized => c.Unauthorized(),
      ErrorType.Forbidden => c.Forbid(),
      _ => c.StatusCode(500, c.ProblemDetails(r.Error!, 500))
    };
  }

  private static ProblemDetails ProblemDetails(this ControllerBase c, Error e, int status)
      => new()
      {
        Status = status,
        Title = e.Code,
        Detail = e.Message,
        Type = $"{status}",
        Instance = c.HttpContext.TraceIdentifier
      };
}
