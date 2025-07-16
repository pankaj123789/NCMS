
namespace F1Solutions.Naati.Common.Migrations.NAATI._20220707_AddEmailTemplateIdtotblEmailMessage
{
    [NaatiMigration(202207071500)]
    class AddEmailTemplateIdtotblEmailMessage : NaatiMigration
    {
        public override void Up()
        {
            this.ExecuteSql(Sql.Relationship_tblEmailMessage_tblEmailMessage_FKEmailTemplateId);
        }
    }
}
