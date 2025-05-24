using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using KC.Domain.Common.Exceptions;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetOrgUsersQueryHandler : IRequestHandler<GetOrgUsersQuery, IList<OrgUserDto>>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMapper _mapper;

        public GetOrgUsersQueryHandler(IIdentityUnitOfWork data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<IList<OrgUserDto>> Handle(GetOrgUsersQuery request, CancellationToken cancellationToken)
        {
            var org = await _data.Orgs.AsNoTracking().Include("Users.User")
                .Include("Users.Role").GetAsync(request.OrgId, cancellationToken);
            if (org is not null)
            {
                if (org.Users.Count > 0)
                {
                    var dto = _mapper.Map<IList<OrgUserDto>>(org.Users);
                    return dto
                        .OrderBy(u => u.Id)
                        .ThenBy(u => u.FullName)
                        .ToList();
                }
            }
            else
            {
                throw new NotFoundException();
            }
            return new List<OrgUserDto>();
        }
    }
}
