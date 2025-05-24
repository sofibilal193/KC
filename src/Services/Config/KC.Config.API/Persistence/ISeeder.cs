namespace KC.Config.API.Persistence
{
    public interface ISeeder
    {
        Task SeedItemsAsync(CancellationToken cancellationToken = default);
    }
}
