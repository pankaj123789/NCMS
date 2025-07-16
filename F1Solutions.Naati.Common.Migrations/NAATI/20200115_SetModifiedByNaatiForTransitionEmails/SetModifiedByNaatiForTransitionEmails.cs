using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20200115_SetModifiedByNaatiForTransitionEmails
{
    [NaatiMigration(202001151050)]
    public class SetModifiedByNaatiForTransitionEmails : NaatiMigration
    {
        public override void Up()
        {
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 52});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 53});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 54});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 55});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 56});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 57});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 58});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 59});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 131});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 132});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 145});
            Update.Table("tblEmailTemplate").Set(new {ModifiedByNaati = 0}).Where(new {EmailTemplateId = 146});
        }
    }
}
