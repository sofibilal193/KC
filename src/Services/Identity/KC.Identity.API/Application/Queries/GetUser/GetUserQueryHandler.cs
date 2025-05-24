using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using KC.Identity.API.Entities;
using KC.Identity.API.Repositories;

namespace KC.Identity.API.Application
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto?>
    {
        private readonly IIdentityUnitOfWork _data;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(IIdentityUnitOfWork data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }
        public async Task<UserDto?> Handle(GetUserQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _data.Users.AsNoTracking().GetAsync(request.UserId, cancellationToken);
            return _mapper.Map<User?, UserDto?>(user);
        }
    }
}
