using KC.Domain.Common.Repositories;

namespace KC.Config.API.Repositories
{
    public interface IConfigUnitOfWork : IUnitOfWork
    {
        IConfigRepository Configs { get; }

        IOrgConfigRepository OrgConfigs { get; }

        IOrgConfigValueRepository OrgConfigValues { get; }

        IUserConfigRepository UserConfigs { get; }

        IUserConfigValueRepository UserConfigValues { get; }
    }
}
