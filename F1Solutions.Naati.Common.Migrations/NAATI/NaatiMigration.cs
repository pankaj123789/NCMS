using System;
using System.Linq;
using FluentMigrator;

namespace F1Solutions.Naati.Common.Migrations.NAATI
{
    public abstract class NaatiMigration : Migration
    {
        public NaatiMigration()
        {
            if (this.GetType().CustomAttributes.Any(y => typeof(MigrationAttribute).IsAssignableFrom(y.AttributeType) && !(typeof(NaatiMigrationAttribute).IsAssignableFrom(y.AttributeType))))
            {
                throw new Exception($"Migration {this.GetType().Name} should implement {typeof(NaatiMigrationAttribute).Name} attribute");
            }
        }
        public override void Up()
        {
            throw new System.NotImplementedException();
        }

        public override void Down()
        {
            throw new System.NotImplementedException();
        }
    }
}
