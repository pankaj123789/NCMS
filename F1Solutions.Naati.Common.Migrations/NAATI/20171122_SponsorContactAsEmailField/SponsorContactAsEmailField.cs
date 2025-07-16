using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20171122_SponsorContactAsEmailField
{
    [NaatiMigration(201711221116)]
    public class SponsorContactAsEmailField : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("Set IDENTITY_INSERT tblDataType ON");
            Insert.IntoTable("tblDataType").Row(new
            {
                DataTypeId = 6,
                Name = "Email",
                DisplayName = "Email"
            });
            Execute.Sql("Set IDENTITY_INSERT tblDataType OFF");

            Execute.Sql("UPDATE tblCredentialApplicationField SET DataTypeId = 6 WHERE[CredentialApplicationFieldId] = 36");
            Execute.Sql("UPDATE tblCredentialApplicationField SET DataTypeId = 6 WHERE[CredentialApplicationFieldId] = 40");

            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationFormAnswerType ON");
            Insert.IntoTable("tblCredentialApplicationFormAnswerType").Row(new
            {
                CredentialApplicationFormAnswerTypeId = 14, 
                Name = "Email",
                DisplayName = "Email"
            });
            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationFormAnswerType OFF");

            Execute.Sql("UPDATE tblCredentialApplicationFormQuestionType SET CredentialApplicationFormAnswerTypeId = 14 WHERE CredentialApplicationFormQuestionTypeId = 26");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
