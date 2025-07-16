
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190911_UpdateEmailTemplateForCSLIandCSHI
{
    [NaatiMigration(201909111600)]
    public class UpdateEmailTemplateForCSLIandCSHI : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"                
                update tblEmailTemplate set ModifiedByNaati = 0, ModifiedUser = 40 where EmailTemplateId in (79,51,104,105,106,130,139,140,141,144,227,262)

                update tblEmailTemplate set subject = 'Your Prerequisite Test result' where EmailTemplateId in (139,140)
                update tblEmailTemplate set subject = 'NAATI Certification/Prerequisite Test Review result' where EmailTemplateId in (142,144)

                update tblEmailTemplate set Active = 0 where EmailTemplateId in (50,74,75,97,98,100,101,129,136,137,138)
            ");
        }
    }
}
