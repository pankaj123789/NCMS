namespace F1Solutions.Naati.Common.Migrations.NAATI._20171017_InitialDataPopulationNov
{
   [NaatiMigration(201710171115)]
    public class InitialDataPopulationNov : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.DocumentTypeInsert);
            Execute.Sql(Sql.CredentialApplicationInsert);
            Execute.Sql(Sql.CredentialApplicationTypeCredentialTypeInsert);
            Execute.Sql(Sql.DataTypeInsert);
            Execute.Sql(Sql.CredentialApplicationFieldInsert);
            Execute.Sql(Sql.CredentialApplicationStatusTypeInsert);
            Execute.Sql(Sql.CredentialApplicationFormInsert);
            Execute.Sql(Sql.CredentialApplicationFormSectionInsert);
            Execute.Sql(Sql.CredentialApplicationFormAnswerTypeInsert);
            Execute.Sql(Sql.CredentialApplicationFormQuestionTypeInsert);
            Execute.Sql(Sql.CredentialApplicationFormQuestionInsert);
            Execute.Sql(Sql.CredentialApplicationFormAnswerOptionInsert);
            Execute.Sql(Sql.CredentialApplicationFormQuestionAnswerOptionInsert);
            Execute.Sql(Sql.CredentialApplicationFormQuestionLogicInsert);
            Execute.Sql(Sql.CredentialApplicationFormActionTypeInsert);
            Execute.Sql(Sql.CredentialApplicationFormAnswerOptionActionTypeInsert);
            Execute.Sql(Sql.CredentialApplicationFormAnswerOptionDocumentTypeInsert);
            Execute.Sql(Sql.CredentialTypeTemplateInsert);
            Execute.Sql(Sql.CredentialApplicationTypeDocumentTypeInsert);
            Execute.Sql("DELETE FROM tblEmailTemplate");
        }
    }
}
