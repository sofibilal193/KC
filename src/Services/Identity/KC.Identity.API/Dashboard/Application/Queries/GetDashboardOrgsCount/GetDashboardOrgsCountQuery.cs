using MediatR;
using KC.Domain.Common;

namespace KC.Identity.API.Dashboard.Application
{
    public readonly record struct GetDashboardOrgsCountQuery() : IRequest<CountDto>;
}
