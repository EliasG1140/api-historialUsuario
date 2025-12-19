using Infrastructure.Data;
using MediatR;
using Domain.Personas;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Personas.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record UpdatePersonaCommand(
    int Id,
    string Nombre,
    string Apellido,
    string Cedula,
    string? Apodo,
    string Telefono,
    string Direccion,
    string Descripcion,
    bool EsLider,
    int Barrio,
    int CodigoC,
    List<int> Lengua,
    int? Lider,
    int MesaVotacion,
    List<int>? CodigoB
) : IRequest<Result<UpdatePersonaResponse>>;

public sealed record UpdatePersonaResponse(string Message);

//* ------------------------------- Handler ------------------------------- */
public sealed class UpdatePersonaCommandHandler(AppDbContext db, IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdatePersonaCommand, Result<UpdatePersonaResponse>>
{
    public async Task<Result<UpdatePersonaResponse>> Handle(UpdatePersonaCommand request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;
        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
          ?? user?.FindFirst("sub")?.Value;

        var persona = await db.Personas
            .Include(p => p.CodigosB)
            .Include(p => p.Lenguas)
            .Include(p => p.PersonasACargo)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (persona == null)
        {
            return Result<UpdatePersonaResponse>.Fail(Error.NotFound("Persona no encontrada.", "Persona.Update.NotFound"));
        }

        var existsCedula = await db.Personas.AnyAsync(p => p.Cedula == request.Cedula && p.Id != request.Id, cancellationToken);
        if (existsCedula)
        {
            return Result<UpdatePersonaResponse>.Fail(Error.Conflict("La cédula ya está registrada.", "Persona.Update.CedulaExists"));
        }

        if (persona.IsLider && !request.EsLider)
        {
            var personasACargo = persona.PersonasACargo?.Count ?? 0;
            if (personasACargo > 0)
            {
                return Result<UpdatePersonaResponse>.Fail(Error.Conflict("No puede dejar de ser líder porque tiene personas a cargo.", "Persona.Update.Lider.ConPersonasACargo"));
            }
        }

        if (!persona.IsLider && request.EsLider)
        {
            persona.LiderId = null;
        }

        persona.Nombre = request.Nombre;
        persona.Apellido = request.Apellido;
        persona.Cedula = request.Cedula;
        persona.Apodo = request.Apodo;
        persona.Telefono = request.Telefono;
        persona.Direccion = request.Direccion;
        persona.Descripcion = request.Descripcion;
        persona.IsLider = request.EsLider;
        persona.BarrioId = request.Barrio;
        persona.CodigoCId = request.CodigoC;
        persona.LiderId = request.Lider;
        persona.MesaVotacionId = request.MesaVotacion;
        persona.LastModifiedAt = DateTime.UtcNow;
        persona.LastModifiedByUserId = userId;

        persona.Lenguas.Clear();
        if (request.Lengua is { Count: > 0 })
        {
            persona.Lenguas = request.Lengua.Select(id => new PersonaLengua { PersonaId = persona.Id, LenguaId = id }).ToList();
        }

        persona.CodigosB?.Clear();
        if (request.CodigoB is { Count: > 0 })
        {
            persona.CodigosB = request.CodigoB.Select(id => new PersonaCodigoB { PersonaId = persona.Id, CodigoBId = id }).ToList();
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result<UpdatePersonaResponse>.Ok(new UpdatePersonaResponse("Persona actualizada exitosamente."));
    }
}
