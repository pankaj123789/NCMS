using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160907_AdditionalChangesForETL
{
    [NaatiMigration(201609071518)]
    public class AddObsoletedDateIndex : NaatiMigration
    {
        public override void Up()
        {
            foreach (var table in TableInfo.Tables)
            {
                var commandText = table.CreateObsoleteDateIndexQuery;

                Execute.WithConnection((dbConnection, dbTransaction) =>
                {
                    using (var command = dbConnection.CreateCommand())
                    {
                        command.CommandTimeout = 300;
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
