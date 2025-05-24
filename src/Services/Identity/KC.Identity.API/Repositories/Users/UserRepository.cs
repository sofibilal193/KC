using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using KC.Domain.Common.Extensions;
using KC.Domain.Common.Identity;
using KC.Domain.Common.Types;
using KC.Identity.API.Application;
using KC.Identity.API.Entities;
using KC.Identity.API.Persistence;
using KC.Persistence.Common.Extensions;
using KC.Persistence.Common.Repositories;

namespace KC.Identity.API.Repositories
{
    public class UserRepository : SqlRepository<User>, IUserRepository
    {
        private readonly IdentityDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(IdentityDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<User>> GetOrgUsersAsync(OrgType orgType, List<int> orgIds,
            int page, int pageSize, string? search, string sortExpression, CancellationToken cancellationToken = default)
        {
            IQueryable<User> query;
            if (orgType == OrgType.SuperAdmin)
            {
                query = _context.OrgUsers.AsNoTracking()
                    .Where(u => !u.IsDisabled && (u.IsInviteProcessed || !u.IsInvited))
                    .Select(u => u.User!);
            }
            else
            {
                query = _context.OrgUsers.AsNoTracking()
                    .Where(u => !u.IsDisabled && (u.IsInviteProcessed || !u.IsInvited) && orgIds.Contains(u.OrgId))
                    .Select(u => u.User!);
            }

            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                {
                    query = query.Where(u => u.Id == id);
                }
                else
                {
                    query = query.Where(u => string.Concat(u.FirstName, " ", u.LastName).Contains(search) || u.Email.Contains(search));
                }
            }

            return await query.Distinct().OrderBy(sortExpression).ToPagedListAsync(page, pageSize, cancellationToken);
        }

        public async Task<int> GetOrgUsersCountAsync(OrgType orgType, List<int> orgIds, CancellationToken cancellationToken = default)
        {
            if (orgType == OrgType.SuperAdmin)
            {
                return await _context.OrgUsers.AsNoTracking()
                    .Where(u => !u.IsDisabled && (u.IsInviteProcessed || !u.IsInvited))
                    .Select(u => u.User!).Distinct().CountAsync(cancellationToken);
            }
            else
            {
                return await _context.OrgUsers.AsNoTracking()
                    .Where(u => !u.IsDisabled && (u.IsInviteProcessed || !u.IsInvited) && orgIds.Contains(u.OrgId))
                    .Select(u => u.User!).Distinct().CountAsync(cancellationToken);
            }
        }

        public async Task<List<PermissionUserDto>> GetAllUsersByPermissionsAsync(int orgId, List<string> permissions, CancellationToken cancellationToken = default)
        {
            var roleIds = await _context.Permissions.AsNoTracking().Where(p => permissions.Any(per => per == p.Name) &&
                p.Roles.Any(r => r.OrgId == orgId || r.OrgId == null)).SelectMany(p => p.Roles).Select(r => r.Id).ToListAsync(cancellationToken);
            if (roleIds.Count > 0)
            {
                var users = await _context.Users.AsNoTracking()
                    .Where(i => i.Orgs.Any(o => o.OrgId == orgId && !o.IsDisabled && (!o.IsInvited || o.IsInviteProcessed) &&
                        roleIds.Any(r => r == o.RoleId))).Select(u => new PermissionUserDto() { Id = u.Id, FullName = u.FullName })
                    .ToListAsync(cancellationToken);
                return users;
            }
            else
            {
                return new();
            }
        }

        public async Task<List<SearchUserDto>> SearchAsync(string search, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AsNoTracking().Where(i => i.Email.Contains(search)).Select(user =>
            _mapper.Map<User, SearchUserDto>(user)).ToListAsync(cancellationToken);
        }
    }
}
