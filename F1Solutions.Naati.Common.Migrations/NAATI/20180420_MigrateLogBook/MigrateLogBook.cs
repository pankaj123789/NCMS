
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180420_MigrateLogBook
{
    [NaatiMigration(201804201600)]
    public class MigrateLogBook: NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("Update tblProfessionalDevelopmentActivity set ProfessionalDevelopmentCategoryId = 32 WHERE ProfessionalDevelopmentCategoryId =31");
        }
    }
}
