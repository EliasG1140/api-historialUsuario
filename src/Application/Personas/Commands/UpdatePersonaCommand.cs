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
    string? Direccion,
    string? Descripcion,
    bool EsLider,
    bool EsCoordinador,
    int? Barrio,
    int CodigoC,
    List<int>? Lengua,
    int? Lider,
    int? Coordinador,
    string? Familia,
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

        // Validaciones de asignación de lider y coordinador
        if (request.EsCoordinador)
        {
            // Si es coordinador, no debe tener lider asignado
            if (request.Lider != null)
            {
                return Result<UpdatePersonaResponse>.Fail(Error.Validation("Un coordinador no puede tener un líder asignado.", "Persona.Update.CoordinadorSinLider"));
            }
        }
        else if (request.EsLider)
        {
            // Si es líder, debe tener un coordinador asignado
            if (request.Coordinador == null)
            {
                return Result<UpdatePersonaResponse>.Fail(Error.Validation("Un líder debe tener un coordinador asignado.", "Persona.Update.LiderSinCoordinador"));
            }
        }
        else
        {
            // Si no es ni líder ni coordinador, debe tener ambos asignados
            if (request.Lider == null || request.Coordinador == null)
            {
                return Result<UpdatePersonaResponse>.Fail(Error.Validation("Debe asignar tanto un líder como un coordinador.", "Persona.Update.SinLiderNiCoordinador"));
            }
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


        persona.Nombre = request.Nombre.ToUpper();
        persona.Apellido = request.Apellido.ToUpper();
        persona.Cedula = request.Cedula;
        persona.Apodo = request.Apodo != null ? request.Apodo.ToUpper() : null;
        persona.Telefono = request.Telefono;
        persona.Direccion = request.Direccion ?? string.Empty;
        persona.Descripcion = request.Descripcion ?? string.Empty;
        persona.IsLider = request.EsLider;
        persona.IsCoordinador = request.EsCoordinador;
        persona.CoordinadorId = request.Coordinador;
        persona.BarrioId = request.Barrio;
        persona.CodigoCId = request.CodigoC;
        persona.LiderId = request.Lider;
        persona.MesaVotacionId = request.MesaVotacion;
        persona.Familia = request.Familia != null ? request.Familia.Trim().ToUpper() : null;
        persona.LastModifiedAt = DateTime.UtcNow;
        persona.LastModifiedByUserId = userId;

        if (persona.Lenguas != null)
            persona.Lenguas.Clear();
        else
            persona.Lenguas = new List<PersonaLengua>();
        if (request.Lengua is { Count: > 0 })
        {
            persona.Lenguas = request.Lengua.Select(id => new PersonaLengua { PersonaId = persona.Id, LenguaId = id }).ToList();
        }

        if (persona.CodigosB != null)
            persona.CodigosB.Clear();
        else
            persona.CodigosB = new List<PersonaCodigoB>();
        if (request.CodigoB is { Count: > 0 })
        {
            persona.CodigosB = request.CodigoB.Select(id => new PersonaCodigoB { PersonaId = persona.Id, CodigoBId = id }).ToList();
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result<UpdatePersonaResponse>.Ok(new UpdatePersonaResponse("Persona actualizada exitosamente."));
    }
}
