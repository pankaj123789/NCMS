using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationTypeTestLocationScriptGenerator : BaseScriptGenerator
    {
        public CredentialApplicationTypeTestLocationScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialApplicationTypeTestLocation";
        public override IList<string> Columns => new[] {
			"CredentialApplicationTypeTestLocationId",
			"CredentialApplicationTypeId",
			"TestLocationId"
		};

        public override void RunScripts()
        {
            // Certification
            CreateOrUpdateTableRow(new[] { "1", "2", "1" });
            CreateOrUpdateTableRow(new[] { "2", "2", "2" });
            CreateOrUpdateTableRow(new[] { "3", "2", "3" });
            CreateOrUpdateTableRow(new[] { "4", "2", "4" });
            CreateOrUpdateTableRow(new[] { "5", "2", "5" });
            CreateOrUpdateTableRow(new[] { "6", "2", "6" });
            CreateOrUpdateTableRow(new[] { "7", "2", "7" });
            CreateOrUpdateTableRow(new[] { "8", "2", "8" });
            CreateOrUpdateTableRow(new[] { "9", "2", "9" });


            // CertificationPractitioner
            CreateOrUpdateTableRow(new[] { "10", "6", "1" });
            CreateOrUpdateTableRow(new[] { "11", "6", "2" });
            CreateOrUpdateTableRow(new[] { "12", "6", "3" });
            CreateOrUpdateTableRow(new[] { "13", "6", "4" });
            CreateOrUpdateTableRow(new[] { "14", "6", "5" });
            CreateOrUpdateTableRow(new[] { "15", "6", "6" });
            CreateOrUpdateTableRow(new[] { "16", "6", "7" });
            CreateOrUpdateTableRow(new[] { "17", "6", "8" });
            CreateOrUpdateTableRow(new[] { "18", "6", "9" });

            // CCLv2
            CreateOrUpdateTableRow(new[] { "19", "7", "1" });
            CreateOrUpdateTableRow(new[] { "20", "7", "2" });
            CreateOrUpdateTableRow(new[] { "21", "7", "3" });
            CreateOrUpdateTableRow(new[] { "22", "7", "4" });
            CreateOrUpdateTableRow(new[] { "23", "7", "5" });
            CreateOrUpdateTableRow(new[] { "24", "7", "6" });
            CreateOrUpdateTableRow(new[] { "25", "7", "7" });
            CreateOrUpdateTableRow(new[] { "26", "7", "8" });

            // CLA
            CreateOrUpdateTableRow(new[] { "27", "9", "1" });
            CreateOrUpdateTableRow(new[] { "28", "9", "3" });
            CreateOrUpdateTableRow(new[] { "29", "9", "4" });
            CreateOrUpdateTableRow(new[] { "30", "9", "7" });

            // CCL v3
            //CreateOrUpdateTableRow(new[] { "31", "13", "1" });
            //CreateOrUpdateTableRow(new[] { "32", "13", "2" });
            //CreateOrUpdateTableRow(new[] { "33", "13", "3" });
            //CreateOrUpdateTableRow(new[] { "34", "13", "4" });
            //CreateOrUpdateTableRow(new[] { "35", "13", "5" });
            //CreateOrUpdateTableRow(new[] { "36", "13", "6" });
            //CreateOrUpdateTableRow(new[] { "37", "13", "7" });
            //CreateOrUpdateTableRow(new[] { "38", "13", "8" });
            CreateOrUpdateTableRow(new[] { "39", "13", "13" });
            CreateOrUpdateTableRow(new[] { "40", "2", "13" });
            CreateOrUpdateTableRow(new[] { "41", "13", "15" });


            //Practice Test
            CreateOrUpdateTableRow(new[] { "42", "17", "13" });

            //Practice Test
            //remove these
            DeleteTableRow("46");
            DeleteTableRow("43");
            DeleteTableRow("49");
            DeleteTableRow("50");
            //remove for Manilla test location on Test/UAT/Prod
            DeleteTableRow("51");
            DeleteTableRow("52");
            DeleteTableRow("53");
            DeleteTableRow("54");
            CreateOrUpdateTableRow(new[] { "43", "9", "13" });
            CreateOrUpdateTableRow(new[] { "44", "9", "6" });

            //Abu Dhabi
            CreateOrUpdateTableRow(new[] { "45", "13", "16" });

            //CCL V2 Manila
            CreateOrUpdateTableRow(new[] { "47", "7", "18" });

            //CCL V3 Manila
            CreateOrUpdateTableRow(new[] { "46", "13", "18" });
        }

        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow("31");
            DeleteTableRow("32");
            DeleteTableRow("33");
            DeleteTableRow("34");
            DeleteTableRow("35");
            DeleteTableRow("36");
            DeleteTableRow("37");
            DeleteTableRow("38");
        }              
    }
}
