using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160906_AlterInvoiceTable
{
    [NaatiMigration(201609061252)]
    public class AlterInvoiceTable : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("ALTER TABLE [Internal].[Invoice] ADD [CreatedDate] DATETIME NULL");
            Execute.Sql("ALTER TABLE [Internal].[Invoice] ADD [Office] VARCHAR(100) NULL");
            Execute.Sql("ALTER TABLE [Internal].[Invoice] ADD [FullNameResponsible] VARCHAR(300) NULL");
            Execute.Sql("ALTER TABLE [Internal].[Invoice] ADD [RefundedInvoiceLineID] INT NULL");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
