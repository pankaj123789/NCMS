using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class TestLocationScriptGenerator : BaseScriptGenerator
    {
        public TestLocationScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblTestLocation";
        public override IList<string> Columns => new[] {
                                                           "TestLocationId",
                                                           "OfficeId",
                                                           "CountryId",
                                                           "Name"                                                          
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "3", "13", "Sydney" });
            CreateOrUpdateTableRow(new[] { "2", "1", "13", "Canberra" });
            CreateOrUpdateTableRow(new[] { "3", "9", "13", "Melbourne" });
            CreateOrUpdateTableRow(new[] { "4", "6", "13", "Brisbane" });
            CreateOrUpdateTableRow(new[] { "5", "8", "13", "Hobart" });
            CreateOrUpdateTableRow(new[] { "6", "7", "13", "Adelaide" });
            CreateOrUpdateTableRow(new[] { "7", "10", "13", "Perth" });
            CreateOrUpdateTableRow(new[] { "8", "4", "13", "Darwin" });
            CreateOrUpdateTableRow(new[] { "9", "5", "153", "Auckland" });
            CreateOrUpdateTableRow(new[] { "10", "10", "13", "Broome" });
            CreateOrUpdateTableRow(new[] { "11", "6", "13", "Cairns" });
            CreateOrUpdateTableRow(new[] { "12", "6", "13", "Thursday Island" });
            CreateOrUpdateTableRow(new[] { "13", "316", "13", "Online" });
            CreateOrUpdateTableRow(new[] { "14", "4", "13", "Alice Springs" });
            CreateOrUpdateTableRow(new[] { "15", "123", "162", "Karachi" });
            CreateOrUpdateTableRow(new[] { "16", "123", "221", "Abu Dhabi" });
            //Note 18 is being used because the Prtod Script inserted that ID
            CreateOrUpdateTableRow(new[] { "18", "123", "168", "Manila" });
        }
    }
}

