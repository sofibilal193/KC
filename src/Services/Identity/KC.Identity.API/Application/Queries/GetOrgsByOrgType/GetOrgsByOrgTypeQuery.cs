using FluentValidation;
using MediatR;
using KC.Domain.Common.Types;

namespace KC.Identity.API.Application
{
    /// <summary>
    /// Query for getting a paged list of organizations for a user logged in.
    /// </summary>
    /// <param name="ParentOrgId">Id of Parent Org.</param>
    /// <param name="Page">Page number of records to retrieve.</param>
    /// <param name="PageSize">Number of records to retrieve.</param>
    /// <param name="Search">Partial search filter by Id or Name of Org.</param>
    /// <param name="Sort">Field to sort records by (Adding a `-` before the name switches to sorting in descending order).</param>
    public readonly record struct GetOrgsByOrgTypeQuery(
        int? ParentOrgId,
        int Page,
        int PageSize,
        string? Search,
        string? Sort
    ) : IRequest<PagedList<OrgsByTypeDto?>>;

    public class GetOrgsByOrgTypeQueryValidator : AbstractValidator<GetOrgsByOrgTypeQuery>
    {
        public GetOrgsByOrgTypeQueryValidator()
        {
            RuleFor(q => q.Page)
                .GreaterThan(0);
            RuleFor(q => q.PageSize)
                .InclusiveBetween(1, 100);
        }
    }
}
