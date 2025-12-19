using Application.Common.Tx;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Behaviors;

public sealed class UnitOfWorkBehavior<TReq, TRes>(AppDbContext db, ICommitActions afterCommit) : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        var strategy = db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await db.Database.BeginTransactionAsync(ct);
            try
            {
                var resp = await next();

                var isSuccess = resp is IResult r ? r.Succeeded : true;

                if (isSuccess)
                {
                    await db.SaveChangesAsync(ct);
                    await tx.CommitAsync(ct);
                    foreach (var a in afterCommit.DequeueAll())
                        await a(ct);
                }
                else
                {
                    await tx.RollbackAsync(ct);
                    afterCommit.DequeueAll();
                }

                return resp;
            }
            catch
            {
                await tx.RollbackAsync(ct);
                afterCommit.DequeueAll();
                throw;
            }
        });
    }
}