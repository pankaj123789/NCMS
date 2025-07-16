namespace F1Solutions.Naati.Common.Contracts.Dal.Services
{
    public interface IDatabaseMigrationService
    {
        (bool migrated, bool error) MigrateDb();

        string GetAssemblyFileVersion();
    }
}
