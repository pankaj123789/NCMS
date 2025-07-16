using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationTypeCredentialTypeScriptGenerator : BaseScriptGenerator
    {
        public CredentialApplicationTypeCredentialTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialApplicationTypeCredentialType";
        public override IList<string> Columns => new [] {
            "CredentialApplicationTypeCredentialTypeId",
            "CredentialApplicationTypeId",
            "CredentialTypeId",
            "HasTest",
            "AllowSupplementary",
            "AllowPaidReview"};

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "1", "1", "0", "0", "0"});
            CreateOrUpdateTableRow(new[] { "2", "1", "2", "0", "0", "0"});
            CreateOrUpdateTableRow(new[] { "3", "1", "3", "0", "0", "0"});
            CreateOrUpdateTableRow(new[] { "4", "1", "4", "0", "0", "0"});
            CreateOrUpdateTableRow(new[] { "5", "1", "5", "0", "0", "0"});
            CreateOrUpdateTableRow(new[] { "6", "1", "6", "0", "0", "0"});
            CreateOrUpdateTableRow(new[] { "7", "1", "7", "0", "0", "0"});
            CreateOrUpdateTableRow(new[] { "8", "1", "8", "0", "0", "0"});
            CreateOrUpdateTableRow(new[] { "9", "1", "9", "0", "0", "0"});
            CreateOrUpdateTableRow(new[] { "10", "1", "10", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "11", "1", "11", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "12", "1", "12", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "13", "1", "13", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "14", "2", "2", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "15", "2", "6", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "16", "2", "7", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "17", "2", "13", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "18", "3", "14", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "19", "4", "16", "1", "0", "0" });
            CreateOrUpdateTableRow(new[] { "20", "5", "17", "1", "0", "0" });
            CreateOrUpdateTableRow(new[] { "21", "1", "20", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "22", "1", "21", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "23", "1", "24", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "24", "1", "25", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "25", "1", "26", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "26", "2", "1", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "28", "2", "5", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "32", "2", "12", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "33", "2", "20", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "34", "2", "21", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "38", "1", "27", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "39", "6", "1", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "40", "6", "2", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "41", "6", "5", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "42", "6", "6", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "43", "6", "7", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "44", "6", "12", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "45", "6", "13", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "46", "6", "20", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "47", "6", "21", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "48", "7", "14", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "49", "1", "28", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "50", "8", "1", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "51", "8", "2", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "52", "8", "3", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "53", "8", "4", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "54", "8", "5", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "55", "8", "6", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "56", "8", "7", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "57", "8", "8", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "58", "8", "9", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "59", "8", "10", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "60", "8", "11", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "61", "8", "12", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "62", "8", "13", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "63", "8", "19", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "64", "8", "20", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "65", "8", "21", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "66", "8", "22", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "67", "8", "23", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "68", "8", "24", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "69", "8", "25", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "70", "8", "26", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "71", "8", "27", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "72", "8", "28", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "73", "9", "15", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "74", "10", "26", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "75", "10", "27", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "76", "11", "29", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "77", "12", "30", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "78", "2", "10", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "79", "6", "10", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "80", "2", "8", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "81", "6", "8", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "82", "2", "9", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "83", "6", "9", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "84", "2", "24", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "85", "6", "24", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "86", "2", "22", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "87", "6", "22", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "88", "2", "23", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "89", "6", "23", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "90", "2", "3", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "91", "6", "3", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "92", "13", "14", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "93", "14", "31", "1", "0", "0" });
            CreateOrUpdateTableRow(new[] { "94", "15", "32", "1", "0", "0" });
            CreateOrUpdateTableRow(new[] { "95", "16", "33", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "96", "16", "34", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "97", "16", "35", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "98", "10", "25", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "99", "10", "28", "0", "0", "0" });

            CreateOrUpdateTableRow(new[] { "100", "1", "36", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "101", "8", "36", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "102", "2", "36", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "103", "6", "36", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "104", "1", "37", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "105", "8", "37", "0", "0", "0" });
            CreateOrUpdateTableRow(new[] { "106", "2", "37", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "107", "6", "37", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "108", "17", "38", "1", "0", "0" });
            CreateOrUpdateTableRow(new[] { "109", "17", "39", "1", "0", "0" });
            CreateOrUpdateTableRow(new[] { "110", "17", "40", "1", "0", "0" });
        }

        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow("27");
            DeleteTableRow("29");
            DeleteTableRow("30");
            DeleteTableRow("31");
            DeleteTableRow("35");
            DeleteTableRow("36");
            DeleteTableRow("37");
        }
    }
}
