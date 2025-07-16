
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171003_ConfigureInitialData
{
    [NaatiMigration(201710031701)]
    public class ConfigureInitialDataForNcms1_0_0 : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.ClearExistingData);
            Execute.Sql(Sql.LoadtblCredentialApplicationStatusType);
            Execute.Sql(Sql.LoadtblCredentialRequestStatusType);
            Execute.Sql(Sql.LoadtblCredentialApplicationType);
            Execute.Sql(Sql.LoadtblCredentialCategory);
            Execute.Sql(Sql.LoadtblSkillType);
            Execute.Sql(Sql.LoadtblDirectionType);
            Execute.Sql(Sql.LoadtblCredentialType);
            Execute.Sql(Sql.LoadtblSkill);
            Execute.Sql(Sql.LoadtblCredentialApplicationTypeCredentialType);
            Execute.Sql(Sql.LoadtblDataType);
            Execute.Sql(Sql.LoadtblCredentialApplicationField);
            Execute.Sql(Sql.LoadtblCredentialApplicationTypeDocumentType);
            Execute.Sql(Sql.LoadtblEmailTemplate);
            Execute.Sql("update tblSystemValue set [Value] = 'transition@naati.com.au' where ValueKey = 'DefaultEmailSenderAddress'");
        }
    }
}
