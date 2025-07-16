using System;
using FluentMigrator;

namespace F1Solutions.NAATI.SAM.Migrations._20160802_PersonSearchTerm
{
    [Migration(201608020943)]
    public class PersonSearchTerm:Migration
    {
        public override void Up()
        {
            this.ExecuteSql(Sql.CreatePersonSearchTerm);
        }
        
        public override void Down()
        {
            throw new NotImplementedException();
        }       
    }
}
