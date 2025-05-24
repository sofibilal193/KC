using KC.Config.API.Persistence;

namespace KC.Config.API.Repositories
{
    public class ConfigUnitOfWork : IConfigUnitOfWork
    {
        private readonly ConfigDbContext _context;
        private bool _disposedValue;

        public IConfigRepository Configs { get; init; }

        public IOrgConfigRepository OrgConfigs { get; init; }

        public IOrgConfigValueRepository OrgConfigValues { get; init; }

        public IUserConfigRepository UserConfigs { get; init; }

        public IUserConfigValueRepository UserConfigValues { get; init; }

        public ConfigUnitOfWork(ConfigDbContext context, IConfigRepository configs,
            IOrgConfigRepository orgConfigs, IOrgConfigValueRepository orgConfigValues,
            IUserConfigRepository userConfigs, IUserConfigValueRepository userConfigValues)
        {
            _context = context;

            // repositories
            Configs = configs;
            OrgConfigs = orgConfigs;
            OrgConfigValues = orgConfigValues;
            UserConfigs = userConfigs;
            UserConfigValues = userConfigValues;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveEntitiesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
