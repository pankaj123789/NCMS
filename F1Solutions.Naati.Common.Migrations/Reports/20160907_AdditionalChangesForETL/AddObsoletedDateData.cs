using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160907_AdditionalChangesForETL
{
    [NaatiMigration(201609071515)]
    public class AddObsoletedDateData : NaatiMigration
    {
        public override void Up()
        {
            foreach (var table in TableInfo.Tables)
            {
                var commandText = table.UpdateObsoletedDateQuery;

                Execute.WithConnection((dbConnection, dbTransaction) =>
                {
                    using (var command = dbConnection.CreateCommand())
                    {
                        command.CommandTimeout = 600;
                        command.Transaction = dbTransaction;
                        command.CommandText = commandText;
                        command.ExecuteNonQuery();
                    }
                });
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
