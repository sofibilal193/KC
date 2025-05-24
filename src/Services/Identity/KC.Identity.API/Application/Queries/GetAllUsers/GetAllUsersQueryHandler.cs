using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using KC.Domain.Common.Identity;
using KC.Domain.Common.Types;
using KC.Identity.API.Entities;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedList<UserDto>>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMediator _mediator;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IIdentityUnitOfWork data, IMediator mediator,
            ICurrentUser currentUser, IMapper mapper)
        {
            _data = data;
            _mediator = mediator;
            _currentUser = currentUser;
            _mapper = mapper;
        }
        public async Task<PagedList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var sortExpression = request.Sort?.Replace(nameof(UserDto.FullName), nameof(User.FirstName))
                .Replace(nameof(UserDto.LastModifiedDateTimeUtc), nameof(User.ModifyDateTimeUtc)) ?? $"-{nameof(User.ModifyDateTimeUtc)}";

            IEnumerable<int> orgIds = new List<int>();
            if (_currentUser.OrgType != OrgType.SuperAdmin)
            {
                orgIds = await _mediator.Send(new GetCurrentUserOrgIdsQuery(true), cancellationToken);
            }

            var users = await _data.Users.GetOrgUsersAsync(_currentUser.OrgType, orgIds.ToList(),
                request.PageNumber, request.PageSize, request.Search, sortExpression, cancellationToken);

            return _mapper.Map<PagedList<UserDto>>(users);
        }
    }
}
