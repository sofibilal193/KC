using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetOrgUserQueryHandler : IRequestHandler<GetOrgUserQuery, OrgUserDto?>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMapper _mapper;

        public GetOrgUserQueryHandler(IIdentityUnitOfWork data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<OrgUserDto?> Handle(GetOrgUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _data.Users.AsNoTracking().Include("Orgs.Role")
                .Include(u => u.Orgs.Where(o => o.OrgId == request.OrgId))
                .GetAsync(request.UserId, cancellationToken);
            var orgUser = user?.Orgs?.FirstOrDefault();
            if (orgUser is null)
            {
                return null;
            }
            return _mapper.Map<OrgUserDto>(orgUser);
        }
    }
}
