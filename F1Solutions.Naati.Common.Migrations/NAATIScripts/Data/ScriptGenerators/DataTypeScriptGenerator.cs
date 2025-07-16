using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class DataTypeScriptGenerator : BaseScriptGenerator
    {

        public DataTypeScriptGenerator(NaatiScriptRunner runner): base(runner) {
        }

        public override string TableName => "tblDataType";
        public override IList<string> Columns => new[] {
            "DataTypeId",
            "Name",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Bool", "Boolean" });
            CreateOrUpdateTableRow(new[] { "2", "Text", "Text" });
            CreateOrUpdateTableRow(new[] { "3", "Date", "Date" });
            CreateOrUpdateTableRow(new[] { "4", "CountryLookup", "CountryLookup" });
            CreateOrUpdateTableRow(new[] { "5", "Options", "Options" });
            CreateOrUpdateTableRow(new[] { "6", "Email", "Email" });
            CreateOrUpdateTableRow(new[] { "7", "EndorsedQualificationLookup", "Endorsed Qualification Lookup" });
            CreateOrUpdateTableRow(new[] { "8", "EndorsedQualificationStartDate", "Endorsed Qualification Start Date" });
            CreateOrUpdateTableRow(new[] { "9", "EndorsedQualificationEndDate", "Endorsed Qualification End Date" });
            CreateOrUpdateTableRow(new[] { "10", "EndorseInstitutionLookup", "Endorsed Institution Lookup" });
            CreateOrUpdateTableRow(new[] { "11", "EndorsedLocationLookup", "Endorsed Location Lookup" });
            CreateOrUpdateTableRow(new[] { "12", "EndorsedQualificationIdText", "Endorsed Qualification Id Text" });
            CreateOrUpdateTableRow(new[] { "13", "RadioOptions", "Radio Options" });
        }

    }
}
