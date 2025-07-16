using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20171116_NonEndorsedCountryQualification
{
    [NaatiMigration(201711161313)]
    public class NonEndorsedCountryQualification : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationField ON");
            Insert.IntoTable("tblCredentialApplicationField").Row(new
            {
                CredentialApplicationFieldId = 45,
                CredentialApplicationTypeId = 2,
                Name = "Qualification Country",
                Section = "Non-Endorsed Qualification Details",
                DataTypeId = 4,
                PerCredentialRequest = false,
                Description = String.Empty,
                Mandatory = false,
            });
            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationField OFF");

            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationFormAnswerType ON");
            Insert.IntoTable("tblCredentialApplicationFormAnswerType").Row(new
            {
                CredentialApplicationFormAnswerTypeId = 13,
                Name = "CountrySelector",
                DisplayName = "Country Selector"
            });
            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationFormAnswerType OFF");

            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationFormQuestionType ON");
            Insert.IntoTable("tblCredentialApplicationFormQuestionType").Row(new
            {
                CredentialApplicationFormQuestionTypeId = 31,
                Text = "Please select the country where you undertook this qualification",
                CredentialApplicationFormAnswerTypeId = 13,
                Description = String.Empty
            });
            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationFormQuestionType OFF");

            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationFormQuestion ON");
            Insert.IntoTable("tblCredentialApplicationFormQuestion").Row(new
            {
                CredentialApplicationFormQuestionId = 50,
                CredentialApplicationFormSectionId = 9,
                CredentialApplicationFormQuestionTypeId = 31,
                CredentialApplicationFieldId = 45,
                DisplayOrder = 5
            });
            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationFormQuestion OFF");

            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationFormQuestionLogic ON");
            Insert.IntoTable("tblCredentialApplicationFormQuestionLogic").Row(new
            {
                CredentialApplicationFormQuestionLogicId = 36,
                CredentialApplicationFormQuestionId = 50,
                CredentialApplicationFormQuestionAnswerOptionId = 17,
                Not = false,
                And = false
            });
            Execute.Sql("Set IDENTITY_INSERT tblCredentialApplicationFormQuestionLogic OFF");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
