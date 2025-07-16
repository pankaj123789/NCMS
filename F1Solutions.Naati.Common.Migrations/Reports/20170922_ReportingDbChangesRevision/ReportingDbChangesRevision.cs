using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20170922_ReportingDbChangesRevision
{
    [NaatiMigration(201709221500)]
    public class ReportingDbChangesRevision : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("NAATINumber").OnTable("PersonHistory").AsInt32().Nullable();
            Create.Column("PractitionerNumber").OnTable("PersonHistory").AsInt32().Nullable();
            Create.Column("DoNotSendCorrespondence").OnTable("PersonHistory").AsInt32().Nullable();

            Create.Column("NAATINumber").OnTable("ApplicationHistory").AsInt32().Nullable();
            Create.Column("PractitionerNumber").OnTable("ApplicationHistory").AsInt32().Nullable();
            Rename.Column("StatusChangeDate").OnTable("ApplicationHistory").To("StatusDateModified");
            Rename.Column("ModifiedUser").OnTable("ApplicationHistory").To("StatusModifiedUser");
            Delete.Column("EnteredUser").FromTable("ApplicationHistory");
            Create.Column("EnteredUser").OnTable("ApplicationHistory").AsString(100).Nullable();

            Create.Column("NAATINumber").OnTable("CredentialRequestsHistory").AsInt32().Nullable();
            Create.Column("PractitionerNumber").OnTable("CredentialRequestsHistory").AsInt32().Nullable();
            Create.Column("Language1Code").OnTable("CredentialRequestsHistory").AsString(10).Nullable();
            Create.Column("Language1Group").OnTable("CredentialRequestsHistory").AsString(50).Nullable();
            Create.Column("Language2Code").OnTable("CredentialRequestsHistory").AsString(10).Nullable();
            Create.Column("Language2Group").OnTable("CredentialRequestsHistory").AsString(50).Nullable();
            Rename.Column("ModifiedUser").OnTable("CredentialRequestsHistory").To("StatusModifiedUser");
            Rename.Column("StatusChangedDate").OnTable("CredentialRequestsHistory").To("StatusDateModified");

            Create.Column("NAATINumber").OnTable("CredentialsHistory").AsInt32().Nullable();
            Create.Column("PractitionerNumber").OnTable("CredentialsHistory").AsInt32().Nullable();
            Create.Column("CredentialTypeExternalName").OnTable("CredentialsHistory").AsString(50).Nullable();
            Create.Column("Certification").OnTable("CredentialsHistory").AsBoolean();
            Create.Column("Language1").OnTable("CredentialsHistory").AsString(50).Nullable();
            Create.Column("Language1Code").OnTable("CredentialsHistory").AsString(10).Nullable();
            Create.Column("Language1Group").OnTable("CredentialsHistory").AsString(50).Nullable();
            Create.Column("Language2").OnTable("CredentialsHistory").AsString(50).Nullable();
            Create.Column("Language2Code").OnTable("CredentialsHistory").AsString(10).Nullable();
            Create.Column("Language2Group").OnTable("CredentialsHistory").AsString(50).Nullable();
            Rename.Column("Direction").OnTable("CredentialsHistory").To("DirectionDisplayName");
            Rename.Column("CredentialType").OnTable("CredentialsHistory").To("CredentialTypeInternalName");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
