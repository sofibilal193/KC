using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MediatR;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetUserOrgsQueryHandler : IRequestHandler<GetUserOrgsQuery, IList<UserOrgDto>>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserOrgsQueryHandler> _logger;

        public GetUserOrgsQueryHandler(IIdentityUnitOfWork data, IMapper mapper,
            ILogger<GetUserOrgsQueryHandler> logger)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IList<UserOrgDto>> Handle(GetUserOrgsQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _data.Users.AsNoTracking().Include("Orgs.Org").Include("Orgs.Role.Permissions")
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (user is not null)
            {
                var orgUserList = user.Orgs?.ToList();

                if (orgUserList?.Count > 0)
                {
                    var orgs = _mapper.Map<IList<UserOrgDto>>(orgUserList);

                    _logger.LogDebug("Orgs {count}.", orgs.Count);

                    if (request.Type.HasValue)
                    {
                        return orgs.Where(x => x.OrgType == request.Type).ToList();
                    }
                    else
                    {
                        return orgs.ToList();
                    }
                }
                else
                {
                    _logger.LogInformation("Orgs not found");
                    return new List<UserOrgDto>();
                }
            }
            _logger.LogInformation("Orgs not found");
            return new List<UserOrgDto>();
        }
    }
}
