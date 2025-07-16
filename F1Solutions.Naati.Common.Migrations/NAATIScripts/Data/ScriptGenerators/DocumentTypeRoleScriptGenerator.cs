using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class DocumentTypeRoleScriptGenerator : BaseScriptGenerator
    {
        public DocumentTypeRoleScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblDocumentTypeRole";

        public override IList<string> Columns => new[]
        {
            "DocumentTypeRoleId",
            "DocumentTypeId",
            "RoleId",
            "Upload",
            "Download"
        };

        public override void RunScripts()
        {
            ScriptRunner.RunScript("DELETE FROM tblDocumentTypeRole");

            CreateOrUpdateTableRow(new[] { "1", "25", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "2", "26", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "3", "21", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "4", "13", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "5", "20", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "6", "18", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "7", "23", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "8", "14", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "9", "15", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "10", "9", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "11", "16", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "12", "17", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "13", "28", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "14", "32", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "15", "24", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "16", "12", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "17", "11", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "18", "41", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "19", "27", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "20", "10", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "21", "19", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "22", "22", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "23", "8", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "24", "29", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "25", "46", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "26", "47", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "27", "8", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "28", "38", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "29", "40", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "30", "30", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "31", "31", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "32", "35", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "33", "43", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "34", "45", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "35", "44", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "36", "42", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "37", "33", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "38", "34", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "39", "4", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "40", "7", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "41", "3", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "42", "37", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "43", "36", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "44", "6", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "45", "5", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "46", "1", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "47", "2", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "48", "25", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "49", "26", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "50", "21", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "51", "13", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "52", "20", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "53", "18", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "54", "23", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "55", "14", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "56", "15", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "57", "9", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "58", "16", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "59", "17", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "60", "28", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "61", "32", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "62", "24", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "63", "12", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "64", "11", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "65", "41", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "66", "27", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "67", "10", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "68", "19", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "69", "22", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "70", "8", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "71", "29", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "72", "46", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "73", "47", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "74", "8", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "75", "4", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "76", "7", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "77", "3", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "78", "37", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "79", "36", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "80", "6", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "81", "5", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "82", "1", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "83", "2", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "84", "8", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "85", "25", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "86", "26", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "87", "21", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "88", "13", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "89", "20", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "90", "18", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "91", "23", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "92", "14", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "93", "15", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "94", "9", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "95", "16", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "96", "17", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "97", "28", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "98", "32", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "99", "24", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "100", "12", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "101", "11", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "102", "41", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "103", "27", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "104", "10", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "105", "46", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "106", "47", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "107", "4", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "108", "7", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "109", "3", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "110", "37", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "111", "36", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "112", "6", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "113", "5", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "114", "1", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "115", "2", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "116", "40", "5", "1", "1" });
            CreateOrUpdateTableRow(new[] { "117", "30", "5", "1", "1" });
            CreateOrUpdateTableRow(new[] { "118", "31", "5", "1", "1" });
            CreateOrUpdateTableRow(new[] { "119", "35", "5", "1", "1" });
            CreateOrUpdateTableRow(new[] { "120", "43", "5", "1", "1" });
            CreateOrUpdateTableRow(new[] { "121", "45", "5", "1", "1" });
            CreateOrUpdateTableRow(new[] { "122", "44", "5", "1", "1" });
            CreateOrUpdateTableRow(new[] { "123", "42", "5", "1", "1" });
            CreateOrUpdateTableRow(new[] { "124", "25", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "125", "26", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "126", "21", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "127", "13", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "128", "20", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "129", "18", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "130", "23", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "131", "14", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "132", "15", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "133", "9", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "134", "16", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "135", "17", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "136", "28", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "137", "32", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "138", "24", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "139", "12", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "140", "11", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "141", "41", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "142", "27", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "143", "10", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "144", "8", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "145", "40", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "146", "30", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "147", "31", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "148", "35", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "149", "4", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "150", "7", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "151", "3", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "152", "37", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "153", "36", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "154", "6", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "155", "5", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "156", "1", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "157", "2", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "158", "25", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "159", "26", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "160", "21", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "161", "13", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "162", "20", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "163", "18", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "164", "23", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "165", "14", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "166", "15", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "167", "9", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "168", "16", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "169", "17", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "170", "28", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "171", "32", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "172", "24", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "173", "12", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "174", "11", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "175", "41", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "176", "27", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "177", "10", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "178", "8", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "179", "4", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "180", "7", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "181", "3", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "182", "37", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "183", "36", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "184", "6", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "185", "5", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "186", "1", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "187", "2", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "188", "25", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "189", "26", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "190", "21", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "191", "13", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "192", "20", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "193", "18", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "194", "23", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "195", "14", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "196", "15", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "197", "9", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "198", "16", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "199", "17", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "200", "28", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "201", "32", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "202", "24", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "203", "12", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "204", "11", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "205", "41", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "206", "27", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "207", "10", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "208", "8", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "209", "38", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "210", "7", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "211", "1", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "212", "25", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "213", "26", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "214", "21", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "215", "13", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "216", "20", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "217", "18", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "218", "23", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "219", "14", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "220", "15", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "221", "9", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "222", "16", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "223", "17", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "224", "28", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "225", "32", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "226", "24", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "227", "12", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "228", "11", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "229", "41", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "230", "27", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "231", "10", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "232", "8", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "233", "40", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "234", "30", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "235", "31", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "236", "35", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "237", "43", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "238", "45", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "239", "44", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "240", "42", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "241", "33", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "242", "34", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "243", "4", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "244", "7", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "245", "3", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "246", "37", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "247", "36", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "248", "6", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "249", "5", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "250", "1", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "251", "2", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "252", "4", "11", "1", "1" });
            CreateOrUpdateTableRow(new[] { "253", "7", "11", "1", "1" });
            CreateOrUpdateTableRow(new[] { "254", "3", "11", "1", "1" });
            CreateOrUpdateTableRow(new[] { "255", "37", "11", "1", "1" });
            CreateOrUpdateTableRow(new[] { "256", "36", "11", "1", "1" });
            CreateOrUpdateTableRow(new[] { "257", "6", "11", "1", "1" });
            CreateOrUpdateTableRow(new[] { "258", "5", "11", "1", "1" });
            CreateOrUpdateTableRow(new[] { "259", "1", "11", "1", "1" });
            CreateOrUpdateTableRow(new[] { "260", "2", "11", "1", "1" });
            CreateOrUpdateTableRow(new[] { "261", "46", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "262", "46", "5", "1", "1" });
            CreateOrUpdateTableRow(new[] { "263", "47", "5", "1", "1" });
            CreateOrUpdateTableRow(new[] { "264", "47", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "265", "39", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "266", "39", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "267", "39", "4", "1", "1" });
            CreateOrUpdateTableRow(new[] { "268", "39", "6", "1", "1" });
            CreateOrUpdateTableRow(new[] { "269", "39", "7", "1", "1" });
            CreateOrUpdateTableRow(new[] { "270", "39", "9", "1", "1" });
            CreateOrUpdateTableRow(new[] { "271", "39", "10", "1", "1" });
            CreateOrUpdateTableRow(new[] { "272", "30", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "273", "31", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "274", "35", "3", "1", "1" });
            CreateOrUpdateTableRow(new[] { "275", "40", "3", "1", "1" });
            //// Finance can upload NAATI Employment
            //CreateOrUpdateTableRow(new[] { "1", "38", "9", "1", "1" });

            ////------------------- BEGIN: TEST MATERIALS ----------------------
            //// Testing Operations, and Testing Operations Admin roles are allowed to updload test materials

            //CreateOrUpdateTableRow(new[] { "2", "30", "3", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "3", "31", "3", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "4", "35", "3", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "5", "40", "3", "1", "1" });

            //CreateOrUpdateTableRow(new[] { "6", "30", "4", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "7", "31", "4", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "8", "35", "4", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "9", "40", "4", "1", "1" });

            //// Regional Managers are allowed to download test materials
            //CreateOrUpdateTableRow(new[] { "10", "30", "6", "0", "1" });
            //CreateOrUpdateTableRow(new[] { "11", "31", "6", "0", "1" });
            //CreateOrUpdateTableRow(new[] { "12", "35", "6", "0", "1" });
            //CreateOrUpdateTableRow(new[] { "13", "40", "6", "0", "1" });
            ////------------------- END: TEST MATERIALS ----------------------

            ////------------------- BEGIN: MATERIAL REQUESTS ----------------------
            //// Senior Manager, Testing Operations, Testing Operations Admin
            //CreateOrUpdateTableRow(new[] { "14", "42", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "15", "43", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "16", "44", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "17", "45", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "18", "46", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "19", "47", "1", "1", "1" });

            //CreateOrUpdateTableRow(new[] { "20", "42", "3", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "21", "43", "3", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "22", "44", "3", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "23", "45", "3", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "24", "46", "3", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "25", "47", "3", "1", "1" });

            //CreateOrUpdateTableRow(new[] { "26", "42", "4", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "27", "43", "4", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "28", "44", "4", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "29", "45", "4", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "30", "46", "4", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "31", "47", "4", "1", "1" });

            ////------------------- END:  MATERIAL REQUESTS ----------------------

            //// Senior Manager can upload/download everything
            //CreateOrUpdateTableRow(new[] { "32", "1", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "33", "2", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "34", "3", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "35", "4", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "36", "5", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "37", "6", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "38", "7", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "39", "8", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "40", "9", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "41", "10", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "42", "11", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "43", "12", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "44", "13", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "45", "14", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "46", "15", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "47", "16", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "48", "17", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "49", "18", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "50", "19", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "51", "20", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "52", "21", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "53", "22", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "54", "23", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "55", "24", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "56", "25", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "57", "26", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "58", "27", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "59", "28", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "60", "29", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "61", "32", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "62", "33", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "63", "34", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "64", "36", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "65", "37", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "66", "38", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "67", "39", "1", "1", "1" });
            //CreateOrUpdateTableRow(new[] { "68", "41", "1", "1", "1" });
        }
    }
}
