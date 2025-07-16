using System;
using FluentMigrator;

namespace F1Solutions.Naati.Common.Migrations
    {
        public class NaatiMigrationAttribute : MigrationAttribute
        {
            public NaatiMigrationAttribute(long migrationDate) : base(migrationDate)
            {
                var migrationString = migrationDate.ToString();
                if (migrationString.Length != 12)
                {
                    throw new Exception($"Migration number length {migrationString} is invalid");
                }

                var year = migrationString.Substring(0, 4);

                if (Convert.ToInt32(year) < 2016 || Convert.ToInt32(year) > 2030)
                {
                    throw new Exception($"Migration year of {migrationString} is invalid");
                }

                var month = migrationString.Substring(4, 2);
                if (Convert.ToInt32(month) < 1 || Convert.ToInt32(month) > 12)
                {
                    throw new Exception($"Migration month of {migrationString} is invalid");
                }

                var day = migrationString.Substring(6, 2);
                if (Convert.ToInt32(day) < 1 || Convert.ToInt32(day) > 31)
                {
                    throw new Exception($"Migration day of {migrationString} is invalid");
                }

                var hour = migrationString.Substring(8, 2);
                if (Convert.ToInt32(hour) < 0 || Convert.ToInt32(hour) > 23)
                {
                    throw new Exception($"Migration hour of {migrationString} is invalid");
                }

                var minutes = migrationString.Substring(10, 2);
                if (Convert.ToInt32(minutes) < 0 || Convert.ToInt32(minutes) > 59)
                {
                    throw new Exception($"Migration minute of {migrationString} is invalid");
                }

            }
        }
}
