using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using KC.Domain.Common.Entities;
using KC.Domain.Common.Identity;

namespace KC.Persistence.Common
{
    internal static class MediatorExtensions
    {
        internal static async Task DispatchDomainEventsAsync(this IMediator mediator,
            DbContext ctx, ICurrentUser user, CancellationToken cancellationToken = default)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents?.Count > 0);

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents!)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                domainEvent.SetUserSource(user.UserId, user.FullName, user.Source);
                await mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
