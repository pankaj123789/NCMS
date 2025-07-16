using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialTypeTestMaterialDomainScriptGenerator : BaseScriptGenerator
    {
        public CredentialTypeTestMaterialDomainScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialTypeTestMaterialDomain";
        public override IList<string> Columns => new[] {
            "CredentialTypeTestMaterialDomainId",
            "CredentialTypeId",
            "TestMaterialDomainId"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "2", "41" });
            CreateOrUpdateTableRow(new[] { "2", "6", "41" });
            CreateOrUpdateTableRow(new[] { "3", "7", "41" });
            CreateOrUpdateTableRow(new[] { "4", "13", "41" });
            CreateOrUpdateTableRow(new[] { "5", "14", "41" });
            CreateOrUpdateTableRow(new[] { "6", "20", "41" });
            CreateOrUpdateTableRow(new[] { "7", "21", "41" });

            CreateOrUpdateTableRow(new[] { "8", "2", "2" });
            CreateOrUpdateTableRow(new[] { "9", "6", "2" });
            CreateOrUpdateTableRow(new[] { "10", "7", "2" });
            CreateOrUpdateTableRow(new[] { "11", "13", "2" });
            CreateOrUpdateTableRow(new[] { "12", "14", "2" });
            CreateOrUpdateTableRow(new[] { "13", "20", "2" });
            CreateOrUpdateTableRow(new[] { "14", "21", "2" });

            CreateOrUpdateTableRow(new[] { "15", "2", "3" });
            CreateOrUpdateTableRow(new[] { "16", "6", "3" });
            CreateOrUpdateTableRow(new[] { "17", "7", "3" });
            CreateOrUpdateTableRow(new[] { "18", "13", "3" });
            CreateOrUpdateTableRow(new[] { "19", "14", "3" });
            CreateOrUpdateTableRow(new[] { "20", "20", "3" });
            CreateOrUpdateTableRow(new[] { "21", "21", "3" });

            CreateOrUpdateTableRow(new[] { "22", "2", "4" });
            CreateOrUpdateTableRow(new[] { "23", "6", "4" });
            CreateOrUpdateTableRow(new[] { "24", "7", "4" });
            CreateOrUpdateTableRow(new[] { "25", "13", "4" });
            CreateOrUpdateTableRow(new[] { "26", "14", "4" });
            CreateOrUpdateTableRow(new[] { "27", "20", "4" });
            CreateOrUpdateTableRow(new[] { "28", "21", "4" });

            CreateOrUpdateTableRow(new[] { "29", "2", "5" });
            CreateOrUpdateTableRow(new[] { "30", "6", "5" });
            CreateOrUpdateTableRow(new[] { "31", "7", "5" });
            CreateOrUpdateTableRow(new[] { "32", "13", "5" });
            CreateOrUpdateTableRow(new[] { "33", "14", "5" });
            CreateOrUpdateTableRow(new[] { "34", "20", "5" });
            CreateOrUpdateTableRow(new[] { "35", "21", "5" });

            CreateOrUpdateTableRow(new[] { "36", "2", "6" });
            CreateOrUpdateTableRow(new[] { "37", "6", "6" });
            CreateOrUpdateTableRow(new[] { "38", "7", "6" });
            CreateOrUpdateTableRow(new[] { "39", "13", "6" });
            CreateOrUpdateTableRow(new[] { "40", "14", "6" });
            CreateOrUpdateTableRow(new[] { "41", "20", "6" });
            CreateOrUpdateTableRow(new[] { "42", "21", "6" });

            CreateOrUpdateTableRow(new[] { "43", "2", "7" });
            CreateOrUpdateTableRow(new[] { "44", "6", "7" });
            CreateOrUpdateTableRow(new[] { "45", "7", "7" });
            CreateOrUpdateTableRow(new[] { "46", "8", "7" });
            CreateOrUpdateTableRow(new[] { "47", "13", "7" });
            CreateOrUpdateTableRow(new[] { "48", "14", "7" });
            CreateOrUpdateTableRow(new[] { "49", "20", "7" });
            CreateOrUpdateTableRow(new[] { "50", "21", "7" });

            CreateOrUpdateTableRow(new[] { "51", "2", "8" });
            CreateOrUpdateTableRow(new[] { "52", "6", "8" });
            CreateOrUpdateTableRow(new[] { "53", "7", "8" });
            CreateOrUpdateTableRow(new[] { "54", "13", "8" });
            CreateOrUpdateTableRow(new[] { "55", "14", "8" });
            CreateOrUpdateTableRow(new[] { "56", "20", "8" });
            CreateOrUpdateTableRow(new[] { "57", "21", "8" });


            CreateOrUpdateTableRow(new[] { "58", "2", "9" });
            CreateOrUpdateTableRow(new[] { "59", "6", "9" });
            CreateOrUpdateTableRow(new[] { "60", "7", "9" });
            CreateOrUpdateTableRow(new[] { "61", "14", "9" });

            CreateOrUpdateTableRow(new[] { "62", "2", "10" });
            CreateOrUpdateTableRow(new[] { "63", "6", "10" });
            CreateOrUpdateTableRow(new[] { "64", "7", "10" });
            CreateOrUpdateTableRow(new[] { "65", "13", "10" });
            CreateOrUpdateTableRow(new[] { "66", "14", "10" });
            CreateOrUpdateTableRow(new[] { "67", "20", "10" });
            CreateOrUpdateTableRow(new[] { "68", "21", "10" });

            CreateOrUpdateTableRow(new[] { "69", "2", "11" });
            CreateOrUpdateTableRow(new[] { "70", "6", "11" });
            CreateOrUpdateTableRow(new[] { "71", "7", "11" });
            CreateOrUpdateTableRow(new[] { "72", "9", "11" });
            CreateOrUpdateTableRow(new[] { "73", "13", "11" });
            CreateOrUpdateTableRow(new[] { "74", "14", "11" });
            CreateOrUpdateTableRow(new[] { "75", "20", "11" });
            CreateOrUpdateTableRow(new[] { "76", "21", "11" });

            CreateOrUpdateTableRow(new[] { "77", "2", "12" });
            CreateOrUpdateTableRow(new[] { "78", "6", "12" });
            CreateOrUpdateTableRow(new[] { "79", "7", "12" });
            CreateOrUpdateTableRow(new[] { "80", "13", "12" });
            CreateOrUpdateTableRow(new[] { "81", "14", "12" });
            CreateOrUpdateTableRow(new[] { "82", "20", "12" });
            CreateOrUpdateTableRow(new[] { "83", "21", "12" });

            CreateOrUpdateTableRow(new[] { "84", "16", "13" });
            CreateOrUpdateTableRow(new[] { "85", "16", "14" });
            CreateOrUpdateTableRow(new[] { "86", "16", "15" });

            CreateOrUpdateTableRow(new[] { "87", "16", "16" });
            CreateOrUpdateTableRow(new[] { "88", "16", "17" });
            CreateOrUpdateTableRow(new[] { "89", "16", "18" });
            CreateOrUpdateTableRow(new[] { "90", "16", "19" });
            CreateOrUpdateTableRow(new[] { "91", "16", "20" });
            CreateOrUpdateTableRow(new[] { "92", "16", "21" });
            CreateOrUpdateTableRow(new[] { "93", "16", "22" });
            CreateOrUpdateTableRow(new[] { "94", "16", "23" });

            CreateOrUpdateTableRow(new[] { "95", "16", "24" });
            CreateOrUpdateTableRow(new[] { "96", "16", "25" });
            CreateOrUpdateTableRow(new[] { "97", "16", "26" });
            CreateOrUpdateTableRow(new[] { "98", "16", "27" });
            CreateOrUpdateTableRow(new[] { "99", "16", "28" });
            CreateOrUpdateTableRow(new[] { "100", "16", "29" });
            CreateOrUpdateTableRow(new[] { "101", "16", "30" });

            CreateOrUpdateTableRow(new[] { "102", "9", "31" });
            CreateOrUpdateTableRow(new[] { "103", "10", "31" });
            CreateOrUpdateTableRow(new[] { "104", "16", "31" });
            CreateOrUpdateTableRow(new[] { "105", "9", "32" });
            CreateOrUpdateTableRow(new[] { "106", "10", "32" });
            CreateOrUpdateTableRow(new[] { "107", "16", "32" });
            CreateOrUpdateTableRow(new[] { "108", "9", "33" });
            CreateOrUpdateTableRow(new[] { "109", "10", "33" });
            CreateOrUpdateTableRow(new[] { "110", "16", "33" });
            CreateOrUpdateTableRow(new[] { "111", "9", "34" });
            CreateOrUpdateTableRow(new[] { "112", "10", "34" });
            CreateOrUpdateTableRow(new[] { "113", "16", "34" });
            CreateOrUpdateTableRow(new[] { "114", "9", "35" });
            CreateOrUpdateTableRow(new[] { "115", "10", "35" });
            CreateOrUpdateTableRow(new[] { "116", "16", "35" });
            CreateOrUpdateTableRow(new[] { "117", "9", "36" });
            CreateOrUpdateTableRow(new[] { "118", "10", "36" });
            CreateOrUpdateTableRow(new[] { "119", "16", "36" });
            CreateOrUpdateTableRow(new[] { "120", "9", "37" });
            CreateOrUpdateTableRow(new[] { "121", "10", "37" });
            CreateOrUpdateTableRow(new[] { "122", "16", "37" });
            CreateOrUpdateTableRow(new[] { "123", "9", "38" });
            CreateOrUpdateTableRow(new[] { "124", "10", "38" });
            CreateOrUpdateTableRow(new[] { "125", "16", "38" });
            CreateOrUpdateTableRow(new[] { "126", "9", "39" });
            CreateOrUpdateTableRow(new[] { "127", "10", "39" });
            CreateOrUpdateTableRow(new[] { "128", "16", "39" });
            CreateOrUpdateTableRow(new[] { "129", "9", "40" });
            CreateOrUpdateTableRow(new[] { "130", "10", "40" });
            CreateOrUpdateTableRow(new[] { "131", "16", "40" });

            CreateOrUpdateTableRow(new[] { "132", "2", "42" });
            CreateOrUpdateTableRow(new[] { "133", "7", "42" });
            CreateOrUpdateTableRow(new[] { "134", "21", "42" });
            CreateOrUpdateTableRow(new[] { "135", "2", "43" });
            CreateOrUpdateTableRow(new[] { "136", "7", "43" });
            CreateOrUpdateTableRow(new[] { "137", "21", "43" });
            CreateOrUpdateTableRow(new[] { "138", "2", "44" });
            CreateOrUpdateTableRow(new[] { "139", "7", "44" });
            CreateOrUpdateTableRow(new[] { "140", "21", "44" });
            CreateOrUpdateTableRow(new[] { "141", "2", "45" });
            CreateOrUpdateTableRow(new[] { "142", "7", "45" });
            CreateOrUpdateTableRow(new[] { "143", "21", "45" });
            CreateOrUpdateTableRow(new[] { "144", "2", "46" });
            CreateOrUpdateTableRow(new[] { "145", "7", "46" });
            CreateOrUpdateTableRow(new[] { "146", "21", "46" });

            CreateOrUpdateTableRow(new[] { "147", "2", "47" });
            CreateOrUpdateTableRow(new[] { "148", "7", "47" });
            CreateOrUpdateTableRow(new[] { "149", "10", "47" });
            CreateOrUpdateTableRow(new[] { "150", "21", "47" });
            CreateOrUpdateTableRow(new[] { "151", "2", "48" });
            CreateOrUpdateTableRow(new[] { "152", "7", "48" });
            CreateOrUpdateTableRow(new[] { "153", "10", "48" });
            CreateOrUpdateTableRow(new[] { "154", "21", "48" });

            CreateOrUpdateTableRow(new[] { "155", "2", "49" });
            CreateOrUpdateTableRow(new[] { "156", "10", "50" });
            CreateOrUpdateTableRow(new[] { "157", "10", "51" });
            CreateOrUpdateTableRow(new[] { "158", "10", "52" });
            CreateOrUpdateTableRow(new[] { "159", "2", "53" });
            CreateOrUpdateTableRow(new[] { "160", "10", "53" });

            CreateOrUpdateTableRow(new[] { "161", "17", "54" });

            CreateOrUpdateTableRow(new[] { "176", "9", "69" });
            CreateOrUpdateTableRow(new[] { "177", "9", "70" });
            CreateOrUpdateTableRow(new[] { "178", "9", "71" });
            CreateOrUpdateTableRow(new[] { "179", "9", "72" });
            CreateOrUpdateTableRow(new[] { "180", "9", "73" });
            CreateOrUpdateTableRow(new[] { "181", "9", "74" });
            CreateOrUpdateTableRow(new[] { "183", "9", "76" });
            CreateOrUpdateTableRow(new[] { "184", "9", "77" });
            CreateOrUpdateTableRow(new[] { "185", "9", "78" });
            CreateOrUpdateTableRow(new[] { "186", "9", "79" });
            CreateOrUpdateTableRow(new[] { "187", "9", "80" });
            CreateOrUpdateTableRow(new[] { "188", "9", "81" });
            CreateOrUpdateTableRow(new[] { "189", "9", "82" });
            CreateOrUpdateTableRow(new[] { "190", "9", "83" });
            CreateOrUpdateTableRow(new[] { "191", "9", "84" });
            CreateOrUpdateTableRow(new[] { "192", "9", "85" });

            CreateOrUpdateTableRow(new[] { "193", "10", "86" });
            CreateOrUpdateTableRow(new[] { "194", "10", "87" });
            CreateOrUpdateTableRow(new[] { "195", "10", "88" });
            CreateOrUpdateTableRow(new[] { "196", "10", "89" });
            CreateOrUpdateTableRow(new[] { "197", "10", "90" });

            CreateOrUpdateTableRow(new[] { "198", "9", "91" });
            CreateOrUpdateTableRow(new[] { "199", "9", "92" });

            CreateOrUpdateTableRow(new[] { "200", "15", "93" });
            CreateOrUpdateTableRow(new[] { "201", "31", "11" });
            CreateOrUpdateTableRow(new[] { "202", "32", "7" });

            CreateOrUpdateTableRow(new[] { "203", "26", "41" });
            CreateOrUpdateTableRow(new[] { "204", "26", "2" });
            CreateOrUpdateTableRow(new[] { "205", "26", "3" });
            CreateOrUpdateTableRow(new[] { "206", "26", "4" });
            CreateOrUpdateTableRow(new[] { "207", "26", "5" });
            CreateOrUpdateTableRow(new[] { "208", "26", "6" });
            CreateOrUpdateTableRow(new[] { "209", "26", "7" });
            CreateOrUpdateTableRow(new[] { "210", "26", "8" });
            CreateOrUpdateTableRow(new[] { "211", "26", "9" });
            CreateOrUpdateTableRow(new[] { "212", "26", "10" });
            CreateOrUpdateTableRow(new[] { "213", "27", "41" });
            CreateOrUpdateTableRow(new[] { "214", "27", "2" });
            CreateOrUpdateTableRow(new[] { "215", "27", "3" });
            CreateOrUpdateTableRow(new[] { "216", "27", "4" });
            CreateOrUpdateTableRow(new[] { "217", "27", "5" });
            CreateOrUpdateTableRow(new[] { "218", "27", "6" });
            CreateOrUpdateTableRow(new[] { "219", "27", "7" });
            CreateOrUpdateTableRow(new[] { "220", "27", "8" });
            CreateOrUpdateTableRow(new[] { "221", "27", "9" });
            CreateOrUpdateTableRow(new[] { "222", "27", "10" });
            CreateOrUpdateTableRow(new[] { "223", "27", "42" });
            CreateOrUpdateTableRow(new[] { "224", "27", "43" });
            CreateOrUpdateTableRow(new[] { "225", "27", "44" });
            CreateOrUpdateTableRow(new[] { "226", "27", "45" });
            CreateOrUpdateTableRow(new[] { "227", "27", "46" });
            CreateOrUpdateTableRow(new[] { "228", "27", "47" });
            CreateOrUpdateTableRow(new[] { "229", "27", "48" });
            CreateOrUpdateTableRow(new[] { "230", "26", "11" });
            CreateOrUpdateTableRow(new[] { "231", "26", "12" });
            CreateOrUpdateTableRow(new[] { "232", "27", "11" });
            CreateOrUpdateTableRow(new[] { "233", "27", "12" });

            //Unscpecified domain
            CreateOrUpdateTableRow(new[] { "234", "1", "1" });
            CreateOrUpdateTableRow(new[] { "235", "2", "1" });
            CreateOrUpdateTableRow(new[] { "236", "3", "1" });
            CreateOrUpdateTableRow(new[] { "237", "4", "1" });
            CreateOrUpdateTableRow(new[] { "238", "5", "1" });
            CreateOrUpdateTableRow(new[] { "239", "6", "1" });
            CreateOrUpdateTableRow(new[] { "240", "7", "1" });
            CreateOrUpdateTableRow(new[] { "241", "8", "1" });
            CreateOrUpdateTableRow(new[] { "242", "9", "1" });
            CreateOrUpdateTableRow(new[] { "243", "10", "1" });
            CreateOrUpdateTableRow(new[] { "244", "11", "1" });
            CreateOrUpdateTableRow(new[] { "245", "12", "1" });
            CreateOrUpdateTableRow(new[] { "246", "13", "1" });
            CreateOrUpdateTableRow(new[] { "247", "14", "1" });
            CreateOrUpdateTableRow(new[] { "248", "15", "1" });
            CreateOrUpdateTableRow(new[] { "249", "16", "1" });
            CreateOrUpdateTableRow(new[] { "250", "17", "1" });
            CreateOrUpdateTableRow(new[] { "251", "18", "1" });
            CreateOrUpdateTableRow(new[] { "252", "19", "1" });
            CreateOrUpdateTableRow(new[] { "253", "20", "1" });
            CreateOrUpdateTableRow(new[] { "254", "21", "1" });
            CreateOrUpdateTableRow(new[] { "255", "22", "1" });
            CreateOrUpdateTableRow(new[] { "256", "23", "1" });
            CreateOrUpdateTableRow(new[] { "257", "24", "1" });
            CreateOrUpdateTableRow(new[] { "258", "25", "1" });
            CreateOrUpdateTableRow(new[] { "259", "26", "1" });
            CreateOrUpdateTableRow(new[] { "260", "27", "1" });
            CreateOrUpdateTableRow(new[] { "261", "28", "1" });
            CreateOrUpdateTableRow(new[] { "262", "29", "1" });
            CreateOrUpdateTableRow(new[] { "263", "30", "1" });
            CreateOrUpdateTableRow(new[] { "264", "31", "1" });
            CreateOrUpdateTableRow(new[] { "265", "32", "1" });
            CreateOrUpdateTableRow(new[] { "266", "33", "1" });
            CreateOrUpdateTableRow(new[] { "267", "34", "1" });
            CreateOrUpdateTableRow(new[] { "268", "35", "1" });

            //180745
            CreateOrUpdateTableRow(new[] { "269", "36", "1" });
            CreateOrUpdateTableRow(new[] { "270", "36", "31" });
            CreateOrUpdateTableRow(new[] { "271", "36", "32" });
            CreateOrUpdateTableRow(new[] { "272", "36", "33" });
            CreateOrUpdateTableRow(new[] { "273", "36", "34" });
            CreateOrUpdateTableRow(new[] { "274", "36", "35" });
            CreateOrUpdateTableRow(new[] { "275", "36", "36" });
            CreateOrUpdateTableRow(new[] { "276", "36", "37" });
            CreateOrUpdateTableRow(new[] { "277", "36", "38" });
            CreateOrUpdateTableRow(new[] { "278", "36", "39" });
            CreateOrUpdateTableRow(new[] { "279", "36", "40" });
            CreateOrUpdateTableRow(new[] { "280", "36", "47" });
            CreateOrUpdateTableRow(new[] { "281", "36", "48" });
            CreateOrUpdateTableRow(new[] { "282", "36", "50" });
            CreateOrUpdateTableRow(new[] { "283", "36", "51" });
            CreateOrUpdateTableRow(new[] { "284", "36", "52" });
            CreateOrUpdateTableRow(new[] { "285", "36", "53" });
            CreateOrUpdateTableRow(new[] { "286", "36", "86" });
            CreateOrUpdateTableRow(new[] { "287", "36", "87" });
            CreateOrUpdateTableRow(new[] { "288", "36", "88" });
            CreateOrUpdateTableRow(new[] { "289", "36", "89" });
            CreateOrUpdateTableRow(new[] { "290", "36", "90" });

            CreateOrUpdateTableRow(new[] { "291", "37", "1" });
            CreateOrUpdateTableRow(new[] { "292", "37", "31" });
            CreateOrUpdateTableRow(new[] { "293", "37", "32" });
            CreateOrUpdateTableRow(new[] { "294", "37", "33" });
            CreateOrUpdateTableRow(new[] { "295", "37", "34" });
            CreateOrUpdateTableRow(new[] { "296", "37", "35" });
            CreateOrUpdateTableRow(new[] { "297", "37", "36" });
            CreateOrUpdateTableRow(new[] { "298", "37", "37" });
            CreateOrUpdateTableRow(new[] { "299", "37", "38" });
            CreateOrUpdateTableRow(new[] { "300", "37", "39" });
            CreateOrUpdateTableRow(new[] { "301", "37", "40" });
            CreateOrUpdateTableRow(new[] { "302", "37", "47" });
            CreateOrUpdateTableRow(new[] { "303", "37", "48" });
            CreateOrUpdateTableRow(new[] { "304", "37", "50" });
            CreateOrUpdateTableRow(new[] { "305", "37", "51" });
            CreateOrUpdateTableRow(new[] { "306", "37", "52" });
            CreateOrUpdateTableRow(new[] { "307", "37", "53" });
            CreateOrUpdateTableRow(new[] { "308", "37", "86" });
            CreateOrUpdateTableRow(new[] { "309", "37", "87" });
            CreateOrUpdateTableRow(new[] { "310", "37", "88" });
            CreateOrUpdateTableRow(new[] { "311", "37", "89" });
            CreateOrUpdateTableRow(new[] { "312", "37", "90" });

            //#180936
            CreateOrUpdateTableRow(new[] { "313", "36", "94" });
            CreateOrUpdateTableRow(new[] { "314", "36", "95" });
            CreateOrUpdateTableRow(new[] { "315", "36", "96" });
            CreateOrUpdateTableRow(new[] { "316", "36", "97" });
            CreateOrUpdateTableRow(new[] { "317", "36", "98" });
            CreateOrUpdateTableRow(new[] { "318", "36", "99" });
            CreateOrUpdateTableRow(new[] { "319", "36", "100" });

            CreateOrUpdateTableRow(new[] { "320", "37", "94" });
            CreateOrUpdateTableRow(new[] { "321", "37", "95" });
            CreateOrUpdateTableRow(new[] { "322", "37", "96" });
            CreateOrUpdateTableRow(new[] { "323", "37", "97" });
            CreateOrUpdateTableRow(new[] { "324", "37", "98" });
            CreateOrUpdateTableRow(new[] { "325", "37", "99" });
            CreateOrUpdateTableRow(new[] { "326", "37", "100" });

            CreateOrUpdateTableRow(new[] { "327", "36", "12" });
            CreateOrUpdateTableRow(new[] { "328", "36", "4" });
            CreateOrUpdateTableRow(new[] { "329", "36", "7" });
            CreateOrUpdateTableRow(new[] { "331", "37", "12" });
            CreateOrUpdateTableRow(new[] { "332", "37", "4" });
            CreateOrUpdateTableRow(new[] { "333", "37", "7" });

            // 204018
            CreateOrUpdateTableRow(new[] { "334", "38", "41" });
            CreateOrUpdateTableRow(new[] { "335", "38", "2" });
            CreateOrUpdateTableRow(new[] { "336", "38", "3" });
            CreateOrUpdateTableRow(new[] { "337", "38", "4" });
            CreateOrUpdateTableRow(new[] { "338", "38", "5" });
            CreateOrUpdateTableRow(new[] { "339", "38", "6" });
            CreateOrUpdateTableRow(new[] { "340", "38", "7" });
            CreateOrUpdateTableRow(new[] { "341", "38", "8" });
            CreateOrUpdateTableRow(new[] { "342", "38", "9" });
            CreateOrUpdateTableRow(new[] { "343", "38", "10" });
            CreateOrUpdateTableRow(new[] { "344", "38", "11" });
            CreateOrUpdateTableRow(new[] { "345", "38", "12" });

            CreateOrUpdateTableRow(new[] { "346", "39", "41" });
            CreateOrUpdateTableRow(new[] { "347", "39", "2" });
            CreateOrUpdateTableRow(new[] { "348", "39", "3" });
            CreateOrUpdateTableRow(new[] { "349", "39", "4" });
            CreateOrUpdateTableRow(new[] { "350", "39", "5" });
            CreateOrUpdateTableRow(new[] { "351", "39", "6" });
            CreateOrUpdateTableRow(new[] { "352", "39", "7" });
            CreateOrUpdateTableRow(new[] { "353", "39", "8" });
            CreateOrUpdateTableRow(new[] { "354", "39", "9" });
            CreateOrUpdateTableRow(new[] { "355", "39", "10" });
            CreateOrUpdateTableRow(new[] { "356", "39", "11" });
            CreateOrUpdateTableRow(new[] { "357", "39", "12" });
            CreateOrUpdateTableRow(new[] { "358", "39", "42" });
            CreateOrUpdateTableRow(new[] { "359", "39", "43" });
            CreateOrUpdateTableRow(new[] { "360", "39", "44" });
            CreateOrUpdateTableRow(new[] { "361", "39", "45" });
            CreateOrUpdateTableRow(new[] { "362", "39", "46" });
            CreateOrUpdateTableRow(new[] { "363", "39", "47" });
            CreateOrUpdateTableRow(new[] { "364", "39", "48" });
            CreateOrUpdateTableRow(new[] { "365", "39", "49" });
            CreateOrUpdateTableRow(new[] { "366", "39", "53" });
            CreateOrUpdateTableRow(new[] { "367", "39", "1" });

            CreateOrUpdateTableRow(new[] { "368", "40", "41" });
            CreateOrUpdateTableRow(new[] { "369", "40", "2" });
            CreateOrUpdateTableRow(new[] { "370", "40", "3" });
            CreateOrUpdateTableRow(new[] { "371", "40", "4" });
            CreateOrUpdateTableRow(new[] { "372", "40", "5" });
            CreateOrUpdateTableRow(new[] { "373", "40", "6" });
            CreateOrUpdateTableRow(new[] { "374", "40", "7" });
            CreateOrUpdateTableRow(new[] { "375", "40", "8" });
            CreateOrUpdateTableRow(new[] { "376", "40", "9" });
            CreateOrUpdateTableRow(new[] { "377", "40", "10" });
            CreateOrUpdateTableRow(new[] { "378", "40", "11" });
            CreateOrUpdateTableRow(new[] { "379", "40", "12" });
            CreateOrUpdateTableRow(new[] { "380", "40", "42" });
            CreateOrUpdateTableRow(new[] { "381", "40", "43" });
            CreateOrUpdateTableRow(new[] { "382", "40", "44" });
            CreateOrUpdateTableRow(new[] { "383", "40", "45" });
            CreateOrUpdateTableRow(new[] { "384", "40", "46" });
            CreateOrUpdateTableRow(new[] { "385", "40", "47" });
            CreateOrUpdateTableRow(new[] { "386", "40", "48" });
            CreateOrUpdateTableRow(new[] { "387", "40", "1" });

            CreateOrUpdateTableRow(new[] { "388", "36", "101" });
            CreateOrUpdateTableRow(new[] { "389", "37", "101" });

        }
    }
}
