using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160830_AddPaymentsTable
{
    [NaatiMigration(201608300951)]
    public class AddPaymentsTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("Payment").InSchema("Internal")
                .WithColumn("PaymentId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("InvoiceId").AsInt32().Nullable()
                .WithColumn("InvoiceLineId").AsInt32().Nullable()
                .WithColumn("PaymentDate").AsDateTime().Nullable()
                .WithColumn("Office").AsAnsiString(100).Nullable()
                .WithColumn("PayerNAATINumber").AsInt32().Nullable()
                .WithColumn("PayerName").AsAnsiString(300).Nullable()
                .WithColumn("PaymentType").AsAnsiString(20).Nullable()
                .WithColumn("InvoiceLineTotalPaid").AsCurrency().Nullable()
                .WithColumn("InvoiceTotalPaid").AsCurrency().Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
