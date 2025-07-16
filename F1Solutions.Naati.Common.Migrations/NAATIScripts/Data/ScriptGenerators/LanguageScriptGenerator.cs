using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class LanguageScriptGenerator : BaseScriptGenerator
    {
        public LanguageScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblLanguage";
        public override IList<string> Columns => new[] {
            "LanguageId",
            "Name",
            "Code",
            "LanguageGroupId"
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Afrikaans", "AFR ", null });
            CreateOrUpdateTableRow(new[] { "3", "Albanian", "ALB ", null });
            CreateOrUpdateTableRow(new[] { "4", "Amharic", "AMH ", null });
            CreateOrUpdateTableRow(new[] { "5", "Anindilyakwa", "ANIN", "1" });
            CreateOrUpdateTableRow(new[] { "6", "Anmatyerr", "AMX", "1" });
            CreateOrUpdateTableRow(new[] { "7", "Arabic", "ARA ", null });
            CreateOrUpdateTableRow(new[] { "9", "Assyrian", "AII", null });
            CreateOrUpdateTableRow(new[] { "10", "Auslan", "ASF", null });
            CreateOrUpdateTableRow(new[] { "13", "Bangla", "BANG", null });
            CreateOrUpdateTableRow(new[] { "15", "Bielorussian", "BIEL", null });
            CreateOrUpdateTableRow(new[] { "16", "Bosnian", "BOS ", null });
            CreateOrUpdateTableRow(new[] { "17", "Bulgarian", "BULG", null });
            CreateOrUpdateTableRow(new[] { "18", "Burmese", "BUR ", null });
            CreateOrUpdateTableRow(new[] { "19", "Burarra", "BURA", "1" });
            CreateOrUpdateTableRow(new[] { "20", "Cantonese", "CANT", null });
            CreateOrUpdateTableRow(new[] { "22", "Cebuano", "CEB ", null });
            CreateOrUpdateTableRow(new[] { "24", "Chinese", "ZHO", null });
            CreateOrUpdateTableRow(new[] { "25", "Chiu-Chownese", "CHIU", null });
            CreateOrUpdateTableRow(new[] { "26", "Cook Islands Maori", "CIM ", null });
            CreateOrUpdateTableRow(new[] { "27", "Croatian", "CRO ", null });
            CreateOrUpdateTableRow(new[] { "28", "Czech", "CZE ", null });
            CreateOrUpdateTableRow(new[] { "29", "Danish", "DAN ", null });
            CreateOrUpdateTableRow(new[] { "30", "Dari", "DARI", null });
            CreateOrUpdateTableRow(new[] { "31", "Djapu", "DJA ", "1" });
            CreateOrUpdateTableRow(new[] { "32", "Djambarrpuyngu", "DJAM", "1" });
            CreateOrUpdateTableRow(new[] { "36", "Dutch", "DUT ", null });
            CreateOrUpdateTableRow(new[] { "37", "Dyirbal", "DYIR", "1" });
            CreateOrUpdateTableRow(new[] { "39", "Eastern Arrernte", "AER", "1" });
            CreateOrUpdateTableRow(new[] { "40", "English", "ENG ", null });
            CreateOrUpdateTableRow(new[] { "42", "Estonian", "EST ", null });
            CreateOrUpdateTableRow(new[] { "44", "Fijian", "FIJ ", null });
            CreateOrUpdateTableRow(new[] { "45", "Filipino", "FIL ", null });
            CreateOrUpdateTableRow(new[] { "46", "Finnish", "FIN ", null });
            CreateOrUpdateTableRow(new[] { "48", "French", "FRE ", null });
            CreateOrUpdateTableRow(new[] { "52", "Fuzhou (dialect of Min Dong Chinese)", "FUZH", null });
            CreateOrUpdateTableRow(new[] { "54", "Gajerrong", "GAJ ", "1" });
            CreateOrUpdateTableRow(new[] { "55", "Garawa", "GAR ", "1" });
            CreateOrUpdateTableRow(new[] { "58", "Georgian", "GEOR", null });
            CreateOrUpdateTableRow(new[] { "59", "German", "GER ", null });
            CreateOrUpdateTableRow(new[] { "61", "Greek", "GRE ", null });
            CreateOrUpdateTableRow(new[] { "62", "Gujarati", "GUJ ", null });
            CreateOrUpdateTableRow(new[] { "63", "Gumatj", "GUM ", "1" });
            CreateOrUpdateTableRow(new[] { "65", "Gupapuyngu", "GUP ", "1" });
            CreateOrUpdateTableRow(new[] { "70", "Hebrew", "HEB ", null });
            CreateOrUpdateTableRow(new[] { "71", "Hindi", "HIN ", null });
            CreateOrUpdateTableRow(new[] { "72", "Hiri Motu", "HIRI", null });
            CreateOrUpdateTableRow(new[] { "73", "Hindustani", "HIST", null });
            CreateOrUpdateTableRow(new[] { "74", "Hmong", "HMO ", null });
            CreateOrUpdateTableRow(new[] { "75", "Hokkien (dialect of Min Nan Chinese)", "HOK ", null });
            CreateOrUpdateTableRow(new[] { "76", "Hungarian", "HUNG", null });
            CreateOrUpdateTableRow(new[] { "77", "Ibo", "IBO ", null });
            CreateOrUpdateTableRow(new[] { "78", "Icelandic", "ICE ", null });
            CreateOrUpdateTableRow(new[] { "81", "Indonesian", "IND ", null });
            CreateOrUpdateTableRow(new[] { "82", "Italian", "IT  ", null });
            CreateOrUpdateTableRow(new[] { "83", "Iwaidja", "IWA ", "1" });
            CreateOrUpdateTableRow(new[] { "84", "Japanese", "JAP ", null });
            CreateOrUpdateTableRow(new[] { "85", "Jaru", "JARU", "1" });
            CreateOrUpdateTableRow(new[] { "91", "Khmer", "KHM ", null });
            CreateOrUpdateTableRow(new[] { "92", "Kija", "KIJA", "1" });
            CreateOrUpdateTableRow(new[] { "93", "Kinyarwanda", "KINY", null });
            CreateOrUpdateTableRow(new[] { "95", "Kirundi", "KIRU", null });
            CreateOrUpdateTableRow(new[] { "98", "Korean", "KOR ", null });
            CreateOrUpdateTableRow(new[] { "99", "Kriol", "ROP", null });
            CreateOrUpdateTableRow(new[] { "100", "Kukatja", "KUKA", "1" });
            CreateOrUpdateTableRow(new[] { "101", "Kunwinjku", "KUN ", "1" });
            CreateOrUpdateTableRow(new[] { "102", "Kurmanji Kurdish", "KURK", null });
            CreateOrUpdateTableRow(new[] { "103", "Sorani Kurdish", "KURS", null });
            CreateOrUpdateTableRow(new[] { "104", "Lao", "LAO ", null });
            CreateOrUpdateTableRow(new[] { "105", "Latin", "LAT ", null });
            CreateOrUpdateTableRow(new[] { "106", "Latvian", "LATV", null });
            CreateOrUpdateTableRow(new[] { "108", "Lingala", "LING", null });
            CreateOrUpdateTableRow(new[] { "109", "Lithuanian", "LITH", null });
            CreateOrUpdateTableRow(new[] { "110", "Liyagalawumirr", "LIYA", "1" });
            CreateOrUpdateTableRow(new[] { "112", "Macedonian", "MAC ", null });
            CreateOrUpdateTableRow(new[] { "113", "Malay", "MAL ", null });
            CreateOrUpdateTableRow(new[] { "114", "Malayalam", "MALA", null });
            CreateOrUpdateTableRow(new[] { "115", "Maltese", "MLT", null });
            CreateOrUpdateTableRow(new[] { "116", "Mandarin", "CMN", null });
            CreateOrUpdateTableRow(new[] { "118", "Marathi", "MARA", null });
            CreateOrUpdateTableRow(new[] { "119", "Martu Wangka", "MART", "1" });
            CreateOrUpdateTableRow(new[] { "121", "Miriwung", "MIRI", "1" });
            CreateOrUpdateTableRow(new[] { "122", "Mongolian", "MONG", null });
            CreateOrUpdateTableRow(new[] { "123", "Modern Tiwi", "MTIW", "1" });
            CreateOrUpdateTableRow(new[] { "125", "Murrinh-Patha", "MURR", "1" });
            CreateOrUpdateTableRow(new[] { "127", "Ngaanyatjarra", "NGAA", "1" });
            CreateOrUpdateTableRow(new[] { "128", "Norwegian", "NOR ", null });
            CreateOrUpdateTableRow(new[] { "131", "Nyangumarta", "NYAN", "1" });
            CreateOrUpdateTableRow(new[] { "134", "Persian", "PERS", null });
            CreateOrUpdateTableRow(new[] { "135", "Melanesian Pidgin English", "PIDG", null });
            CreateOrUpdateTableRow(new[] { "136", "Pitjantjatjara", "PIT ", "1" });
            CreateOrUpdateTableRow(new[] { "137", "Polish", "POL ", null });
            CreateOrUpdateTableRow(new[] { "138", "Portuguese", "POR ", null });
            CreateOrUpdateTableRow(new[] { "140", "Pukapukan", "PUKA", null });
            CreateOrUpdateTableRow(new[] { "141", "Punjabi", "PUNJ", null });
            CreateOrUpdateTableRow(new[] { "142", "Pashto", "PUSH", null });
            CreateOrUpdateTableRow(new[] { "143", "Romanian", "ROM ", null });
            CreateOrUpdateTableRow(new[] { "145", "Russian", "RUSS", null });
            CreateOrUpdateTableRow(new[] { "146", "Samoan", "SAM ", null });
            CreateOrUpdateTableRow(new[] { "148", "Serbian", "SERB", null });
            CreateOrUpdateTableRow(new[] { "150", "Sindhi", "SIND", null });
            CreateOrUpdateTableRow(new[] { "151", "Sinhalese", "SINH", null });
            CreateOrUpdateTableRow(new[] { "152", "Slovak", "SLOK", null });
            CreateOrUpdateTableRow(new[] { "153", "Slovene", "SLON", null });
            CreateOrUpdateTableRow(new[] { "154", "Somali", "SOM ", null });
            CreateOrUpdateTableRow(new[] { "155", "Spanish", "SPAN", null });
            CreateOrUpdateTableRow(new[] { "156", "Sundanese", "SUND", null });
            CreateOrUpdateTableRow(new[] { "158", "Swatow (dialect of Min Nan Chinese)", "SWAT", null });
            CreateOrUpdateTableRow(new[] { "159", "Swedish", "SWED", null });
            CreateOrUpdateTableRow(new[] { "160", "Syriac", "SYR ", null });
            CreateOrUpdateTableRow(new[] { "161", "Taiwanese (dialect of Min Nan Chinese)", "TAIW", null });
            CreateOrUpdateTableRow(new[] { "162", "Tamil", "TAM ", null });
            CreateOrUpdateTableRow(new[] { "164", "Telugu", "TELU", null });
            CreateOrUpdateTableRow(new[] { "165", "Teochew (dialect of Min Nan Chinese)", "TEOC", null });
            CreateOrUpdateTableRow(new[] { "166", "Tetum", "TET ", null });
            CreateOrUpdateTableRow(new[] { "167", "Tigré", "TGR ", null });
            CreateOrUpdateTableRow(new[] { "168", "Thai", "THAI", null });
            CreateOrUpdateTableRow(new[] { "169", "Tibetan", "TIB ", null });
            CreateOrUpdateTableRow(new[] { "170", "Tigrinya", "TIG ", null });
            CreateOrUpdateTableRow(new[] { "172", "Traditional Tiwi", "TIWI", "1" });
            CreateOrUpdateTableRow(new[] { "174", "Tongan", "TON ", null });
            CreateOrUpdateTableRow(new[] { "175", "Yumplatok", "TSIC", "1" });
            CreateOrUpdateTableRow(new[] { "176", "Turkish", "TURK", null });
            CreateOrUpdateTableRow(new[] { "178", "Ukrainian", "UKR ", null });
            CreateOrUpdateTableRow(new[] { "179", "Urdu", "URDU", null });
            CreateOrUpdateTableRow(new[] { "180", "Uzbek", "UZB ", null });
            CreateOrUpdateTableRow(new[] { "181", "Vietnamese", "VIET", null });
            CreateOrUpdateTableRow(new[] { "183", "Walmajarri", "WAL ", "1" });
            CreateOrUpdateTableRow(new[] { "184", "Wangkatha", "WANG", "1" });
            CreateOrUpdateTableRow(new[] { "185", "Warlpiri", "WARL", "1" });
            CreateOrUpdateTableRow(new[] { "186", "Warumungu", "WARU", "1" });
            CreateOrUpdateTableRow(new[] { "189", "Wik-Mungkan", "WIK ", "1" });
            CreateOrUpdateTableRow(new[] { "190", "Shanghainese (dialect of Wu Chinese)", "WU  ", null });
            CreateOrUpdateTableRow(new[] { "191", "Yanyuwa", "YAN ", "1" });
            CreateOrUpdateTableRow(new[] { "192", "Yankunytjatjara", "YANK", "1" });
            CreateOrUpdateTableRow(new[] { "193", "Yiddish", "YID ", null });
            CreateOrUpdateTableRow(new[] { "194", "Yindjibarndi", "YIND", "1" });
            CreateOrUpdateTableRow(new[] { "296", "Non Language Specifice", "NLANGSPEC", null });
            CreateOrUpdateTableRow(new[] { "396", "Acholi", "ACH", null });
            CreateOrUpdateTableRow(new[] { "398", "Kariyarra", "KARI", "1" });
            CreateOrUpdateTableRow(new[] { "399", "Alyawarr", "ALY", "1" });
            CreateOrUpdateTableRow(new[] { "400", "Krio", "KRI", null });
            CreateOrUpdateTableRow(new[] { "401", "Mende", "MEN", null });
            CreateOrUpdateTableRow(new[] { "402", "Kakwa", "KAK", null });
            CreateOrUpdateTableRow(new[] { "495", "Fur", "FUR", null });
            CreateOrUpdateTableRow(new[] { "496", "Motu", "MOTU", null });
            CreateOrUpdateTableRow(new[] { "497", "Ganda", "GANDA", null });
            CreateOrUpdateTableRow(new[] { "596", "Mandingo", "MAN", null });
            CreateOrUpdateTableRow(new[] { "597", "Ma'di", "MAD", null });
            CreateOrUpdateTableRow(new[] { "598", "Bari", "BARI", null });
            CreateOrUpdateTableRow(new[] { "599", "Liberian English", "LIEN", null });
            CreateOrUpdateTableRow(new[] { "600", "Shilluk", "SHIL", null });
            CreateOrUpdateTableRow(new[] { "601", "Bassa", "BASS", null });
            CreateOrUpdateTableRow(new[] { "603", "Zande", "ZAND", null });
            CreateOrUpdateTableRow(new[] { "604", "Uyghur", "UYG", null });
            CreateOrUpdateTableRow(new[] { "695", "Fulfulde", "FUL", null });
            CreateOrUpdateTableRow(new[] { "795", "Falam Chin", "CFM", "2" });
            CreateOrUpdateTableRow(new[] { "797", "Khumi Chin", "CNK", "2" });
            CreateOrUpdateTableRow(new[] { "798", "Mara Chin", "MRH", "2" });
            CreateOrUpdateTableRow(new[] { "799", "Zotung Chin", "CZT", "2" });
            CreateOrUpdateTableRow(new[] { "996", "Armenian", "ARM", null });
            CreateOrUpdateTableRow(new[] { "997", "Dinka", "DINKA", null });
            CreateOrUpdateTableRow(new[] { "998", "Nuer", "NUER", null });
            CreateOrUpdateTableRow(new[] { "999", "Oromo", "OROMO", null });
            CreateOrUpdateTableRow(new[] { "1000", "Swahili", "SWA", null });
            CreateOrUpdateTableRow(new[] { "1095", "Aceh", "ACE", null });
            CreateOrUpdateTableRow(new[] { "1097", "Pulaar", "FUC", null });
            CreateOrUpdateTableRow(new[] { "1099", "Hassaniyya", "MEY", null });
            CreateOrUpdateTableRow(new[] { "1195", "Hazaragi", "HAZ", null });
            CreateOrUpdateTableRow(new[] { "1198", "Fiji Hindi", "HIF", null });
            CreateOrUpdateTableRow(new[] { "1199", "Min Nan Chinese", "NAN", null });
            CreateOrUpdateTableRow(new[] { "1200", "Tedim Chin", "CTD", "2" });
            CreateOrUpdateTableRow(new[] { "1203", "Mizo Chin", "LUS", "2" });
            CreateOrUpdateTableRow(new[] { "1204", "Kaytetye", "GBB", "1" });
            CreateOrUpdateTableRow(new[] { "1395", "New Zealand Sign Language", "NZSL", null });
            CreateOrUpdateTableRow(new[] { "1410", "Nepali", "NEP", null });
            CreateOrUpdateTableRow(new[] { "1411", "Pwo Eastern Karen", "KIP", null });
            CreateOrUpdateTableRow(new[] { "1412", "S'gaw Karen", "KSW", null });
            CreateOrUpdateTableRow(new[] { "1415", "Hakka Chinese", "HAK", null });
            CreateOrUpdateTableRow(new[] { "1425", "Hakha Chin", "CNH", "2" });
            CreateOrUpdateTableRow(new[] { "1435", "American Sign Language", "ASE", null });
            CreateOrUpdateTableRow(new[] { "1440", "Turkmen", "TUK", null });
            CreateOrUpdateTableRow(new[] { "1450", "Western Arrarnta", "ARE", "1" });
            CreateOrUpdateTableRow(new[] { "1461", "Pintupi-Luritja", "PIU", "1" });
            CreateOrUpdateTableRow(new[] { "1480", "Nigerian Pidgin", "PCM", null });
            CreateOrUpdateTableRow(new[] { "1485", "Roper River Kriol", "ROP", "1" });
            CreateOrUpdateTableRow(new[] { "1486", "Gurindji Kriol", "ROP", "1" });
            CreateOrUpdateTableRow(new[] { "1487", "Fitzroy Valley Kriol", "ROP", "1" });
            CreateOrUpdateTableRow(new[] { "1488", "Kimberley Kriol", "ROP", "1" });
            CreateOrUpdateTableRow(new[] { "1489", "Adapted Sign Language", "ASL", null });
            CreateOrUpdateTableRow(new[] { "1490", "Non-Conventional Sign Language", "NCSL", null });
            CreateOrUpdateTableRow(new[] { "1491", "Other Conventional Sign Language", "OCSL", null });
            CreateOrUpdateTableRow(new[] { "1492", "Written English", "WEN", null });
            CreateOrUpdateTableRow(new[] { "1505", "Matu Chin", "HLT", "2" });
            CreateOrUpdateTableRow(new[] { "1515", "Zo", "ZOM", null });
            CreateOrUpdateTableRow(new[] { "1530", "Kalaw Kawaw Ya (dialect of Kala Lagaw Ya)", "KKY", "1" });
            CreateOrUpdateTableRow(new[] { "1540", "Deaf Interpreter", "DEF", null });
            CreateOrUpdateTableRow(new[] { "1545", "Chaldean", "CHAL", null });
            CreateOrUpdateTableRow(new[] { "1546", "Southern Kurdish", "SDH", null });
            CreateOrUpdateTableRow(new[] { "1551", "Rohingya", "RHG", null });
            CreateOrUpdateTableRow(new[] { "1555", "Kija Kriol", "KKIJ", "1" });
            CreateOrUpdateTableRow(new[] { "2000", "Ethics", "ETHIC", "3" });
            CreateOrUpdateTableRow(new[] { "2001", "Intercultural", "INTERC", "3" });
            CreateOrUpdateTableRow(new[] { "2002", "Migration Assessment", "MIGRAT", null });
            CreateOrUpdateTableRow(new[] { "2003", "North Azerbaijani", "AZJ", null });
            CreateOrUpdateTableRow(new[] { "2004", "South Azerbaijani", "AZB", null });
            CreateOrUpdateTableRow(new[] { "2005", "Kayah", "EKY", null });
            CreateOrUpdateTableRow(new[] { "2006", "Afar", "AAR", null});
            CreateOrUpdateTableRow(new[] { "2007", "Tok Pisin", "TPI", null });
            CreateOrUpdateTableRow(new[] { "2008", "Shona", "SNA", null });
            CreateOrUpdateTableRow(new[] { "2009", "Mauritian Creole", "MFE", null });

            //these are deleted item and added again - to be confirmed
            CreateOrUpdateTableRow(new[] { "111", "Luritja", "LUR", "1" });
            CreateOrUpdateTableRow(new[] { "130", "Nunggubuyu", "NUNG", "1" });
            CreateOrUpdateTableRow(new[] { "1001", "Meriam, Miriam-Mir", "MER", "1" });
            

        }
    }
}
