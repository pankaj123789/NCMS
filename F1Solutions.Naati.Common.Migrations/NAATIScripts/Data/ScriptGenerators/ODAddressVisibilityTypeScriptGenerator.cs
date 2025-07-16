using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class ODAddressVisibilityTypeScriptGenerator : BaseScriptGenerator
    {
        public ODAddressVisibilityTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblODAddressVisibilityType";
        public override IList<string> Columns => new[] {
                                                           "ODAddressVisibilityTypeId",
                                                           "Name",
                                                           "DisplayName"                                                           
                                                       };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "DoNotShow", "Do Not Show" });
            CreateOrUpdateTableRow(new[] { "2", "StateOnly", "State Only" });
            CreateOrUpdateTableRow(new[] { "3", "StateAndSuburb", "State and Suburb" });
            CreateOrUpdateTableRow(new[] { "4", "FullAddress", "Full Address" });            
        }
    }
}
