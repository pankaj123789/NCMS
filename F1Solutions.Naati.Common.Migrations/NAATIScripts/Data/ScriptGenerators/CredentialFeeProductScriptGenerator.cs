using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialFeeProductScriptGenerator : BaseScriptGenerator
    {
        public CredentialFeeProductScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialFeeProduct";
        public override IList<string> Columns => new[] {
                                                           "CredentialFeeProductId",
                                                           "CredentialApplicationTypeId",
                                                           "CredentialTypeId",
                                                           "FeeTypeId",
                                                           "ProductSpecificationId",
                                                           "CredentialApplicationRefundPolicyId"
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "2", "2", "3", "4", "2" });
            CreateOrUpdateTableRow(new[] { "2", "2", "6", "3", "12", "3" });
            CreateOrUpdateTableRow(new[] { "3", "2", "7", "3", "8", "2" });
            CreateOrUpdateTableRow(new[] { "4", "2", "13", "3", "12", "3" });
            CreateOrUpdateTableRow(new[] { "5", "3", "14", "3", "15", "1" });
            CreateOrUpdateTableRow(new[] { "6", "4", "16", "3", "13", "2" });
            CreateOrUpdateTableRow(new[] { "7", "5", "17", "3", "14", "2" });
            CreateOrUpdateTableRow(new[] { "8", "2", "3", "3", "7", "3" });
            CreateOrUpdateTableRow(new[] { "9", "2", "8", "3", "10", "3" });
            CreateOrUpdateTableRow(new[] { "10", "2", "9", "3", "11", "3" });
            CreateOrUpdateTableRow(new[] { "11", "2", "10", "3", "6", "3" });
            CreateOrUpdateTableRow(new[] { "12", "2", "20", "3", "98", "3" });
            CreateOrUpdateTableRow(new[] { "13", "2", "21", "3", "95", "3" });
            CreateOrUpdateTableRow(new[] { "14", "2", "22", "3", "96", "3" });
            CreateOrUpdateTableRow(new[] { "15", "2", "23", "3", "97", "3" });
            CreateOrUpdateTableRow(new[] { "16", "2", "24", "3", "94", "3" });
            CreateOrUpdateTableRow(new[] { "17", "2", "1", "2", "2", "1" });
            CreateOrUpdateTableRow(new[] { "18", "2", "5", "2", "2", "1" });
            CreateOrUpdateTableRow(new[] { "19", "2", "12", "2", "2", "1" });
            CreateOrUpdateTableRow(new[] { "20", "2", "19", "2", "2", "1" });
            CreateOrUpdateTableRow(new[] { "21", "6", "2", "3", "4", "2" });
            CreateOrUpdateTableRow(new[] { "22", "6", "6", "3", "12", "3" });
            CreateOrUpdateTableRow(new[] { "23", "6", "7", "3", "8", "2" });
            CreateOrUpdateTableRow(new[] { "24", "6", "13", "3", "12", "3" });
            CreateOrUpdateTableRow(new[] { "25", "6", "3", "3", "7", "3" });
            CreateOrUpdateTableRow(new[] { "26", "6", "8", "3", "10", "3" });
            CreateOrUpdateTableRow(new[] { "27", "6", "9", "3", "11", "3" });
            CreateOrUpdateTableRow(new[] { "28", "6", "10", "3", "6", "3" });
            CreateOrUpdateTableRow(new[] { "29", "6", "20", "3", "98", "3" });
            CreateOrUpdateTableRow(new[] { "30", "6", "21", "3", "95", "3" });
            CreateOrUpdateTableRow(new[] { "31", "6", "22", "3", "96", "3" });
            CreateOrUpdateTableRow(new[] { "32", "6", "23", "3", "97", "3" });
            CreateOrUpdateTableRow(new[] { "33", "6", "24", "3", "94", "3" });
            CreateOrUpdateTableRow(new[] { "34", "6", "1", "2", "2", "1" });
            CreateOrUpdateTableRow(new[] { "35", "6", "5", "2", "2", "1" });
            CreateOrUpdateTableRow(new[] { "36", "6", "12", "2", "2", "1" });
            CreateOrUpdateTableRow(new[] { "37", "6", "19", "2", "2", "1" });
            CreateOrUpdateTableRow(new[] { "38", "7", null, "1", "42", "2" });
            CreateOrUpdateTableRow(new[] { "39", "2", "2", "4", "43", "1" });
            CreateOrUpdateTableRow(new[] { "40", "2", "6", "4", "50", "1" });
            CreateOrUpdateTableRow(new[] { "41", "2", "7", "4", "46", "1" });
            CreateOrUpdateTableRow(new[] { "42", "2", "13", "4", "50", "1" });
            CreateOrUpdateTableRow(new[] { "43", "2", "3", "4", "45", "1" });
            CreateOrUpdateTableRow(new[] { "44", "2", "8", "4", "48", "1" });
            CreateOrUpdateTableRow(new[] { "45", "2", "9", "4", "49", "1" });
            CreateOrUpdateTableRow(new[] { "46", "2", "10", "4", "44", "1" });
            CreateOrUpdateTableRow(new[] { "47", "2", "20", "4", "50", "1" });
            CreateOrUpdateTableRow(new[] { "48", "2", "21", "4", "46", "1" });
            CreateOrUpdateTableRow(new[] { "49", "2", "22", "4", "48", "1" });
            CreateOrUpdateTableRow(new[] { "50", "2", "23", "4", "49", "1" });
            CreateOrUpdateTableRow(new[] { "51", "2", "24", "4", "44", "1" });
            CreateOrUpdateTableRow(new[] { "52", "6", "2", "4", "43", "1" });
            CreateOrUpdateTableRow(new[] { "53", "6", "6", "4", "50", "1" });
            CreateOrUpdateTableRow(new[] { "54", "6", "7", "4", "46", "1" });
            CreateOrUpdateTableRow(new[] { "55", "6", "13", "4", "50", "1" });
            CreateOrUpdateTableRow(new[] { "56", "6", "3", "4", "45", "1" });
            CreateOrUpdateTableRow(new[] { "57", "6", "8", "4", "48", "1" });
            CreateOrUpdateTableRow(new[] { "58", "6", "9", "4", "49", "1" });
            CreateOrUpdateTableRow(new[] { "59", "6", "10", "4", "44", "1" });
            CreateOrUpdateTableRow(new[] { "60", "6", "20", "4", "50", "1" });
            CreateOrUpdateTableRow(new[] { "61", "6", "21", "4", "46", "1" });
            CreateOrUpdateTableRow(new[] { "62", "6", "22", "4", "48", "1" });
            CreateOrUpdateTableRow(new[] { "63", "6", "23", "4", "49", "1" });
            CreateOrUpdateTableRow(new[] { "64", "6", "24", "4", "44", "1" });
            CreateOrUpdateTableRow(new[] { "65", "1", null, "2", "1", "1" });
            CreateOrUpdateTableRow(new[] { "66", "8", null, "2", "3", "1" });
            CreateOrUpdateTableRow(new[] { "67", "9", null, "2", "52", "1" });
            CreateOrUpdateTableRow(new[] { "68", "9", null, "5", "53", "1" });
            CreateOrUpdateTableRow(new[] { "69", "3", null, "5", "41", "1" });
            CreateOrUpdateTableRow(new[] { "70", "7", null, "5", "41", "1" });
            CreateOrUpdateTableRow(new[] { "71", "2", "2", "5", "58", "1" });
            CreateOrUpdateTableRow(new[] { "72", "2", "6", "5", "65", "1" });
            CreateOrUpdateTableRow(new[] { "73", "2", "7", "5", "61", "1" });
            CreateOrUpdateTableRow(new[] { "74", "2", "13", "5", "65", "1" });
            CreateOrUpdateTableRow(new[] { "75", "4", null, "5", "56", "1" });
            CreateOrUpdateTableRow(new[] { "76", "5", null, "5", "57", "1" });
            CreateOrUpdateTableRow(new[] { "77", "2", "3", "5", "60", "1" });
            CreateOrUpdateTableRow(new[] { "78", "2", "8", "5", "63", "1" });
            CreateOrUpdateTableRow(new[] { "79", "2", "9", "5", "64", "1" });
            CreateOrUpdateTableRow(new[] { "80", "2", "10", "5", "59", "1" });
            CreateOrUpdateTableRow(new[] { "81", "2", "20", "5", "65", "1" });
            CreateOrUpdateTableRow(new[] { "82", "2", "21", "5", "61", "1" });
            CreateOrUpdateTableRow(new[] { "83", "2", "22", "5", "63", "1" });
            CreateOrUpdateTableRow(new[] { "84", "2", "23", "5", "64", "1" });
            CreateOrUpdateTableRow(new[] { "85", "2", "24", "5", "59", "1" });
            CreateOrUpdateTableRow(new[] { "86", "6", "2", "5", "58", "1" });
            CreateOrUpdateTableRow(new[] { "87", "6", "6", "5", "65", "1" });
            CreateOrUpdateTableRow(new[] { "88", "6", "7", "5", "61", "1" });
            CreateOrUpdateTableRow(new[] { "89", "6", "13", "5", "65", "1" });
            CreateOrUpdateTableRow(new[] { "90", "6", "3", "5", "60", "1" });
            CreateOrUpdateTableRow(new[] { "91", "6", "8", "5", "63", "1" });
            CreateOrUpdateTableRow(new[] { "92", "6", "9", "5", "64", "1" });
            CreateOrUpdateTableRow(new[] { "93", "6", "10", "5", "59", "1" });
            CreateOrUpdateTableRow(new[] { "94", "6", "20", "5", "65", "1" });
            CreateOrUpdateTableRow(new[] { "95", "6", "21", "5", "61", "1" });
            CreateOrUpdateTableRow(new[] { "96", "6", "22", "5", "63", "1" });
            CreateOrUpdateTableRow(new[] { "97", "6", "23", "5", "64", "1" });
            CreateOrUpdateTableRow(new[] { "98", "6", "24", "5", "59", "1" });
            CreateOrUpdateTableRow(new[] { "99", "10", "26", "3", "12", "3" });
            CreateOrUpdateTableRow(new[] { "100", "10", "27", "3", "8", "3" });
            CreateOrUpdateTableRow(new[] { "101", "11", "29", "3", "13", "3" });
            CreateOrUpdateTableRow(new[] { "102", "11", null, "5", "56", "1" });
            CreateOrUpdateTableRow(new[] { "103", "12", "30", "3", "14", "3" });
            CreateOrUpdateTableRow(new[] { "104", "12", null, "5", "57", "1" });
            CreateOrUpdateTableRow(new[] { "105", "10", "28", "2", "2", "1" });
            CreateOrUpdateTableRow(new[] { "106", "10", "25", "2", "2", "1" });
            CreateOrUpdateTableRow(new[] { "107", "10", "26", "4", "50", "1" });
            CreateOrUpdateTableRow(new[] { "108", "10", "27", "4", "46", "1" });
            CreateOrUpdateTableRow(new[] { "109", "10", "26", "5", "65", "1" });
            CreateOrUpdateTableRow(new[] { "110", "10", "27", "5", "61", "1" });
            CreateOrUpdateTableRow(new[] { "111", "13", "14", "3", "42", "2" });
            CreateOrUpdateTableRow(new[] { "112", "13", "14", "5", "41", "1" });
            CreateOrUpdateTableRow(new[] { "116", "14", "31", "5", "79", "1" });
            CreateOrUpdateTableRow(new[] { "117", "15", "32", "5", "81", "1" });
            //CreateOrUpdateTableRow(new[] { "118", "16", "33", "1", "85", "1" });
            //CreateOrUpdateTableRow(new[] { "119", "16", "33", "1", "86", "1" });
            //CreateOrUpdateTableRow(new[] { "120", "16", "34", "1", "87", "1" });
            //CreateOrUpdateTableRow(new[] { "121", "16", "34", "1", "87", "1" });
            //CreateOrUpdateTableRow(new[] { "122", "16", "35", "1", "88", "1" });
            //CreateOrUpdateTableRow(new[] { "123", "16", "35", "1", "88", "1" });
            CreateOrUpdateTableRow(new[] { "124", "14", "31", "3", "91", "3" });
            CreateOrUpdateTableRow(new[] { "125", "15", "32", "3", "92", "3" });

            CreateOrUpdateTableRow(new[] { "126", "2", "36", "3", "94", "3" });
            CreateOrUpdateTableRow(new[] { "127", "2", "37", "3", "94", "3" });
            CreateOrUpdateTableRow(new[] { "128", "6", "36", "3", "94", "3" });
            CreateOrUpdateTableRow(new[] { "129", "6", "37", "3", "94", "3" });
            CreateOrUpdateTableRow(new[] { "130", "2", "36", "4", "44", "1" });
            CreateOrUpdateTableRow(new[] { "131", "2", "37", "4", "44", "1" });
            CreateOrUpdateTableRow(new[] { "132", "6", "36", "4", "44", "1" });
            CreateOrUpdateTableRow(new[] { "133", "6", "37", "4", "44", "1" });
            CreateOrUpdateTableRow(new[] { "134", "2", "36", "5", "59", "1" });
            CreateOrUpdateTableRow(new[] { "135", "2", "37", "5", "59", "1" });
            CreateOrUpdateTableRow(new[] { "136", "6", "36", "5", "59", "1" });
            CreateOrUpdateTableRow(new[] { "137", "6", "37", "5", "59", "1" });
            CreateOrUpdateTableRow(new[] { "138", "17", "38", "3", "99", "2" });
            CreateOrUpdateTableRow(new[] { "139", "17", "39", "3", "100", "2" });
            CreateOrUpdateTableRow(new[] { "140", "17", "40", "3", "101", "2" });
        }

        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow("118");
            DeleteTableRow("119");
            DeleteTableRow("120");
            DeleteTableRow("121");
            DeleteTableRow("122");
            DeleteTableRow("123");
        }
    }
}