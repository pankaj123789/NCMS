using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20181205_AddLinkedCredentialRequests
{
    [NaatiMigration(201812051000)]
    public class AddLinkedCredentialRequests : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("CredentialId")
                .OnTable("CredentialRequestsHistory")
                .AsInt32()
                .Nullable();

            Create.Column("LinkedCredentialRequestId")
                .OnTable("CredentialRequestsHistory")
                .AsInt32()
                .Nullable();

            Create.Column("LinkedCredentialRequestReason")
                .OnTable("CredentialRequestsHistory")
                .AsString(50)
                .Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
